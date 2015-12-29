/*globals window,jQuery,Exp,waz*/
(function ($, global, Shell, Exp, undefined) {
    "use strict";

    var grid,
        statusIcons = {
            Registered: {
                text: "Registered",
                iconName: "complete"
            },
            Default: {
                iconName: "spinner"
            }
        };

    function onRowSelected(row) {
    }

    function loadTab(extension, renderArea, initData) {
        var localDataSet = {
            url: global.CmpWapExtensionAdminExtension.Controller.adminOssUrl,
            dataSetName: global.CmpWapExtensionAdminExtension.Controller.adminOssUrl
        },
            columns = [
                { name: "ID", field: "VmOsId", sortable: false },
                { name: "Name", field: "Name", filterable: false, sortable: false },
             //   { name: "DisplayName", field: "DisplayName", filterable: false, sortable: false },
                { name: "Description", field: "Description", filterable: false, sortable: false },
                //{ name: "Config", field: "Config", sortable: false },
                //{ name: "TagData", field: "TagData", sortable: false },
                { name: "IsActive", field: "IsActive", sortable: false }
            ];

        grid = renderArea.find(".grid-container")
            .wazObservableGrid("destroy")
            .wazObservableGrid({
                lastSelectedRow: null,
                data: localDataSet,
                keyField: "name",
                columns: columns,
                gridOptions: {
                    rowSelect: onRowSelected
                }
                //emptyListOptions: {
                //    extensionName: "CmpWapExtensionAdminExtension",
                //    templateName: "FileServersTabEmpty"
                //}
            });
    }

    function cleanUp() {
        if (grid) {
            grid.wazObservableGrid("destroy");
            grid = null;
        }
    }

    global.CmpWapExtensionAdminExtension = global.CmpWapExtensionAdminExtension || {};
    global.CmpWapExtensionAdminExtension.OssTab = {
        loadTab: loadTab,
        cleanUp: cleanUp
    };
})(jQuery, this); 