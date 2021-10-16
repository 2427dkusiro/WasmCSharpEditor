self.importScripts('./decode.min.js');
self.importScripts("_content/JSWrapper/js/OnFetchHandler.js");
self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));
self.addEventListener("message", event => OnMessage(event.data));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [/\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/];
const offlineAssetsExclude = [/^service-worker\.js$/];

async function onInstall(event) {
    console.info('Service worker: Install');

    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url + ".br")); // ハッシュ照合を無効化、代替手法の検討必要か?
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    console.log("fetch requested:" + event.request);
    if (IsSpecial(event.request)) {
        let response = await GetSpecialResponse(event.request);
        return response;
    }
    let cachedResponse = null;
    if (event.request.method === 'GET') {
        const shouldServeIndexHtml = event.request.mode === 'navigate';
        const request = (shouldServeIndexHtml ? 'index.html' : event.request) + ".br";
        const cache = await caches.open(cacheName);
        cachedResponse = await cache.match(request);
    }
    const response = cachedResponse || await fetch(event.request);
    const originalResponseBuffer = await response.arrayBuffer();
    const originalResponseArray = new Int8Array(originalResponseBuffer);
    const decompressedResponseArray = BrotliDecode(originalResponseArray);
    const contentType = event.request.headers.get("content-type");
    return new Response(decompressedResponseArray,
        { headers: { 'content-type': contentType } });
}
/* Manifest version: Xrf+qFh4 */
