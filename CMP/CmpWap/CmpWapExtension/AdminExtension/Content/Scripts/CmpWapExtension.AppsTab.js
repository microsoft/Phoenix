/*globals window,jQuery,Exp,waz*/
(function ($, global, Shell, Exp, undefined) {
    "use strict";

    var appGrid,
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
             { name: "ID", field: "ApplicationId", sortable: false },
             { name: "AppIdCode ", field: "AppCode", filterable: false, sortable: false },
             { name: "Name", field: "Name", filterable: false, sortable: false },
           
             { name: "IsActive", field: "IsActive", sortable: false }
        ];

        global.Shell.UI.Spinner.show();
        var promise = global.CmpWapExtensionAdminExtension.Controller.getAppsDataSet();

        promise.done(function(value)
        {
            appGrid = renderArea.find(".grid-container")
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
        if (appGrid) {
            appGrid.wazObservableGrid("destroy");
            appGrid = null;
        }
    }

    global.CmpWapExtensionAdminExtension = global.CmpWapExtensionAdminExtension || {};
    global.CmpWapExtensionAdminExtension.AppsTab = {
        loadTab: loadTab,
        cleanUp: cleanUp
    };
})(jQuery, this); 