/*globals window,jQuery,Exp,waz*/
(function ($, global, Shell, Exp, undefined) {
    "use strict";

    var grid,
        statusIcons =
        {
            Registered:
            {
                text: "Registered",
                iconName: "complete"
            },
            Default:
            {
                iconName: "spinner"
            }
        };

    //*************************************************************************
    //*************************************************************************
    //*************************************************************************

    var i = 0;

    function addValue() {
        var v = document.form1.txtValue.value;
        // get the TextBox Value and assign it into the variable
        var AddOpt = new Option(v, v);
        document.form1.lstValue.options[i++] = AddOpt;
        return true;
    }

    function deleteValue() {
        var s = 1;
        var Index;

        if (document.form1.lstValue.selectedIndex == -1) {
            alert("Please select any item from the ListBox");
            return true;
        }

        while (s > 0) {
            Index = document.form1.lstValue.selectedIndex;

            if (Index >= 0) {
                document.form1.lstValue.options[Index] = null;
                --i;
            }
            else
                s = 0;
        }
        return true;
    }

    //*************************************************************************
    // 
    //*************************************************************************

    var resourceProviderGroupList;
    var resourceProviderList;
    var selectedRow;
    var editingRow;

    //*************************************************************************
    // 
    //*************************************************************************

    function onRowSelected(row) {
        if (null != row)
            selectedRow = row;
    }

    var i = 0;

    function AddTenantToSpg() {
        var v = $("#txtValue").val;
        // get the TextBox Value and assign it into the variable
        var AddOpt = new Option(v, v);
        document.form1.lstValue.options[i++] = AddOpt;
        return true;
    }

    //*************************************************************************
    // Initializes the UI
    //*************************************************************************

    function populateUiElements() {
        var listItems = "";
        for (var i = 0; i < resourceProviderGroupList.length; i++) {
            listItems += "<option data-Id='" + resourceProviderGroupList[i] +
                "' value='" + resourceProviderGroupList[i] + "'>" +
                resourceProviderGroupList[i] + "</option>";
        }

        $("#resourceGroup").html(listItems);

        if (null != editingRow) {
            $("#name").val(selectedRow.Name);
            $("#description").val(selectedRow.Description);
            $("#resourceGroup").val(selectedRow.ResourceGroup);
            $("#accountId").val(selectedRow.AccountID);
            $("#certificateThumbprint").val(selectedRow.CertificateThumbprint);
        }

        //$("#addTenantToSPG").onclick(function () { AddTenantToSpg(); });

    }

    //*************************************************************************
    // OnX
    //*************************************************************************

    function addServiceProviderAccount(id, name, description, resourceGroup, accountId, accountType, certificateThumbprint) {
        //var newSettings = $.extend(true, {}, global.CmpWapExtensionAdminExtension.Controller.getCurrentAdminSettings());
        //newSettings.EndpointAddress = newEndpointUrl;
        //newSettings.Username = newUsername;
        //newSettings.Password = newPassword;

        return global.CmpWapExtensionAdminExtension.Controller.addServiceProviderAccount(
            id, name, description, resourceGroup, accountId, accountType, certificateThumbprint);
    }

    function onEdit() {
        editingRow = selectedRow;
        onChange(false);
    }

    function onAdd() {
        editingRow = null;
        onChange(true);
    }

    function onChange(adding) {
        var promise,
            wizardContainerSelector = ".hw-AddServiceProviderAccount";

        var htmlResources = "";
        var registerReseller = "";

        cdm.stepWizard({
            extension: "CmpWapExtensionAdminExtension",
            steps: [
                {
                    template: "EditServiceProviderAccountDialog1",
                    htmlResources: htmlResources,
                    data: { registerReseller: registerReseller },
                    onStepActivate: function () {
                        //Shell.UI.Validation.setValidationContainer(wizardContainerSelector);
                    },
                    onStepCreated: function () {
                        populateUiElements();
                    }
                }
            ],

            onComplete: function () {
                if (!global.Shell.UI.Validation.validateContainer(wizardContainerSelector)) {
                    return false;
                };

                var iD = 0;
                var name = $("#name").val();
                var description = $("#description").val();
                var resourceGroup = $("#resourceGroup").val();
                var accountId = $("#accountId").val();
                var accountType = $("#accountType").val();
                var certificateThumbprint = $("#certificateThumbprint").val();

                if (!adding)
                    iD = editingRow.ID;

                promise = addServiceProviderAccount(iD, name, description, resourceGroup, accountId, accountType, certificateThumbprint);

                global.waz.interaction.showProgress
                (
                    promise,
                    {
                        initialText: "Registering service provider account...",
                        successText: "Successfully registered the account.",
                        failureText: "Failed to register the account."
                    }
                );

                promise.done(function () {
                    //global.CmpWapExtensionAdminExtension.Controller.invalidateAdminSettingsCache();
                });
            }
        },
        { size: "mediumplus" });
    }











    function onDelete(item) {
        // store the current vmName as tab may change between notifications
        //var targetVMName = vmName;
        var targetVMName = "blah";
        cdm.stepWizard({
            extension: "CmpWapExtensionTenantExtension",
            steps: [
                {
                    template: "DeleteVMDialogBox",
                    onStepCreated: function () {
                        //wizard = this;
                        // options description
                        var optiondesc = {
                            deletedisk: "This option deletes the disk attached to the VM.",
                            keepdisk: "This option does not delete any disks attached to the VM."
                        };

                        $("#vmdelete-option-desc").text(optiondesc.deletedisk); //tie description to textbox

                        var types = [{ text: "Delete VM with disk", value: "deleteVManddisk" },
                                     { text: "Delete VM keep disk", value: "deallocateVMnodisk" }];

                        $("#vm-delete-radio").fxRadio({
                            value: types[0], //default value
                            values: types, //list of values to display
                            change: function (event, args) { //bind to change event. Toggle between options
                                if (args.value.value == "deleteVManddisk") {
                                    $("#vmdelete-option-desc").text(optiondesc.deletedisk);
                                }
                                else if (args.value.value == "deallocateVMnodisk") {
                                    $("#vmdelete-option-desc").text(optiondesc.keepdisk);
                                }
                            }
                        });
                    },
                    onComplete: function () {
                        //helper to get the selected value
                        var getSelectedValue = function () {
                            return $("#vm-delete-radio").fxRadio("value");
                        };

                        /*switch (getSelectedValue().value) {
                            case "deleteVManddisk":
                                onDeleteVMkWithDisk(item, targetVMName);
                                break;
                            case "deallocateVMnodisk":
                                onDeleteVMWithoutDisk(item, targetVMName);
                                break;
                        }*/
                    }
                }
            ]
        },
        { size: "large" });
    }

    //*************************************************************************
    // Sets the application bar command states
    //*************************************************************************

    function setContextualCommands(enableAdd, enableDelete) {
        global.Exp.UI.Commands.Contextual.clear();
        //global.Exp.UI.Commands.Contextual.add(new global.Exp.UI.Command("add", "Add", global.Exp.UI.CommandIconDescriptor.getWellKnown("add"), enableAdd, null, onAdd));
        //global.Exp.UI.Commands.Contextual.add(new global.Exp.UI.Command("edit", "Edit", global.Exp.UI.CommandIconDescriptor.getWellKnown("edit"), enableAdd, null, onEdit));
        //global.Exp.UI.Commands.Contextual.add(new global.Exp.UI.Command("delete", "Delete", global.Exp.UI.CommandIconDescriptor.getWellKnown("delete"), enableDelete, null, onDelete));
        global.Exp.UI.Commands.update();
    }

    //*************************************************************************
    // Saves the list of domains
    //*************************************************************************

    function gotTheRpListData(data) {
        resourceProviderList = data;
        resourceProviderGroupList = ["---"];

        //alert("init " + JSON.stringify(data));

        for (var i = 0; i < resourceProviderList.length; i++)
            //if (-1 == resourceProviderGroupList.indexOf(resourceProviderList[i].ResourceGroup))
            resourceProviderGroupList.push(resourceProviderList[i].ResourceGroup);
    }

    //*************************************************************************
    // Fetches the list of domains from the API
    //*************************************************************************
    function fetchRpList() {
        $.post(global.CmpWapExtensionAdminExtension.Controller.adminServiceProviderAccountsUrl, {})
          .done(function (data) {
              gotTheRpListData(data.data);
          });
    }

    //*************************************************************************
    // Load Tab
    //*************************************************************************

    function loadTab(extension, renderArea, initData) {
        setContextualCommands(false, false); // set to false as mapping is implemented in plan extensions
        fetchRpList();

        var localDataSet =
        {
            url: global.CmpWapExtensionAdminExtension.Controller.adminServiceProviderAccountsUrl,
            dataSetName: global.CmpWapExtensionAdminExtension.Controller.adminServiceProviderAccountsUrl
            //url: global.CmpWapExtensionAdminExtension.Controller.adminFileServersUrl,
            //dataSetName: global.CmpWapExtensionAdminExtension.Controller.adminFileServersUrl
        },
        columns =
        [
                { name: "ID", field: "ID", sortable: false },
                { name: "Name", field: "Name", sortable: false },
                { name: "Description", field: "Description", sortable: false },
                { name: "Resource Group", field: "ResourceGroup", sortable: false },
                { name: "AccountID", field: "AccountID", sortable: false },
                //{ name: "AccountPassword", field: "AccountPassword", sortable: false },
                { name: "Account Type", field: "AccountType", sortable: false },
                //{ name: "Az Affinity Group", field: "AzAffinityGroup", sortable: false },
                //{ name: "Az Region", field: "AzRegion", sortable: false },
                //{ name: "Az Storage Container Url", field: "AzStorageContainerUrl", sortable: false },
                //{ name: "Az Subnet", field: "AzSubnet", sortable: false },
                //{ name: "Az VNet", field: "AzVNet", sortable: false },
                //{ name: "CertificateBlob", field: "CertificateBlob", sortable: false },
                { name: "Certificate Thumbprint", field: "CertificateThumbprint", sortable: false },
                //{ name: "Config", field: "Config", sortable: false },
                //{ name: "Core Count Current", field: "CoreCountCurrent", sortable: false },
                //{ name: "Core Count Max", field: "CoreCountMax", sortable: false },
                //{ name: "ExpirationDate", field: "ExpirationDate", sortable: false },
                //{ name: "Owner Names CSV", field: "OwnerNamesCSV", sortable: false },
                { name: "Active", field: "Active", sortable: false }
                //{ name: "TagData", field: "TagData", sortable: false },
                //{ name: "TagID", field: "TagID", sortable: false }
        ];

        grid = renderArea.find(".grid-container")
            .wazObservableGrid("destroy")
            .wazObservableGrid({
                lastSelectedRow: null,
                data: localDataSet,
                keyField: "Name",
                columns: columns,
                gridOptions:
                {
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

        setContextualCommands(false, false);
    }

    global.CmpWapExtensionAdminExtension = global.CmpWapExtensionAdminExtension || {};
    /*global.CmpWapExtensionAdminExtension.FileServersTab = {
        loadTab: loadTab,
        cleanUp: cleanUp
    };*/
    global.CmpWapExtensionAdminExtension.ServiceProviderAccountsTab = {
        loadTab: loadTab,
        cleanUp: cleanUp
    };
})(jQuery, this);