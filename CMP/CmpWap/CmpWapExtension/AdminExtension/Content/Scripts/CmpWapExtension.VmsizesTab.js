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
            url: global.CmpWapExtensionAdminExtension.Controller.adminVmSizesUrl,
            dataSetName: global.CmpWapExtensionAdminExtension.Controller.adminVmSizesUrl
        },
            columns = [
                { name: "ID", field: "VmSizeId", sortable: false },
                { name: "Name", field: "Name", filterable: false, sortable: false },
                { name: "Description", field: "Description", filterable: false, sortable: false },
                { name: "Cpu Core Count", field: "Cores", sortable: false },
                { name: "Ram (MB)", field: "Memory", sortable: false },
                { name: "Data Disk Count", field: "MaxDataDiskCount", sortable: false },
                { name: "Is Active", field: "IsActive", sortable: false }
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
    global.CmpWapExtensionAdminExtension.VmSizesTab = {
        loadTab: loadTab,
        cleanUp: cleanUp
    };
})(jQuery, this); 