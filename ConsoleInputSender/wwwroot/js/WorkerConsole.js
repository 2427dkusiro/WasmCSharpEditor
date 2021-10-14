export function GetInput() {
    let url = new URL("/hoge.html");
    let xhr = new XMLHttpRequest();
    xhr.open("GET", url, false);
    xhr.send(null);
}