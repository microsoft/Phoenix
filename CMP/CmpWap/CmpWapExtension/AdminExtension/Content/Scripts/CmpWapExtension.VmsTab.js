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

 
    function loadTab(extension, renderArea, initData) {
     var columns = [
                { name: "Vm Name", field: "Name", sortable: false },
                { name: "App Name", field: "VmAppName", sortable: false },
                { name: "Domain", field: "VmDomain", sortable: false },
                { name: "Size", field: "VmSize", sortable: false },
                { name: "Region", field: "VmRegion", sortable: false },
                { name: "Status Code", field: "StatusCode", sortable: false },
                { name: "Status Message", field: "StatusMessage", sortable: false },
                { name: "Address", field: "AddressFromVm", sortable: false }
              
     ];

     global.Shell.UI.Spinner.show();
     var promise = global.CmpWapExtensionAdminExtension.Controller.getVmsDataSet();

     promise.done(function(value)
     {

        grid = renderArea.find(".grid-container")
            .wazObservableGrid("destroy")
            .wazObservableGrid({
                lastSelectedRow: null,
                data: value.data,
                keyField: "Name",
                columns: columns
                //emptyListOptions: {
                //    extensionName: "CmpWapExtensionAdminExtension",
                //    templateName: "FileServersTabEmpty"
                //}
            });
     })
         .fail(function (val) { global.Shell.UI.Spinner.hide(); })
        .always(function () {
            global.Shell.UI.Spinner.hide();
        });
    }

    function cleanUp() {
        if (grid) {
            grid.wazObservableGrid("destroy");
            grid = null;
        }
    }

    global.CmpWapExtensionAdminExtension = global.CmpWapExtensionAdminExtension || {};
   
    global.CmpWapExtensionAdminExtension.VmsTab = {
        loadTab: loadTab,
        cleanUp: cleanUp
    };
})(jQuery, this);