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
    const action = url.searchParams.get("action");
    if (action == "GetInput") {
        const guid = url.searchParams.get("id");
        const value = await GetMessage(guid, 30000);
        const response = new Response(value);
        response.status = 200;
        return response;
    }
}

const inputTable = new Map();
const waitUnit = 200;

async function GetMessage(guid, timeout) {
    const count = timeout == -1 ? Number.MAX_VALUE : timeout / waitUnit + 1;
    for (i = 0; i < count; i++) {
        if (inputTable.has(guid)) {
            const value = inputTable.get(guid);
            inputTable.delete(guid);
            return value;
        }
        await delay(waitUnit);
    }
    return null;
}

function OnMessage(message) {
    const obj = JSON.parse(message);
    if (obj.type == "Input") {
        inputTable.set(obj.guid, obj.message);
    }
}

function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}