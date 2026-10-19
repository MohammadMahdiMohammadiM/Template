window.portfolio = (() => {
    let token = 0;
    let frame = 0;
    let cleanup = [];

    const reduced = matchMedia("(prefers-reduced-motion: reduce)");
    const clamp = (v, min, max) => Math.min(Math.max(v, min), max);
    const maxScroll = () => document.documentElement.scrollHeight - innerHeight;
    const outCubic = t => 1 - (1 - t) ** 3;
    const inOutSine = t => -(Math.cos(Math.PI * t) - 1) / 2;

    //Browser Informations
    function getBrowserInfo() {
        return {
            userAgent: navigator.userAgent,
            languages: Array.from(navigator.languages || []),
            referrer: document.referrer,
            timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone,
            screenWidth: screen.width,
            screenHeight: screen.height
        };
    }

    // Cookie helpers. C# decides which culture to use; these only read and write the cookie.
    function setCookieWithOptions(name, value, options) {
        let cookie = `${encodeURIComponent(name)}=${encodeURIComponent(value)}`;
        if (options.expires) cookie += `; expires=${new Date(options.expires).toUTCString()}`;
        if (options.maxAge !== null && options.maxAge !== undefined) cookie += `; max-age=${options.maxAge}`;
        cookie += `; path=${options.path || "/"}`;
        if (options.secure && location.protocol === "https:") cookie += "; Secure";
        if (options.sameSite) cookie += `; SameSite=${options.sameSite}`;
        document.cookie = cookie;
    }

    function getCookie(name) {
        const prefix = encodeURIComponent(name) + "=";
        const part = document.cookie.split("; ").find(c => c.startsWith(prefix));
        return part ? decodeURIComponent(part.substring(prefix.length)) : null;
    }

    // Removes a cookie by setting its expiry date in the past.
    function deleteCookie(name, path) {
        document.cookie = `${encodeURIComponent(name)}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=${path || "/"}`;
    }

    // Runs when this script loads, before Blazor starts, so the direction is right from the first paint.
    (function setDirectionEarly() {
        try {
            const saved = getCookie("Culture.Cookie");
            const isPersian = saved ? saved.startsWith("fa") : false;
            
            document.documentElement.lang = isPersian ? "fa" : "en";
            document.documentElement.dir = isPersian ? "rtl" : "ltr";
        } catch { }
    })();

    // Moves the page from one scroll position to another. Stops if "token" changes.
    function tween(from, to, ms, ease, id) {
        return new Promise(done => {
            const start = performance.now();
            (function step(now) {
                if (id !== token) return done(false);
                const t = Math.min((now - start) / ms, 1);
                scrollTo(0, from + (to - from) * ease(t));
                t < 1 ? requestAnimationFrame(step) : done(true);
            })(start);
        });
    }

    // Slow down, roll a little past the goal, then settle back onto it.
    async function goTo(id) {
        const el = document.getElementById(id);
        if (!el) return;

        const navH = document.querySelector(".pf-nav").offsetHeight;
        const from = scrollY;
        const target = clamp(el.getBoundingClientRect().top + from - navH, 0, maxScroll());
        const distance = target - from;
        if (Math.abs(distance) < 2) return;

        const me = ++token;
        if (reduced.matches) return scrollTo(0, target);

        const overshoot = Math.min(56, Math.abs(distance) * 0.15);
        const over = clamp(target + Math.sign(distance) * overshoot, 0, maxScroll());
        const ms = clamp(450 + Math.abs(distance) * 0.45, 600, 1500);

        const finished = await tween(from, over, ms, outCubic, me);
        if (finished && over !== target) await tween(over, target, 420, inOutSine, me);
    }

    // Runs at most once per screen refresh while the user scrolls.
    function update() {
        frame = 0;

        const footer = document.querySelector(".pf-footer");
        if (footer) {
            const r = footer.getBoundingClientRect();
            const visible = clamp((innerHeight - r.top) / r.height, 0, 1);
            footer.style.setProperty("--reveal", visible.toFixed(3));
        }

        let current = "";
        document.querySelectorAll("main section[id]").forEach(s => {
            if (s.getBoundingClientRect().top <= innerHeight * 0.4) current = s.id;
        });
        document.querySelectorAll(".pf-nav nav a").forEach(a =>
            a.classList.toggle("active", a.dataset.scroll === current));
    }

    function init() {
        destroy();

        const on = (target, name, fn, options) => {
            target.addEventListener(name, fn, options);
            cleanup.push(() => target.removeEventListener(name, fn, options));
        };
        const requestUpdate = () => { frame ||= requestAnimationFrame(update); };

        // Capture phase: runs before Blazor's own link handling.
        on(document, "click", e => {
            const link = e.target.closest("[data-scroll]");
            if (!link) return;
            e.preventDefault();
            e.stopPropagation();
            goTo(link.dataset.scroll);
        }, true);

        // If the visitor scrolls by hand, stop the automatic scrolling.
        ["wheel", "touchstart", "keydown"].forEach(name => on(window, name, () => token++, { passive: true }));
        on(window, "scroll", requestUpdate, { passive: true });
        on(window, "resize", requestUpdate);

        const observer = new IntersectionObserver(entries =>
            entries.forEach(e => e.target.classList.toggle("is-visible", e.isIntersecting)),
            { threshold: 0.2 });
        document.querySelectorAll("[data-reveal]").forEach(el => observer.observe(el));
        cleanup.push(() => observer.disconnect());

        update();
    }

    function destroy() {
        token++;
        cleanup.forEach(fn => fn());
        cleanup = [];
    }

    return { init, destroy, getCookie, setCookieWithOptions, deleteCookie };
})();
