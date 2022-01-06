importScripts("_content/JSWrapper/js/OnFetchHandler.js");

self.addEventListener('fetch', event => event.respondWith(onFetch(event)));
self.addEventListener("message", event => OnMessage(event.data));

async function onFetch(event) {
    let cachedResponse = null;
    if (IsSpecial(event.request)) {
        let response = await GetSpecialResponse(event.request);
        return response;
    }

    return cachedResponse || fetch(event.request);
}
