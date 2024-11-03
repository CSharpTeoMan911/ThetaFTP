export async function Get_Cache(option, key) {
    let return_value = "";
    switch (option) {
        case "auth":
            return_value = localStorage.getItem(key);
            break;
    }

    return return_value;
}

export async function Insert_Cache(option, key, value) {
    switch (option) {
        case "auth":
            localStorage.setItem(key, value);
            break;
    }
}

export async function Delete_Cache(option, key) {
    switch (option) {
        case "auth":
            localStorage.removeItem(key);
            break;
    }
}

export async function BrowserManagedFileDownload(url, filename) {
    let anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = filename ?? "File";
    anchor.click();
    anchor.remove();
}