(function () {
    var l = abp.localization.getResource('ThemeManagement');
    var _themeAppService = nerd.abp.themeManagement.services.theme;

    var _dataTable = null;

    abp.ui.extensions.entityActions.get('theme').addContributor(
        function (actionList) {
            return actionList.addManyTail(
                [
                    {
                        text: l('Enable'),
                        visible: function (data) {
                            return abp.auth.isGranted('ThemeManagement.Edit');
                        },
                        enabled: function (data) {
                            return !data.record.isEnabled;
                        },
                        confirmMessage: function (data) {
                            return l(
                                'PlugInEnableConfirm',
                                data.record.name
                            );
                        },
                        action: function (data) {
                            abp.ui.setBusy('#ThemeList');
                            _themeAppService
                                .enable(data.record.typeName)
                                .then(function (data) {
                                    abp.ui.clearBusy();
                                    if (data) {
                                        _dataTable.ajax.reloadEx();
                                        abp.notify.success(l('SuccessfullyEnabled'));
                                    }
                                    else {
                                        abp.notify.error(l('FailedToEnable'));
                                    }
                                });
                        },
                    }
                ]
            );
        }
    );

    abp.ui.extensions.tableColumns.get('theme').addContributor(
        function (columnList) {
            columnList.addManyTail(
                [
                    {
                        title: l("Actions"),
                        rowAction: {
                            items: abp.ui.extensions.entityActions.get('theme').actions.toArray()
                        }
                    },
                    {
                        title: l("Name"),
                        data: 'name',
                    },
                    {
                        title: l("Type Name"),
                        data: 'typeName',
                    },
                    {
                        title: "EnabledValue",
                        data: 'isEnabled',
                        visible: false,
                        render: function (isEnabled) {
                            return isEnabled ? 'enabled' : 'disabled';
                        },
                    },
                    {
                        title: l("Current"),
                        data: 'isEnabled',
                        orderData: [2],
                        render: function (isEnabled) {
                            return '<input type="checkbox" disabled ' + (isEnabled ? 'checked' : '') + ' />';
                        },
                    }
                ]
            );
        },
        0 //adds as the first contributor
    );

    $(function () {
        var _$wrapper = $('#ThemeList');
        _dataTable = _$wrapper.find('table').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                order: [[1, 'asc']],
                processing: true,
                paging: true,
                scrollX: true,
                serverSide: false,
                ajax: abp.libs.datatables.createAjax(_themeAppService.getThemes),
                columnDefs: abp.ui.extensions.tableColumns.get('theme').columns.toArray(),
            })
        );
    });
})();
