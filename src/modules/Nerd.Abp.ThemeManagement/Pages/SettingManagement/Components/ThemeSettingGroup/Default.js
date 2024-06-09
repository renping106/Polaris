(function ($) {

    $(function () {
        let l = abp.localization.getResource('ThemeManagement');
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
