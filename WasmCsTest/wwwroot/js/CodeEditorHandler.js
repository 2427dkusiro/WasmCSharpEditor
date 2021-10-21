let editer;
let textAreaId;
let eventGuid;

export function Save() {
    editer.save();
    return document.getElementById(textAreaId).value;
}

export function EnableEventRaising(guid) {
    eventGuid = guid;
}

export function InitializeCodeEditor(id, option) {
    console.log(option);
    document.getElementById(id).value = option.value;
    editer = CodeMirror.fromTextArea(document.getElementById(id), {
        indentUnit: option.indentUnit,
        mode: "text/x-csharp",
        lineNumbers: true
    });
    editer.on("change", OnChange)
    textAreaId = id;
}

function OnChange() {
    if (eventGuid == null) {
        return;
    }
    DotNet.invokeMethodAsync("WasmCsTest", "RaiseOnChangeEventFromJs", eventGuid);
}