let google_auth_completed= null;
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

    const nonce = window.crypto.randomUUID();

    const params = new URLSearchParams({
        client_id: client_id,
        redirect_uri: redirect_url,
        response_type: 'id_token',
        scope: 'openid email profile',
        prompt: 'select_account',
        nonce: nonce
    }).toString();

    localStorage.setItem("gauth_nonce", nonce);

    const authUrl = `${"https://accounts.google.com/o/oauth2/v2/auth?"}${params}`;
    window.open(authUrl, 'windowName', 'height=650,width=500');



    if (google_auth_completed !== null) {
        clearInterval(google_auth_completed);
    }

    google_auth_completed = setInterval(() => {
        const gauth = localStorage.getItem("GAuthSession");

        if (gauth !== null || gauth !== undefined) {
            if (gauth.includes("Sign in successful") || gauth.includes("Sign up successful")) {
                clearInterval(google_auth_completed);
                window.location.href = `${window.location.origin}/log-in`;
            }
            else {
                clearInterval(google_auth_completed);
                localStorage.removeItem("GAuthSession");
            }
        }
    }, 100);
    
}


