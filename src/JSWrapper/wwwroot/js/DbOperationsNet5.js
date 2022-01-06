import Dexie from "./../lib/dexie/dexie.js";

/**
 * @constant データベース名。
 * */
const dbName = "wasmCsEdit";

/**
 * データベースを開き、その参照を取得します。
 * @returns {Dexie}
 * */
export function Open() {
    const db = new Dexie(dbName);
    db.version(1).stores({
        vardata: "&key"
    });
    return db;
}

/**
 * データベースに新しいデータを作成します。
 * @param {Dexie} db 対象のデータベース。
 * @param {string} _key 追加するデータのキー。
 * @param {string} _type 追加するデータ。
 * @param {string} __data Base64エンコードされたデータ。
 */
export function Create(db, _key, _type, __data) {
    const _data = _DecodeBase64(__data);
    db.vardata.add({
        key: _key,
        type: _type,
        data: _data,
    });
}

/**
 * データベースからデータを取得します。
 * @param {Dexie} db 対象のデータベース。
 * @param {string} _key 読み取るデータのキー。
 * @returns {string} Base64エンコードされたデータ。
 */
export function Read(db, _key) {
    return db.vardata.get(_key).then(result => _EncodeBase64(result.data));
}

/**
 * データベースのデータを更新します。
 * @param {Dexie} db 対象のデータベース。
 * @param {string} _key 更新するデータのキー。
 * @param {string} _type 更新するデータ。
 * @param {string} __data Base64エンコードされたデータ。
 */
export function Update(db, _key, _type, __data) {
    const _data = _DecodeBase64(__data);
    db.vardata.update(_key, {
        type: _type,
        data: _data,
    })
}

/**
 * データベースのデータを削除します。
 * @param {Dexie} db 対象のデータベース。
 * @param {string} _key 削除するデータのキー。
 */
export function Delete(db, _key) {
    db.vardata.delete(_key);
}

/**
 * データベースにデータを追加または更新します。
 * @param {Dexie} db 対象のデータベース。
 * @param {string} _key 追加または更新するデータのキー。
 * @param {string} _type 追加または更新するデータの型。
 * @param {string} __data Base64エンコードされたデータ。
 */
export function Put(db, _key, _type, __data) {
    const _data = _DecodeBase64(__data);
    db.vardata.put({
        key: _key,
        type: _type,
        data: _data,
    });
}

/**
 * Base64エンコードされた文字列をデコードします。
 * @param {string} base64 Base64エンコードされた文字列。
 * @returns {Uint8Array} デコードした結果。
 */
function _DecodeBase64(base64) {
    let binary = atob(base64);
    let len = binary.length;
    let bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return bytes;
}

/**
 * Base64文字列にエンコードします。
 * @param {any} array
 */
function _EncodeBase64(array) {
    return btoa(String.fromCharCode.apply(null, array));
}