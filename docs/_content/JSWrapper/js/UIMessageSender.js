export function SendMessage(message, guid, type) {
    navigator.serviceWorker.controller.postMessage(JSON.stringify({ message: message, guid: guid, type: type }));
}