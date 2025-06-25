export const MAX_FILE_SIZE = 100 * 1024 * 1024; 

export const DANGEROUS_EXTENSIONS = [
    '.exe', '.bat', '.cmd', '.sh',
    '.ps1', '.php', '.js', '.jar'
];

export function validateFile(file) {
    if (!file) {
        return { isValid: false, message: 'Пожалуйста, выберите файл' };
    }

    if (file.size === 0) {
        return { isValid: false, message: 'Файл не может быть пустым' };
    }

    if (file.size > MAX_FILE_SIZE) {
        return {
            isValid: false,
            message: `Файл не должен превышать ${MAX_FILE_SIZE / (1024 * 1024)} МБ`
        };
    }

    const fileExt = '.' + file.name.split('.').pop().toLowerCase();
    if (DANGEROUS_EXTENSIONS.includes(fileExt)) {
        return {
            isValid: false,
            message: 'Файлы с этим расширением запрещены'
        };
    }

    return { isValid: true };
}