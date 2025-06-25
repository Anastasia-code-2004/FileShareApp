import { register } from './api-client.js';
import { showMessage, clearMessage, validateForm } from './form-utils.js';

document.getElementById("register-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const form = e.target;
    const messageId = "register-message";

    clearMessage(messageId);

    if (!validateForm(form)) return;

    const password = document.getElementById("password");
    const passwordConfirm = document.getElementById("password-confirm");

    if (password.value !== passwordConfirm.value) {
        passwordConfirm.setCustomValidity("Пароли не совпадают");
        passwordConfirm.classList.add('is-invalid');
        form.classList.add('was-validated');
        return;
    } else {
        passwordConfirm.setCustomValidity("");
    }

    try {
        const response = await register(
            document.getElementById("email").value,
            password.value
        );

        if (response.ok) {
            window.location.href = "login.html";
        } else {
            const error = await response.json();
            showMessage(error.message || "Пользователь с таким email уже существует", "danger", messageId);
        }
    } catch (error) {
        showMessage("Серверная ошибка. Попробуйте позже.", "danger", messageId);
        console.error("Registration error:", error);
    }
});