import Dexie from "./../lib/dexie/dexie.js";

const dbName = "wasmCsEdit";

export function Open() {
    const db = new Dexie(dbName);
    db.version(1).stores({
        vardata: "&key,type,data"
    });
    return db;
}

export function Create(db, _key, _type, _data) {
    db.vardata.add({
        key: _key,
        type: _type,
        data: _data,
    });
}

export function Read(db, _key) {
    return db.vardata.get(_key).then(result => result.data);
}

export function Update(db, _key, _type, _data) {
    db.vardata.update(_key, {
        type: _type,
        data: _data,
    })
}

export function Delete(db, _key, _type, _data) {

}

export function Put(db, _key, _type, _data) {
    db.vardata.put({
        key: _key,
        type: _type,
        data: _data,
    });
}