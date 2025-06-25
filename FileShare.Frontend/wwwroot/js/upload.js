import { uploadFile } from "./api-client.js";
import { validateFile } from "./file-utils.js";

document.getElementById('upload-form').addEventListener('submit', async function (e) {
    e.preventDefault();

    const uploadBtn = document.getElementById('upload-button');
    const fileInput = document.getElementById('file-input');
    const resultDiv = document.getElementById('result');
    const file = fileInput.files[0];

    uploadBtn.disabled = true;
    uploadBtn.innerHTML = `
        <span class="spinner-border spinner-border-sm" aria-hidden="true"></span>
        Загрузка...`;

    clearResult(resultDiv);

    try {
        const validation = validateFile(file);
        if (!validation.isValid) {
            showError(resultDiv, validation.message);
            return;
        }

        const token = localStorage.getItem("token");
        if (!token) {
            showError(resultDiv, 'Вы не авторизованы');
            return;
        }

        const response = await uploadFile(file, token);
        await handleUploadResponse(response, resultDiv);

    } catch (err) {
        showError(resultDiv, 'Ошибка при загрузке файла');
        console.error('Upload error:', err);
    } finally {
        uploadBtn.disabled = false;
        uploadBtn.innerHTML = '<i class="bi bi-upload"></i> Загрузить';
    }
});

async function handleUploadResponse(response, container) {
    if (!response.ok) {
        const contentType = response.headers.get("content-type");
        if (contentType && contentType.includes("application/json")) {
            const json = await response.json();
            message = json.error || json.message || JSON.stringify(json);
        } else {
            message = await response.text();
        }
        showError(container, `Ошибка: ${message}`);
        return;
    }

    const data = await response.json();
    showSuccess(container, data.link);
}

function clearResult(container) {
    container.innerHTML = '';
    container.className = '';
}

function showError(container, message) {
    container.innerHTML = `
        <div class="alert alert-warning mb-0">
            ${message}
        </div>
    `;
    container.classList.add('mt-3');
}
function showSuccess(container, downloadLink) {
    container.innerHTML = `
        <div class="alert alert-success d-flex justify-content-between align-items-center mb-0">
            <a href="${downloadLink}" target="_blank" class="link-success text-decoration-none">
                <i class="bi bi-download"></i> Скачать файл
            </a>
            <button class="btn btn-sm btn-outline-secondary ms-auto copy-button">
                <i class="bi bi-clipboard"></i> Скопировать
            </button>
        </div>
    `;
    container.classList.add('mt-3');

    const downloadAnchor = container.querySelector('a');
    const copyButton = container.querySelector('.copy-button');

    downloadAnchor.addEventListener('click', () => {
        setTimeout(() => {
            container.innerHTML = `
                <div class="d-flex align-items-center">
                    <i class="bi bi-check-circle-fill text-success me-2"></i>
                    <span>Файл скачан и удалён с сервера</span>
                </div>
            `;
        }, 500);
    });

    copyButton.addEventListener('click', () => {
        navigator.clipboard.writeText(downloadLink).then(() => {
            copyButton.innerHTML = `<i class="bi bi-check2"></i> Скопировано!`;
            setTimeout(() => {
                copyButton.innerHTML = `<i class="bi bi-clipboard"></i> Скопировать`;
            }, 2000);
        });
    });
}