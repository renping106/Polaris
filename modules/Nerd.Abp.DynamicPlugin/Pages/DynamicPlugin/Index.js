(function () {
    var l = abp.localization.getResource('DynamicPlugin');
    var _pluginAppService = nerd.abp.dynamicPlugin.services.plugin;
    var _dataTable = null;

    abp.ui.extensions.entityActions.get('plugin').addContributor(
        function (actionList) {
            return actionList.addManyTail(
                [
                    {
                        text: l('Enable'),
                        visible: function (data) {
                            return (!data.isEnabled && abp.auth.isGranted('DynamicPlugin.Edit'));
                        },
                        confirmMessage: function (data) {
                            return l(
                                'PlugInEnableConfirm',
                                data.record.name
                            );
                        },
                        action: function (data) {
                            _pluginAppService
                                .enable(data.record.name)
                                .then(function () {
                                    _dataTable.ajax.reloadEx();
                                    abp.notify.success(l('SuccessfullyEnabled'));
                                });
                        },
                    },
                    {
                        text: l('Disable'),
                        visible: function (data) {
                            return (data.isEnabled && abp.auth.isGranted('DynamicPlugin.Edit'));
                        },
                        confirmMessage: function (data) {
                            return l(
                                'PlugInDisableConfirm',
                                data.record.name
                            );
                        },
                        action: function (data) {
                            _pluginAppService
                                .disable(data.record.name)
                                .then(function () {
                                    _dataTable.ajax.reloadEx();
                                    abp.notify.success(l('SuccessfullyDisabled'));
                                });
                        },
                    }
                ]
            );
        }
    );

    abp.ui.extensions.tableColumns.get('plugin').addContributor(
        function (columnList) {
            columnList.addManyTail(
                [
                    {
                        title: l("Actions"),
                        rowAction: {
                            items: abp.ui.extensions.entityActions.get('plugin').actions.toArray()
                        }
                    },
                    {
                        title: l("Name"),
                        data: 'name',
                    },
                    {
                        title: l("Description"),
                        data: 'description',
                    },
                    {
                        title: l("Version"),
                        data: 'version',
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
                        title: l("Enabled"),
                        data: 'isEnabled',
                        orderData: [3],
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
        var _$wrapper = $('#PlugInList');

        _dataTable = _$wrapper.find('table').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                order: [[1, 'asc']],
                processing: true,
                paging: true,
                scrollX: true,
                serverSide: false,
                ajax: abp.libs.datatables.createAjax(_pluginAppService.getList), 
                columnDefs: abp.ui.extensions.tableColumns.get('plugin').columns.toArray(),
            })
        );
    });
})();
