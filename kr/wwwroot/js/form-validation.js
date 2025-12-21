/**
 * Валидация форм
 */

document.addEventListener('DOMContentLoaded', function () {
    // Кастомная валидация Bootstrap
    const forms = document.querySelectorAll('.needs-validation');

    Array.from(forms).forEach(form => {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }

            form.classList.add('was-validated');
        }, false);
    });

    // Кастомные валидаторы
    initCustomValidators();
});

function initCustomValidators() {
    // Валидация телефона
    const phoneInputs = document.querySelectorAll('input[type="tel"]');
    phoneInputs.forEach(input => {
        input.addEventListener('blur', function () {
            validatePhone(this);
        });
    });

    // Валидация email
    const emailInputs = document.querySelectorAll('input[type="email"]');
    emailInputs.forEach(input => {
        input.addEventListener('blur', function () {
            validateEmail(this);
        });
    });

    // Валидация числа в диапазоне
    const numberInputs = document.querySelectorAll('input[type="number"][min][max]');
    numberInputs.forEach(input => {
        input.addEventListener('change', function () {
            validateNumberRange(this);
        });
    });
}

function validatePhone(input) {
    const value = input.value.trim();
    const phoneRegex = /^\+?7\s?\(?\d{3}\)?\s?\d{3}[-]?\d{2}[-]?\d{2}$/;

    if (value && !phoneRegex.test(value)) {
        setInvalid(input, 'Введите корректный номер телефона');
        return false;
    }

    setValid(input);
    return true;
}

function validateEmail(input) {
    const value = input.value.trim();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (value && !emailRegex.test(value)) {
        setInvalid(input, 'Введите корректный email адрес');
        return false;
    }

    setValid(input);
    return true;
}

function validateNumberRange(input) {
    const value = parseFloat(input.value);
    const min = parseFloat(input.min);
    const max = parseFloat(input.max);

    if (isNaN(value)) {
        setInvalid(input, 'Введите число');
        return false;
    }

    if (value < min || value > max) {
        setInvalid(input, `Значение должно быть от ${min} до ${max}`);
        return false;
    }

    setValid(input);
    return true;
}

function setInvalid(input, message) {
    input.classList.remove('is-valid');
    input.classList.add('is-invalid');

    let feedback = input.nextElementSibling;
    if (!feedback || !feedback.classList.contains('invalid-feedback')) {
        feedback = document.createElement('div');
        feedback.className = 'invalid-feedback';
        input.parentNode.insertBefore(feedback, input.nextSibling);
    }

    feedback.textContent = message;
}

function setValid(input) {
    input.classList.remove('is-invalid');
    input.classList.add('is-valid');

    const feedback = input.nextElementSibling;
    if (feedback && feedback.classList.contains('invalid-feedback')) {
        feedback.textContent = '';
    }
}

// Экспорт функций
window.FormValidation = {
    validatePhone,
    validateEmail,
    validateNumberRange
};