const spPath = "_content/JSWrapper/Dummy.html";

function IsSpecial(request) {
    let url = new URL(request.url);
    if (url.pathname.endsWith(spPath)) {
        return true;
    } else {
        return false;
    }
}

async function GetSpecialResponse(request) {
    let url = new URL(request.url);
    let response = new Response("This is a sample of response." + "action is " + url.searchParams.get("action"));
    response.status = 200;
    return response;
}