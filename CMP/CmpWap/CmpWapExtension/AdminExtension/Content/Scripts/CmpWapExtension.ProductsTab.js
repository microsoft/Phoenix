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

    function dateFormatter(value) {
        return $.datepicker.formatDate("m/d/yy", value);
    }

    function onRowSelected(row) {
    }

    function loadTab(extension, renderArea, initData) {
        var localDataSet = {
            url: global.CmpWapExtensionAdminExtension.Controller.adminProductsUrl,
            dataSetName: global.CmpWapExtensionAdminExtension.Controller.adminProductsUrl
        },
            columns = [
                { name: "Name", field: "ProductName", sortable: false },
                { name: "Price", field: "ProductPrice", filterable: false, sortable: false },
                { name: "Expires", field: "ExpiryDate", type: "date", filterable: false, sortable: false, formatter: dateFormatter },
                { name: "Units", field: "NumberOfUnits", filterable: false, sortable: false },

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
                },
                emptyListOptions: {
                    extensionName: "CmpWapExtensionAdminExtension",
                    templateName: "productsTabEmpty"
                }
            });
    }

    function cleanUp() {
        if (grid) {
            grid.wazObservableGrid("destroy");
            grid = null;
        }
    }

    global.CmpWapExtensionAdminExtension = global.CmpWapExtensionAdminExtension || {};
    global.CmpWapExtensionAdminExtension.ProductsTab = {
        loadTab: loadTab,
        cleanUp: cleanUp
    };
})(jQuery, this);