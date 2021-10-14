import Dexie from "./../lib/dexie/dexie.js";

const dbName = "wasmCsEdit";

let temp;
export function BeginRead(db, _key) {
    db.vardata.get(_key).then(result => temp = _EncodeBase64(result.data));
}

export function EndRead() {
    return temp;
}

export function Open() {
    const db = new Dexie(dbName);
    db.version(1).stores({
        vardata: "&key"
    });
    return db;
}

export function Create(db, _key, _type, __data) {
    const _data = _DecodeBase64(__data);
    db.vardata.add({
        key: _key,
        type: _type,
        data: _data,
    });
}

export function Read(db, _key) {
    return db.vardata.get(_key).then(result => _EncodeBase64(result.data));
}

export function Update(db, _key, _type, __data) {
    const _data = _DecodeBase64(__data);
    db.vardata.update(_key, {
        type: _type,
        data: _data,
    })
}

export function Delete(db, _key) {
    db.vardata.delete(_key);
}

export function Put(db, _key, _type, __data) {
    const _data = _DecodeBase64(__data);
    db.vardata.put({
        key: _key,
        type: _type,
        data: _data,
    });
}

function _DecodeBase64(base64) {
    let binary = atob(base64);
    let len = binary.length;
    let bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return bytes;
}

function _EncodeBase64(array) {
    return btoa(String.fromCharCode.apply(null, array));
}