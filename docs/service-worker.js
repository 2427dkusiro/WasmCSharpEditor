self.importScripts('./workerdecode.min.js');
self.importScripts("./_content/JSWrapper/js/OnFetchHandler.js");
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

    // service worker�̍X�V�𑦓K�p
    event.waitUntil(self.skipWaiting());
    /*
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url + ".br")); // �n�b�V���ƍ��𖳌����A��֎�@�̌����K�v��?
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
    */
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // service worker�����񂩂�L����
    event.waitUntil(self.clients.claim());
    /*
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
    */
}

async function onFetch(event) {
    // ���������Ώۂ̓��ꃊ�N�G�X�g���ǂ����m���߁A�����Ȃ�Ώ������������X�|���X��Ԃ�
    if (IsSpecial(event.request)) {
        let response = await GetSpecialResponse(event.request);
        return response;
    }
    return await fetch(event.request);
    // GET�Ȃ�Έ��k/�L���b�V���ɂ��]���ʍ팸�A�v���[�`������
    // let cachedResponse = null;
    /*
    if (event.request.method === 'GET') {
        const shouldServeIndexHtml = event.request.mode === 'navigate';
        const requestUrl = (shouldServeIndexHtml ? 'index.html' : event.request.url);
        let reWritedUrl = requestUrl;

        let decodeRequired = false;
        // ���k�ς݂�v�����Ă��Ȃ���΁A���k�֏�������
        if (!requestUrl.endsWith(".br")) {
            decodeRequired = true;
            reWritedUrl = requestUrl + ".br";
        }
        const request = new Request(reWritedUrl, { method: "GET" });
        const cache = await caches.open(cacheName);
        let response = await cache.match(request) || await fetch(request);
        if (!response.ok) {
            throw new Error(response.statusText);
        }
        if (decodeRequired) {
            const originalResponseBuffer = await response.arrayBuffer();
            const originalResponseArray = new Int8Array(originalResponseBuffer);
            const decompressedResponseArray = BrotliDecode(originalResponseArray);
            const contentType = getMIMEType(requestUrl);
            response = new Response(decompressedResponseArray, { headers: { 'content-type': contentType } });
        }
        cachedResponse = response;
    }
    */
    // return cachedResponse || await fetch(event.request);
}

function getMIMEType(url) {
    if (url.endsWith(".dll") || url.endsWith(".pdb")) {
        return "application/octet-stream";
    }
    if (url.endsWith(".wasm")) {
        return "application/wasm";
    }
    if (url.endsWith(".html")) {
        return "text/html";
    }
    if (url.endsWith(".js")) {
        return "text/javascript";
    }
    if (url.endsWith(".json")) {
        return "application/json";
    }
    if (url.endsWith(".css")) {
        return "text/css";
    }
    if (url.endsWith(".woff")) {
        return "font/woff";
    }
}
/* Manifest version: nVg+Yts8 */
