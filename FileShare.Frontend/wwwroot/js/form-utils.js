export function clearMessage(elementId) {
    const el = document.getElementById(elementId);
    if (el) {
        el.textContent = '';
        el.className = 'alert d-none';
    }
}

export function clearValidation(form) {
    form.classList.remove('was-validated');
    form.querySelectorAll('.is-invalid').forEach(el => {
        el.classList.remove('is-invalid');
        el.setCustomValidity('');
    });
}

export function validateForm(form) {
    clearValidation(form);

    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return false;
    }
    return true;
}

export function showMessage(message, type = "info", elementId = "message") {
    const messageBox = document.getElementById(elementId);
    if (!messageBox) return;

    messageBox.textContent = message;
    messageBox.className = `alert alert-${type}`;
}