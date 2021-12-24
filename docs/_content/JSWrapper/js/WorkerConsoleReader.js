let baseUrl;
const requestUrl = "_content/JSWrapper/Dummy.html"

function SetBaseUrl(url) {
    baseUrl = url;
}

function GetInput(guid) {
    const xhr = new XMLHttpRequest();
    const url = new URL(requestUrl, baseUrl);
    url.searchParams.set("action", "GetInput");
    url.searchParams.set("id", guid);
    xhr.open("GET", url, false);
    xhr.send(null);

    return JSON.stringify({ Status: xhr.status, Response: xhr.response });
}