(function () {
    var l = abp.localization.getResource('AbpTenantManagement');
    abp.ui.extensions.entityActions.get('tenantManagement.tenant').addContributor(
        function (actionList) {
            return actionList.addManyTail(
                [
                    {
                        text: l('Setup'),
                        visible: function (record) {
                            return !record.extraProperties.Initilized;
                        },
                        action: function (data) {
                            window.location.href = '/setup/install?tenant=' + data.record.id;
                        },
                    }
                ]
            );
        }
    );

    abp.ui.extensions.tableColumns.get('tenantManagement.tenant').addContributor(
        function (columnList) {
            columnList.addManyTail(
                [
                    {
                        title: l("Initilized"),
                        data: 'extraProperties.Initilized',
                        orderable: false,
                        render: function (data) {
                            return abp.libs.datatables.defaultRenderers['boolean'](data);
                        }
                    }
                ]
            );
        }
    );
})();
