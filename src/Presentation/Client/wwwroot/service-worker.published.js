// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [/\.pdb$/, /\.html/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.webmanifest$/];
const offlineAssetsExclude = [/^service-worker\.js$/];

// Replace with your base path if you are hosting on a subfolder. Ensure there is a trailing '/'.
const base = "/";
const baseUrl = new URL(base, self.origin);
const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

async function onInstall(event) {
    self.skipWaiting();
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

    // Take control of the open pages now, instead of at the next navigation
    await self.clients.claim();
}

async function onFetch(event) {
    if (event.request.method !== 'GET') {
        return fetch(event.request);
    }

    const cache = await caches.open(cacheName);

    // Page navigation: network first, so index.html always matches the newest deployment.
    if (event.request.mode === 'navigate') {
        try {
            return await fetch(event.request.url, { cache: 'no-cache' });
        }
        catch {
            // Offline: fall back to the cached index.html.
            const cachedIndex = await cache.match('index.html');
            if (cachedIndex) {
                return cachedIndex;
            }
            throw new Error('No network connection and no cached index.html available.');
        }
    }

    // Your own CSS and JS keep the same name between deployments, so ask the network first.
    const url = new URL(event.request.url);
    const isAppFile = url.origin === self.location.origin
        && !url.pathname.includes('/_framework/')
        && (url.pathname.endsWith('.css') || url.pathname.endsWith('.js'));

    if (isAppFile) {
        try {
            return await fetch(event.request, { cache: 'no-cache' });
        }
        catch {
            const cachedFile = await cache.match(event.request);
            if (cachedFile) {
                return cachedFile;
            }
            throw new Error('No network connection and no cached copy available.');
        }
    }

    // Everything else (framework files with hashed names, images, fonts): cache first.
    const cachedResponse = await cache.match(event.request);
    return cachedResponse || fetch(event.request);
}