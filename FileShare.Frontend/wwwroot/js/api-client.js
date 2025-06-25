import { config } from "./config.js"; 

async function postJson(path, body) {
    console.log(config.apiBaseUrl);
    const res = await fetch(`${config.apiBaseUrl}${path}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body),
    });
    return res;
}

export async function register(email, password) {
    return await postJson("/api/auth/register", { email, password });
}

export async function login(email, password) {
    return await postJson("/api/auth/login", { email, password });
}

export async function uploadFile(file, token) {
    const formData = new FormData();
    formData.append("file", file);

    const res = await fetch(`${config.apiBaseUrl}/api/files/upload`, {
        method: "POST",
        headers: {
            "Authorization": `Bearer ${token}`,
        },
        body: formData,
    });

    return res;
}