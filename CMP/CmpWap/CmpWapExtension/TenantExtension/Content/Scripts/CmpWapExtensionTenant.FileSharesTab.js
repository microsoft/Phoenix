$('head').append('<meta http-equiv="Pragma" content="no-cache" />');
$('head').append('<meta http-equiv="Cache-Control" content="no-cache" />');
$('head').append('<meta http-equiv="Pragma-directive" content="no-cache" />');
$('head').append('<meta http-equiv="Cache-Directive" content="no-cache" />');
$('head').append('<meta http-equiv="Expires" content="-1" />');

/*
/// <reference path="CmpWapExtensiontenant.createwizard.js" />
/// <reference path="CmpWapExtensiontenant.controller.js" />
*/
/// <reference path="cmpwapextensiontenant.controller.ts" />
/*globals window,jQuery,CmpWapExtensionTenantExtension,Exp,waz,cdm*/
var domainList, sizeInfoList, osInfoList, subscriptionIdList, targetRegionsList, appList, environmenttypeList, serviceCategoryList, serverRoleList, networkNicList, serverroleDrivemappingList, sqlcollationList, sqlVersionList, selectedVmRow, navigation, vmDashboardInfo, iisroleservicesList, sqlanalysisservicemodesList, resourcegroupList;


(function ($, global, undefined) {
    "use strict";

    var grid, selectedRow, statusIcons = {
        Registered: {
            text: "Registered",
            iconName: "complete"
        },
        Default: {
            // iconName: "spinner" // no spinner icon available. Icon not getting displayed
            iconName: "null"
        }
    };

    //*************************************************************************
    // Converts the value to a standardized format
    //*************************************************************************
    function dateFormatter(value) {
        try  {
            if (value) {
                return $.datepicker.formatDate("m/d/yy", value);
            }
        } catch (err) {
        }

        return "-";
    }

    //*************************************************************************
    // Launches a remote desktop connection
    //*************************************************************************
    function Launch_TSC(strServer) {
        var strTSCpath = "C:\\windows\\system32\\mstsc.exe /v:";
        var ws = new ActiveXObject("WScript.Shell");
        ws.Exec(strTSCpath + strServer);
    }

    //*************************************************************************
    // Called when a row is selected
    //*************************************************************************
    function onRowSelected(row) {
        if (row) {
            selectedVmRow = row;
            selectedRow = row;
            //updateContextualCommands(row);
        }
    }

    //*************************************************************************
    // Loads a view displaying information on the current user
    //*************************************************************************
    function onViewInfo(item) {
        cdm.stepWizard({
            extension: "DomainTenantExtension",
            steps: [
                {
                    template: "viewInfo",
                    contactInfo: global.DomainTenantExtension.Controller.getCurrentUserInfo(),
                    domain: selectedRow
                }
            ]
        }, { size: "mediumplus" });
    }

    //*************************************************************************
    // Loads a user interface for changing the user's password
    //*************************************************************************
    function changePassword(currentUserInfo) {
        var promise, wizardContainerSelector = ".dm-selectPassword";

        cdm.stepWizard({
            extension: "DomainTenantExtension",
            steps: [
                {
                    template: "selectPassword",
                    data: {
                        customerId: currentUserInfo.GoDaddyShopperId
                    },
                    onStepActivate: function () {
                        Shell.UI.Validation.setValidationContainer(wizardContainerSelector);
                    }
                }
            ],
            onComplete: function () {
                if (!Shell.UI.Validation.validateContainer(wizardContainerSelector)) {
                    return false;
                }

                currentUserInfo.GoDaddyShopperPassword = $("#dm-password").val();
                currentUserInfo.GoDaddyShopperPasswordChanged = true;
                promise = global.DomainTenantExtension.Controller.updateUserInfo(currentUserInfo);

                global.waz.interaction.showProgress(promise, {
                    initialText: "Reseting password...",
                    successText: "Successfully reset the password.",
                    failureText: "Failed to reset the password."
                });

                promise.done(function () {
                    global.DomainTenantExtension.Controller.invalidateUserInfoCache();
                    var portalUrl = global.DomainTenantExtension.Controller.getCurrentUserInfo().GoDaddyCustomerPortalUrl;
                    window.open(portalUrl, "_blank");
                });
            }
        }, { size: "small" });
    }

    //*************************************************************************
    // Opens the Quick Create dialog to the file share creation page
    //*************************************************************************
    function openQuickCreate() {
        Exp.Drawer.openMenu("AccountsAdminMenuItem/CreateFileShare");
    }

    //*************************************************************************
    // Initializes the list for the main page
    //*************************************************************************
    function loadTab(extension, renderArea, initData) {
        // $(".dm-main").text('');
        var subs = Exp.Rdfe.getSubscriptionList(), subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("CmpWapExtension");

        var dataset = global.CmpWapExtensionTenantExtension.Controller.getVmsDataSet(true);

        //if (dataset == null) {
        //    setTimeout(function () { loadTab(extension, renderArea, initData); }, 1000);
        //    return;
        //}
        var columns = [
            { name: "NAME", field: "Name", type: "navigation", navigationField: "Name", sortable: true },
            //{ name: "App Name", field: "VmAppName", filterable: false, sortable: false },
            { name: "BUILD STATUS", field: "StatusCode", filterable: false, sortable: false },
            { name: "STATUS MESSAGE", field: "StatusMessage", filterable: false, sortable: false },
            { name: "LOCATION", field: "VmRegion", filterable: false, sortable: false },
            { name: "SIZE", field: "VmSize", filterable: false, sortable: false }
        ];

        grid = renderArea.find(".gridContainer").wazObservableGrid("destroy").wazObservableGrid({
            lastSelectedRow: null,
            data: dataset.data,
            keyField: "Name",
            columns: columns,
            gridOptions: {
                rowSelect: onRowSelected
            },
            emptyListOptions: {
                extensionName: "CmpWapExtensionTenantExtension",
                templateName: "FileSharesTabEmpty"
            }
        });

        subscriptionIdList = subscriptionRegisteredToService;
    }

    //*************************************************************************
    // Clears out the grid
    //*************************************************************************
    function cleanUp() {
        if (grid) {
            grid.wazObservableGrid("destroy");
            grid = null;
        }
    }

    global.CmpWapExtensionTenantExtension = global.CmpWapExtensionTenantExtension || {};
    global.CmpWapExtensionTenantExtension.FileSharesTab = {
        loadTab: loadTab,
        cleanUp: cleanUp,
        statusIcons: statusIcons
    };
})(jQuery, this);
