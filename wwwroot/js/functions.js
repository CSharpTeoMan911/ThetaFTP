let google_sign_in_callback = null;
let client_id = null;
const redirect_url = `${window.location.origin}/gauth-complete`;


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

export function GoogleSignInApiProcessing(clientId) {
    client_id = clientId;
}

export async function GoogleSignIn() {
    const params = new URLSearchParams({
        client_id: client_id,
        redirect_uri: redirect_url,
        response_type: 'token',
        scope: 'email profile',
        prompt: 'select_account'
    }).toString();

    const authUrl = `${"https://accounts.google.com/o/oauth2/v2/auth?"}${params}`;
    window.open(authUrl, 'windowName', 'height=650,width=500');

    window.addEventListener("message", (e) => {
        if (e?.origin?.includes(redirect_url) === true) {
            if (e?.data["result"] === "Authentication Successful") {
                console.log(`Auth result: ${e?.data['key']}`);
                //if () { }
            }
        }
    });
}

export async function SendGoogleAuthResult() {
    window.opener.postMessage()
}


