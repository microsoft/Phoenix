/*globals window,jQuery,Shell,Exp,waz*/
(function ($, global, Exp, undefined) {
    "use strict";

    // Putting these scripts into a bundle triggers very long and unnecessary MEF dependency resolution. So put it here instead.
    function executeCommand(commandId, commandParameter) {
    }

    function loadAzureSubs(azureGrid) {
        global.CmpWapExtensionAdminExtension.AzureSubs.loadTab(azureGrid);
    }

    // private: Receives config from host and updates the UI
    function initializeServiceOffer(serviceOffer) {
        $(".hs-empty-header").hide();
        //loadAzureSubs($(".grid-container"));
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

        $(".grid-container").fxGrid({
            columns: columns,
            rowSelect: function () { },
            data: dummyRows,
            selectable: false,
            multiselect: false,
            maxHeight: 400
        });
    }

    function initializePage() {
        global.CmpWapExtensionExtension = global.CmpWapExtensionAdminExtension || {};
        global.ServiceOffer.registerExtension({
            initializeServiceOffer: initializeServiceOffer,
            executeCommand: executeCommand
        });
    }

    $(document).ready(initializePage);
})(jQuery, this, Exp);