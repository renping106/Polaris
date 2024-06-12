$(function () {
    toggleConnectionString();
    toggleDbSelect();
    // Show hide the connection string when a provider is selected
    $("#Config_DatabaseProvider").change(function () {
        toggleConnectionString();
    });

    $("#Config_UseHostSetting").change(function () {
        toggleDbSelect();
    });

    $('#Config_Password').popover({
        trigger: 'focus'
    });

    let togglePassword = document.querySelector('#togglePassword');
    togglePassword.addEventListener('click', function (e) {
        togglePasswordVisibility(document.querySelector('#Config_Password'), document.querySelector('#togglePassword'))
    });

    let togglePasswordConfirmation = document.querySelector('#togglePasswordConfirmation');
    togglePasswordConfirmation.addEventListener('click', function (e) {
        togglePasswordVisibility(document.querySelector('#Config_PasswordConfirmation'), document.querySelector('#togglePasswordConfirmation'))
    });

    $('form').submit(function () {
        let errors = $('.input-validation-error');
        var strength = $('.bg-success');
        if (!(errors && errors.length > 0) && strength.length == 1) {
            $('.spinner-container').css('display', 'flex');
        }
    });
});

function toggleConnectionString() {
    $("#Config_DatabaseProvider option:selected").each(function () {
        $(this).data("connection-string").toLowerCase() === "true"
            ? $(".connectionString").show()
            : $(".connectionString").hide();

        $("#Config_ConnectionString").val($(this).data("connection-string-value"));
        $("#connectionStringHint").text($(this).data("connection-string-sample"));
    });
}

function toggleDbSelect() {
    $("#Config_UseHostSetting").is(':checked')
        ? $(".connectionString").hide()
        : $(".connectionString").show();
    $("#Config_UseHostSetting").is(':checked')
        ? $(".dbselect").hide()
        : $(".dbselect").show();
}

function togglePasswordVisibility(passwordCtl, togglePasswordCtl) {
    // toggle the type attribute
    let type = passwordCtl.getAttribute('type') === 'password' ? 'text' : 'password';
    passwordCtl.setAttribute('type', type);

    // toggle the eye slash icon
    let icon = togglePasswordCtl.getElementsByClassName('icon')[0];
    if (icon.getAttribute('data-icon')) { // if the icon is rendered as a svg
        type === 'password' ? icon.setAttribute('data-icon', 'eye') : icon.setAttribute('data-icon', 'eye-slash');
    }
    else { // if the icon is still a <i> element
        type === 'password' ? icon.classList.remove('fa-eye-slash') : icon.classList.remove('fa-eye');
        type === 'password' ? icon.classList.add('fa-eye') : icon.classList.add('fa-eye-slash');
    }
}
