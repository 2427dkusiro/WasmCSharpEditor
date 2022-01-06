import { BrotliDecode } from './decode.min.js';

Blazor.start({
    loadBootResource: function (type, name, defaultUri, integrity) {
        // this means app is in debug mode.
        if (location.hostname == "localhost" && location.port != "5500") {
            const span = document.getElementById("ProgressLoadMode");
            if (span.textContent == "") {
                span.textContent = "Brotli圧縮無効";
                span.className += " text-danger";
            }
            return;
        }

        if (type !== 'dotnetjs') {
            return (async function () {
                const response = await fetch(defaultUri + '.br', { cache: 'no-cache' });
                if (!response.ok) {
                    throw new Error(response.statusText);
                }
                const originalResponseBuffer = await response.arrayBuffer();
                const originalResponseArray = new Int8Array(originalResponseBuffer);
                const decompressedResponseArray = BrotliDecode(originalResponseArray);

                const contentType = type ===
                    'dotnetwasm' ? 'application/wasm' : 'application/octet-stream';
                return new Response(decompressedResponseArray,
                    { headers: { 'content-type': contentType } });
            })();
        }
    }
});