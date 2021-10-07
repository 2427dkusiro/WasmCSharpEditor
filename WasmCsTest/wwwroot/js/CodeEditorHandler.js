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

// id:String textAreaのid属性の値。
export function InitializeCodeEditor(id) {
    editer = CodeMirror.fromTextArea(document.getElementById(id), {
        mode: "text/x-csharp",
        lineNumbers: true,
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