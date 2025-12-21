/**
 * Скрипт для работы с буфером обмена
 */

document.addEventListener('DOMContentLoaded', function () {
    // Инициализация кнопок копирования
    initClipboardButtons();

    // Копирование текста из любого элемента с атрибутом data-copy
    document.addEventListener('click', function (e) {
        const copyButton = e.target.closest('[data-copy-target]');
        if (copyButton) {
            e.preventDefault();
            copyFromTarget(copyButton);
        }
    });
});

/**
 * Инициализация всех кнопок копирования
 */
function initClipboardButtons() {
    const copyButtons = document.querySelectorAll('.btn-copy, [onclick*="copyToClipboard"]');

    copyButtons.forEach(button => {
        const oldOnClick = button.getAttribute('onclick');
        if (oldOnClick && oldOnClick.includes('copyToClipboard')) {
            button.removeAttribute('onclick');
            button.addEventListener('click', function (e) {
                e.preventDefault();
                const targetId = this.getAttribute('onclick')?.match(/copyToClipboard\(\)/)
                    ? 'generatedLink'
                    : this.closest('.input-group')?.querySelector('input')?.id;

                if (targetId) {
                    copyTextFromInput(targetId, this);
                }
            });
        }

        button.setAttribute('data-bs-toggle', 'tooltip');
        button.setAttribute('title', 'Копировать в буфер');
    });
}

/**
 * Копирование текста из input элемента
 * @param {string} inputId - ID input элемента
 * @param {HTMLElement} button - Кнопка копирования
 */
function copyTextFromInput(inputId, button) {
    const input = document.getElementById(inputId);
    if (!input) {
        showToast('Элемент для копирования не найден', 'error');
        return;
    }

    input.select();
    input.setSelectionRange(0, 99999);

    navigator.clipboard.writeText(input.value).then(() => {
        showCopySuccess(button);
        showToast('Скопировано в буфер обмена', 'success');
    }).catch(err => {
        console.error('Ошибка копирования:', err);
        showToast('Не удалось скопировать', 'error');

        // Fallback для старых браузеров
        try {
            document.execCommand('copy');
            showCopySuccess(button);
            showToast('Скопировано в буфер обмена', 'success');
        } catch (fallbackErr) {
            showToast('Браузер не поддерживает копирование', 'error');
        }
    });
}

/**
 * Копирование из элемента с data-copy-target
 * @param {HTMLElement} button - Кнопка копирования
 */
function copyFromTarget(button) {
    const targetSelector = button.getAttribute('data-copy-target');
    const target = document.querySelector(targetSelector);

    if (!target) {
        showToast('Целевой элемент не найден', 'error');
        return;
    }

    const textToCopy = target.textContent || target.value || target.getAttribute('data-copy-text');

    if (!textToCopy) {
        showToast('Нечего копировать', 'error');
        return;
    }

    navigator.clipboard.writeText(textToCopy.trim()).then(() => {
        showCopySuccess(button);
        showToast('Скопировано в буфер обмена', 'success');
    }).catch(err => {
        console.error('Ошибка копирования:', err);
        showToast('Не удалось скопировать', 'error');
    });
}

/**
 * Показать успешное состояние копирования
 * @param {HTMLElement} button - Кнопка копирования
 */
function showCopySuccess(button) {
    const originalHtml = button.innerHTML;
    const originalClasses = button.className;

    const originalClassesWithoutSuccess = originalClasses.replace(/btn-success/g, '').trim();

    button.innerHTML = '<i class="bi bi-check-lg me-1"></i>Скопировано!';
    button.className = originalClassesWithoutSuccess + ' btn-success';
    button.disabled = true;

    if (button._tooltip) {
        button._tooltip.hide();
        button.setAttribute('title', 'Скопировано!');
        button._tooltip = new bootstrap.Tooltip(button);
        button._tooltip.show();
    }

    setTimeout(() => {
        button.innerHTML = originalHtml;
        button.className = originalClasses;
        button.disabled = false;

        if (button._tooltip) {
            button._tooltip.hide();
            button.setAttribute('title', 'Копировать в буфер');
            button._tooltip = new bootstrap.Tooltip(button);
        }
    }, 2000);
}

/**
 * Копировать текст напрямую
 * @param {string} text - Текст для копирования
 * @param {HTMLElement} [feedbackElement] - Элемент для отображения обратной связи
 */
function copyText(text, feedbackElement) {
    if (!navigator.clipboard && !document.execCommand) {
        showToast('Браузер не поддерживает копирование', 'error');
        return false;
    }

    const success = copyToClipboard(text);

    if (success && feedbackElement) {
        const originalText = feedbackElement.textContent;
        feedbackElement.textContent = 'Скопировано!';
        feedbackElement.classList.add('text-success');

        setTimeout(() => {
            feedbackElement.textContent = originalText;
            feedbackElement.classList.remove('text-success');
        }, 2000);
    }

    return success;
}

/**
 * Низкоуровневая функция копирования
 * @param {string} text - Текст для копирования
 */
function copyToClipboard(text) {
    if (navigator.clipboard && window.isSecureContext) {
        navigator.clipboard.writeText(text).then(() => {
            return true;
        }).catch(err => {
            console.error('Clipboard API error:', err);
            return fallbackCopy(text);
        });
        return true;
    }

    return fallbackCopy(text);
}

/**
 * Fallback метод для старых браузеров
 * @param {string} text - Текст для копирования
 */
function fallbackCopy(text) {
    const textArea = document.createElement('textarea');
    textArea.value = text;

    textArea.style.position = 'fixed';
    textArea.style.left = '-999999px';
    textArea.style.top = '-999999px';
    textArea.style.opacity = '0';
    textArea.style.pointerEvents = 'none';

    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();

    try {
        const successful = document.execCommand('copy');
        document.body.removeChild(textArea);
        return successful;
    } catch (err) {
        console.error('Fallback copy error:', err);
        document.body.removeChild(textArea);
        return false;
    }
}

// Экспорт функций для обратной совместимости
window.copyToClipboard = function () {
    const input = document.getElementById('generatedLink');
    if (input) {
        copyTextFromInput('generatedLink', event.target);
    }
};

// Глобальный экспорт
window.Clipboard = {
    copyText,
    copyFromTarget,
    copyTextFromInput
};