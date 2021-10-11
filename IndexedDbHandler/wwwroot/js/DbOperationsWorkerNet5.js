let module;
let db;

function Load() {
    return import("./DbOperationsNet5.js").then(mod => module = mod);
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

function Delete(_key) {
    module.Delete(db, _key);
}

function Put(_key, _type, _data) {
    module.Put(db, _key, _type, _data);
}