/*globals window,jQuery,Exp,waz*/
(function ($, global,undefined) {
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

  
    function loadTab(extension, renderArea, initData) {
        var localDataSet = {
            url: global.CmpWapExtensionAdminExtension.Controller.adminRegionsUrl,
            dataSetName: global.CmpWapExtensionAdminExtension.Controller.adminRegionsUrl
        },
            Columns = [
                { name: "ID", field: "AzureRegionId", sortable: false },
                { name: "Name", field: "Name", filterable: false, sortable: false },
                { name: "Description", field: "Description", filterable: false, sortable: false },
                { name: "IsActive", field: "IsActive", sortable: false }
            ];

        grid = renderArea.find(".grid-container")
            .wazObservableGrid("destroy")
            .wazObservableGrid({
                lastSelectedRow: null,
                data: localDataSet,
                keyField: "Name",
                columns: Columns
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
    global.CmpWapExtensionAdminExtension.RegionsTab = {
        loadTab: loadTab,
        cleanUp: cleanUp
    };


})(jQuery, this); 