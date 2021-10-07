let module;
let db;

function Load() {
    return import("./DbOperations.js").then(mod => module = mod);
}

function Open() {
    db = module.Open();
}

function Create(_key, _type, _data) {
    module.Create(db, _key, _type, _data);
}

function Read(_key) {
    return module.Read(db, _key);
}

function Update(_key, _type, _data) {
    module.Update(db, _key, _type, _data);
}

function Delete(_key, _type, _data) {
    module.Delete(db, _key, _type, _data);
}

function Put(_key, _type, _data) {
    db.vardata.put({
        key: _key,
        type: _type,
        data: _data,
    });
}