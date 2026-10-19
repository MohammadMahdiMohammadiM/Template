// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [/\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/, /\.webmanifest$/];
const offlineAssetsExclude = [/^service-worker\.js$/];

// Replace with your base path if you are hosting on a subfolder. Ensure there is a trailing '/'.
const base = "/";
const baseUrl = new URL(base, self.origin);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

async function onInstall(event) {
    console.info('Service worker: Install');

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

// For all navigation requests, try to serve index.html from cache,
// unless that request is for an offline resource.
// If you need some URLs to be server-rendered, edit the following check to exclude those URLs
// async function onFetch(event) {
//     let cachedResponse = null;
//     if (event.request.method === 'GET') {
//         const shouldServeIndexHtml = event.request.mode === 'navigate'
//             && !manifestUrlList.some(url => url === event.request.url);

//         const request = shouldServeIndexHtml ? 'index.html' : event.request;
//         const cache = await caches.open(cacheName);
//         cachedResponse = await cache.match(request);
//     }

//     return cachedResponse || fetch(event.request);
// }

async function onFetch(event) {
    if (event.request.method !== 'GET') {
        return fetch(event.request);
    }

    // Always try the network first for page navigation.
    // This prevents an old cached index.html from starting
    // a newer/older Blazor deployment with mismatched assets.
    if (event.request.mode === 'navigate') {
        try {
            return await fetch(event.request);
        }
        catch {
            // If the network is unavailable, fall back to the
            // cached index.html so offline support still works.
            const cache = await caches.open(cacheName);
            const cachedResponse = await cache.match('index.html');

            if (cachedResponse) {
                return cachedResponse;
            }

            throw new Error('No network connection and no cached index.html available.');
        }
    }

    // For other GET requests, keep the existing cache-first behavior.
    const cache = await caches.open(cacheName);
    const cachedResponse = await cache.match(event.request);

    return cachedResponse || fetch(event.request);
}