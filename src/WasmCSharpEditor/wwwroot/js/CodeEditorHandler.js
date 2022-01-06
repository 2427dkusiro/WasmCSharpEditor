/**
 * @type CodeMirror.EditorFromTextArea
 * */
let editer;

/**
 * @type string
 * */
let textAreaId;

/**
 * @type string
 * */
let eventGuid;

/** 
 * 現在のエディタの内容をtextAreaと同期し、その値を取得します。
 * @returns {string} 現在のコード。
 * */
export function Save() {
    editer.save();
    return document.getElementById(textAreaId).value;
}

/**
 * .NETへのイベント通知を有効にします。
 * @param guid {string} イベント通知で利用するID。
 * */
export function EnableEventRaising(guid) {
    eventGuid = guid;
}

/**
 * コードエディタを初期化します。
 * @param {string} id コードエディタにするtextAreaの、HTMLのid属性の値。
 * @param {any} option コードエディタの初期化オプション。
 */
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
    DotNet.invokeMethodAsync("WasmCSharpEditor", "RaiseOnChangeEventFromJs", eventGuid);
}