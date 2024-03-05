function openRegisterModal() {
    $('#loginModal').modal('hide'); // Закрыть текущее модальное окно авторизации
    $('#registerModal').modal('show'); // Открыть модальное окно регистрации
}

function openStep2() {
    $('#registerModalBodyStep1').addClass('d-none'); // Скрыть содержимое первого шага
    $('#registerModalBodyStep2').removeClass('d-none'); // Показать содержимое второго шага
}

function finishRegistration() {
    
}

$('#registerModal').on('show.bs.modal', function (e) {
    $('#registerModalBodyStep1').removeClass('d-none'); // Показать первый шаг
    $('#registerModalBodyStep2').addClass('d-none'); // Скрыть второй шаг
});

$(document).ready(function () {
    $('#inn').on('keyup', function () {
        // Получаем количество символов в поле ввода ИНН
        var innLength = $(this).val().trim().length;

        // Если количество символов равно 10, удаляем атрибут disabled у кнопки Продолжить, иначе добавляем его
        if (innLength === 10) {
            $('#continueButton').prop('disabled', false);
        } else {
            $('#continueButton').prop('disabled', true);
        }
    });
});

$(document).ready(function () {
    // Функция для проверки совпадения пароля и его подтверждения
    function passwordsMatch() {
        var password = $('#password').val();
        var confirmPassword = $('#confirmPassword').val();
        return password === confirmPassword;
    }

    // Функция для проверки заполненности полей
    function fieldsNotEmpty() {
        var fio = $('#FIO').val();
        var email = $('#E-mail').val();
        var password = $('#password').val();
        var confirmPassword = $('#confirmPassword').val();
        return fio !== '' && email !== '' && password !== '' && confirmPassword !== '';
    }

    // Функция для обновления состояния кнопки "Завершить регистрацию"
    function updateButtonState() {
        var buttonDisabled = !(passwordsMatch() && fieldsNotEmpty());
        $('#finishRegistrationButton').prop('disabled', buttonDisabled);
    }

    // Обновляем состояние кнопки при изменении значений в полях ввода
    $('#FIO, #E-mail, #password, #confirmPassword').on('input', function () {
        updateButtonState();
    });

    // Обновляем состояние кнопки при загрузке страницы
    updateButtonState();
});

$(document).ready(function () {
    $('.hamburger-button').click(function () {
        $('.hamburger-menu-content').toggleClass('menu-open');
    });

    $('.close-menu-btn').click(function () {
        $('.hamburger-menu-content').removeClass('menu-open');
    });
});







