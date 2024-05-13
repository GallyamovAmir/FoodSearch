// Функция для открытия модального окна регистрации
function openRegisterModal() {
    $('#loginModal').modal('hide'); // Закрыть текущее модальное окно авторизации
    $('#registerModal').modal('show'); // Открыть модальное окно регистрации
}

// Функция для перехода ко второму шагу регистрации
function openStep2() {
    $('#registerModalBodyStep1').addClass('d-none'); // Скрыть содержимое первого шага
    $('#registerModalBodyStep2').removeClass('d-none'); // Показать содержимое второго шага
}

// Функция для завершения регистрации
function finishRegistration() {
    // Ваша логика завершения регистрации здесь
}

// Обработчик события при открытии модального окна регистрации
$('#registerModal').on('show.bs.modal', function (e) {
    $('#registerModalBodyStep1').removeClass('d-none'); // Показать первый шаг
    $('#registerModalBodyStep2').addClass('d-none'); // Скрыть второй шаг
});

// Обработчик события при вводе ОГРН
$('#OGRN').on('keyup', function () {
    // Получаем количество символов в поле ввода ОГРН
    var OGRNLength = $(this).val().trim().length;

    // Если количество символов равно 13, удаляем атрибут disabled у кнопки Продолжить, иначе добавляем его
    if (OGRNLength === 13) {
        $('#continueButton').prop('disabled', false);
    } else {
        $('#continueButton').prop('disabled', true);
    }
});

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

// Обработчик клика по кнопке "Гамбургер"
$('.hamburger-button').on('click', function () {
    $('.hamburger-menu-content').toggleClass('open'); // Переключаем класс open
});

// Обработчик клика по кнопке закрытия меню
$('.close-menu-btn').on('click', function () {
    $('.hamburger-menu-content').removeClass('open'); // Закрываем меню при нажатии на кнопку закрытия
});


// Обработчик события при открытии модального окна
$('#acceptAddition').on('show.bs.modal', function (event) {
    // Получаем кнопку "Добавить", по которой было совершено нажатие
    var button = $(event.relatedTarget);

    // Извлекаем данные о товаре из атрибутов кнопки
    var productName = button.data('product-name');
    var productPrice = button.data('product-price');
    var productImage = button.data('product-image');
    var productId = button.data('product-id');

    // Отображаем информацию о товаре в модальном окне
    $('#productTitle').text(productName);
    $('#productName').val(productName);
    $('#productPrice').val(productPrice);
    $('#productId').val(productId);
    $('#productImage').attr('src', productImage);
});

// Добавим обработчик для кнопок "Добавить", чтобы получить информацию о товаре
$('#cartbutton').click(function () {
    var productName = $(this).data('product-name');
    var productPrice = $(this).data('product-price');
    var productImage = $(this).data('product-image');
    var productId = $(this).data('product-id'); // Исправлено здесь

    // Отображаем информацию о товаре в модальном окне
    $('#productTitle').text(productName);
    $('#productName').val(productName);
    $('#productPrice').val(productPrice);
    $('#productId').val(productId);
    $('#productImage').attr('src', productImage);

    // Открываем модальное окно
    $('#acceptAddition').modal('show');
});


