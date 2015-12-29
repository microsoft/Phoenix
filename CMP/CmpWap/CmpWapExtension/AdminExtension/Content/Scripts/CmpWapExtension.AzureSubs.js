(function ($, global, Shell, Exp, undefined) {
    "use strict";

    var azureGrid,
        planInfo,
        statusIcons = {
            Registered: {
                text: "Registered",
                iconName: "complete"
            },
            Default: {
                iconName: "spinner"
            }
        };

    function loadTab(grid) {
        var columns = [
             { name: "ID", field: "id", sortable: false },
             { name: "Subscription Id", field: "subid", filterable: false, sortable: false },
             { name: "Subscription Name", field: "subname", filterable: false, sortable: true },
             { name: "Subscription State", field: "SubscriptionState", sortable: false },
             { name: "Inserted Date", field: "InsertedDate", sortable: true }
        ];

        var dummyRows = [
            {
                id: "123abc",
                subid: "abc123",
                SubscriptionState: 1,
                InsertedDate: "March123"
            },
            {
                id: "456abc",
                subid: "abc456",
                SubscriptionState: 0,
                InsertedDate: "March123"
            }
        ];

        global.Shell.UI.Spinner.show();
       /* var promise = global.CmpWapExtensionAdminExtension.Controller.getAzureSubsDataSet(planInfo); // TODO: put plan Id here
        promise.done(function (value) {
            azureGrid = grid
                    .fxGrid({
                        columns: columns,
                        rowSelect: function () { },
                        data: dummyRows,
                        selectable: false,
                        multiselect: false,
                        maxHeight: 400
                    });
            })
         .fail(function (val) { global.Shell.UI.Spinner.hide(); })
        .always(function () {
            global.Shell.UI.Spinner.hide();
        });*/
        azureGrid = grid
                .fxGrid({
                    columns: columns,
                    rowSelect: function () { },
                    data: dummyRows,
                    selectable: false,
                    multiselect: false,
                    maxHeight: 400
                });
    }

    function cleanUp() {
        if (azureGrid) {
            azureGrid.wazObservableGrid("destroy");
            azureGrid = null;
        }
    }

    global.CmpWapExtensionAdminExtension = global.CmpWapExtensionAdminExtension || {};
    global.CmpWapExtensionAdminExtension.AzureSubs = {
        loadTab: loadTab,
        cleanUp: cleanUp
    };
})(jQuery, this);