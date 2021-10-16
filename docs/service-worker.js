self.importScripts('./decode.min.js');
self.importScripts("_content/JSWrapper/js/OnFetchHandler.js");
self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));
self.addEventListener("message", event => OnMessage(event.data));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [/\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/, /\.ico$/];
const offlineAssetsExclude = [/^service-worker\.js$/];

async function onInstall(event) {
    console.info('Service worker: Install');

    // service workerの更新を即適用
    event.waitUntil(self.skipWaiting());
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url + ".br")); // ハッシュ照合を無効化、代替手法の検討必要か?
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // service workerを初回から有効化
    event.waitUntil(self.clients.claim());
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    // 書き換え対象の特殊リクエストかどうか確かめ、そうならば書き換えたレスポンスを返す
    if (IsSpecial(event.request)) {
        let response = await GetSpecialResponse(event.request);
        return response;
    }
    // GETならば圧縮/キャッシュによる転送量削減アプローチを検討
    let cachedResponse = null;
    if (event.request.method === 'GET') {
        const shouldServeIndexHtml = event.request.mode === 'navigate';
        let requestUrl = (shouldServeIndexHtml ? 'index.html' : event.request.url);

        let decodeRequired = false;
        // 圧縮済みを要求していなければ、圧縮へ書き換え
        if (!requestUrl.endsWith(".br")) {
            decodeRequired = true;
            requestUrl = requestUrl + ".br";
        }
        const request = new Request(requestUrl, { method: "GET" });
        const cache = await caches.open(cacheName);
        let response = await cache.match(request) || await fetch(request);
        if (decodeRequired) {
            const originalResponseBuffer = await response.arrayBuffer();
            const originalResponseArray = new Int8Array(originalResponseBuffer);
            const decompressedResponseArray = BrotliDecode(originalResponseArray);
            const contentType = event.request.headers.get("content-type");
            console.log("content-type" + contentType);
            response = new Response(decompressedResponseArray, { headers: { 'content-type': contentType } });
        }
        cachedResponse = response;
    }
    return cachedResponse || await fetch(event.request);
}
/* Manifest version: X9mkZwIM */
