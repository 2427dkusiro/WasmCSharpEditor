let baseUrl;
const requestUrl = "_content/JSWrapper/Dummy.html"

function SetBaseUrl(url) {
    baseUrl = url;
}

function GetInput() {
    let xhr = new XMLHttpRequest();
    let url = new URL(requestUrl, baseUrl);
    url.searchParams.set("action", "GetInput");
    xhr.open("GET", url, false);
    xhr.send(null);

    return JSON.stringify({ Status: xhr.status, Response: xhr.response });
}