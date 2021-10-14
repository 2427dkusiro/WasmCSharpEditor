importScripts("_content/JSWrapper/js/OnFetchHandler.js");

self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

async function onFetch(event) {
    let cachedResponse = null;
    if (IsSpecial(event.request)) {
        let response = await GetSpecialResponse(event.request);
        return response;
    }

    return cachedResponse || fetch(event.request);
}
