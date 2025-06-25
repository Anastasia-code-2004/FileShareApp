import { login } from './api-client.js';
import { showMessage, clearMessage, validateForm } from './form-utils.js';

document.getElementById("login-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const messageId = "login-message";
    const loginBtn = document.getElementById("login-btn"); 

    clearMessage(messageId);

    if (!validateForm(e.target)) return;

    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    loginBtn.disabled = true;
    loginBtn.innerHTML = `
        <span class="spinner-border spinner-border-sm" aria-hidden="true"></span>
        Вход...
    `;

    try {
        const res = await login(email, password);

        if (res.ok) {
            const data = await res.json();
            localStorage.setItem("token", data.token);
            window.location.href = "upload.html";
        } else {
            const error = await res.json();
            showMessage(error.message || "Неверные учетные данные", "danger", messageId);
        }
    } catch (error) {
        showMessage("Ошибка соединения", "danger", messageId);
        console.error("Login error:", error);
    } finally {
        loginBtn.disabled = false;
        loginBtn.textContent = "Войти";
    }
});