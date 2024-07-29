export async function Get_Cache(option, key) {
    let return_value = "";
    switch (option) {
        case "auth":
            return_value = localStorage.getItem(key);
            break;
    }
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