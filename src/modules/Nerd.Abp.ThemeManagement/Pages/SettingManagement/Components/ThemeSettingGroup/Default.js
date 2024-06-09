(function ($) {

    $(function () {

        let l = abp.localization.getResource('ThemeManagement');

        $('#ResetToDefaults').click(function (e) {
            abp.message.confirm(l('AreYouSureToResetToDefault'))
                .then(function (confirmed) {
                    if (confirmed) {
                        nerd.abp.themeManagement.services.brandSetting.reset().then(function () {
                            abp.notify.success(l('ResetedToDefault'));

                            setTimeout(function () {
                                location.reload();
                            }, 500);
                        });
                    }
                });
        });

        $("#BrandSettingForm").on('submit', function (event) {
            event.preventDefault();

            if (!$(this).valid()) {
                return;
            }

            let form = $(this).serializeFormToObject();
            nerd.abp.themeManagement.services.brandSetting.update(form).then(function (result) {
                abp.notify.success(l('SuccessfullySaved'));
            });
        });        
    });

})(jQuery);
