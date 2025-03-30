let google_sign_in_callback = null;

export function Get_Cache(option, key) {
    let return_value = "";
    switch (option) {
        case "auth":
            return_value = localStorage.getItem(key);
            break;
    }

    return return_value;
}

export function Insert_Cache(option, key, value) {
    switch (option) {
        case "auth":
            localStorage.setItem(key, value);
            break;
    }
}

export function Delete_Cache(option, key) {
    switch (option) {
        case "auth":
            localStorage.removeItem(key);
            break;
    }
}

export function BrowserManagedFileDownload(url, filename) {
    let anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = filename ?? "File";
    anchor.click();
    anchor.remove();
}

export function GoogleSignInApiProcessing(dotNetModule) {
    google_sign_in_callback = dotNetModule;
    console.log(dotNetModule);
}

export async function GoogleSignIn(google_jwt) {
    console.log("!!! Called !!!");
    let res = await google_sign_in_callback.invokeMethodAsync("ProcessJwtToken", JSON.stringify(google_jwt));
    console.log(res);
}


