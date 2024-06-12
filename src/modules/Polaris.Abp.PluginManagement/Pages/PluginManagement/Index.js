(function () {
    var l = abp.localization.getResource('PluginManagement');
    var _pluginAppService = polaris.abp.pluginManagement.services.plugin;

    var _uploadModal = new abp.ModalManager(
        abp.appPath + 'PluginManagement/Upload'
    );

    var _dataTable = null;

    abp.ui.extensions.entityActions.get('plugin').addContributor(
        function (actionList) {
            return actionList.addManyTail(
                [
                    {
                        text: l('Enable'),
                        visible: function (data) {
                            return (!data.isEnabled && abp.auth.isGranted('PluginManagement.Edit'));
                        },
                        confirmMessage: function (data) {
                            return l(
                                'PlugInEnableConfirm',
                                data.record.name
                            );
                        },
                        action: function (data) {
                            abp.ui.setBusy('#PlugInList');
                            _pluginAppService
                                .enable(data.record.name)
                                .then(function (result) {
                                    if (result.success) {
                                        abp.ui.clearBusy();
                                        _dataTable.ajax.reloadEx();
                                        abp.notify.success(l('SuccessfullyEnabled'));
                                    }
                                    else {
                                        abp.ui.clearBusy();
                                        abp.notify.error(data.message, l('FailedToEnable'));
                                    }
                                }).catch(function () {
                                    abp.ui.clearBusy();
                                });
                        },
                    },
                    {
                        text: l('Disable'),
                        visible: function (data) {
                            return (data.isEnabled && abp.auth.isGranted('PluginManagement.Edit'));
                        },
                        confirmMessage: function (data) {
                            return l(
                                'PlugInDisableConfirm',
                                data.record.name
                            );
                        },
                        action: function (data) {
                            abp.ui.setBusy('#PlugInList');
                            _pluginAppService
                                .disable(data.record.name)
                                .then(function () {
                                    abp.ui.clearBusy();
                                    _dataTable.ajax.reloadEx();
                                    abp.notify.success(l('SuccessfullyDisabled'));
                                });
                        },
                    },
                    {
                        text: l('Remove'),
                        visible: function (data) {
                            return (!data.isEnabled && abp.auth.isGranted('PluginManagement.Edit'));
                        },
                        confirmMessage: function (data) {
                            return l(
                                'PlugInRemoveConfirm',
                                data.record.name
                            );
                        },
                        action: function (data) {
                            abp.ui.setBusy('#PlugInList');
                            _pluginAppService
                                .remove(data.record.name)
                                .then(function () {
                                    abp.ui.clearBusy();
                                    _dataTable.ajax.reloadEx();
                                    abp.notify.success(l('SuccessfullyRemoved'));
                                });
                        },
                    },
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
                        orderData: [4],
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

        _uploadModal.onResult(function () {
            _dataTable.ajax.reloadEx();
        });

        $('#AbpContentToolbar button[name=Upload]').click(function (e) {
            e.preventDefault();
            _uploadModal.open();
        });
    });
})();
