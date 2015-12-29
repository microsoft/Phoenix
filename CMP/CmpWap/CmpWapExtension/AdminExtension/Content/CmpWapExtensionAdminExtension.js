/*globals window,jQuery,Shell,Exp,waz*/

(function (global, $, undefined) {
    "use strict";

    var resources = [],
        CmpWapExtensionExtensionActivationInit,
        navigation;   

    function clearCommandBar() {
        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.Global.clear();
        Exp.UI.Commands.update();
    }

    function onApplicationStart() {
        Exp.UserSettings.getGlobalUserSetting("Admin-skipQuickStart").then(function (results) {
            var setting = results ? results[0] : null;
            if (setting && setting.Value) {
                global.CmpWapExtensionAdminExtension.settings.skipQuickStart = JSON.parse(setting.Value);
            }
        });
                
        global.CmpWapExtensionAdminExtension.settings.skipQuickStart = false;
    }

    function loadQuickStart(extension, renderArea, renderData) {
        clearCommandBar();
        global.CmpWapExtensionAdminExtension.QuickStartTab.loadTab(renderData, renderArea);
    }

    function loadVmsTab(extension, renderArea, renderData) {
        global.CmpWapExtensionAdminExtension.VmsTab.loadTab(renderData, renderArea);
    }

    function loadDomainsTab(extension, renderArea, renderData) {
        global.CmpWapExtensionAdminExtension.DomainsTab.loadTab(renderData, renderArea);
    }

    function loadOssTab(extension, renderArea, renderData) {
        global.CmpWapExtensionAdminExtension.OssTab.loadTab(renderData, renderArea);
    }

    function loadServiceProviderAccountsTab(extension, renderArea, renderData) {
        global.CmpWapExtensionAdminExtension.ServiceProviderAccountsTab.loadTab(renderData, renderArea);
    }

    function loadVmsizesTab(extension, renderArea, renderData) {
        global.CmpWapExtensionAdminExtension.VmSizesTab.loadTab(renderData, renderArea);
    }

    function loadRegionsTab(extension, renderArea, renderData) {
        global.CmpWapExtensionAdminExtension.RegionsTab.loadTab(renderData, renderArea);
    }

    function loadAppsTab(extension, renderArea, renderData) {
        global.CmpWapExtensionAdminExtension.AppsTab.loadTab(renderData, renderArea);
    }

    //*****************

    function loadSettingsTab(extension, renderArea, renderData) {
        global.CmpWapExtensionAdminExtension.SettingsTab.loadTab(renderData, renderArea);
    }

    function loadControlsTab(extension, renderArea, renderData) {
        global.CmpWapExtensionAdminExtension.ControlsTab.loadTab(renderData, renderArea);
    }

    global.CmpWapExtensionExtension = global.CmpWapExtensionAdminExtension || {};

    navigation = {
        tabs: [
                {
                    id: "quickStart",
                    displayName: "quickStart",
                    template: "quickStartTab",
                    activated: loadQuickStart
                },
                {
                  
                    id: "Vms",
                    displayName: "VMs",
                    template: "vmsTab",
                    activated: loadVmsTab
                },
                {
                    id: "Apps",
                    displayName: "Applications",
                    template: "ApspTab",
                    activated: loadAppsTab
                },
                {
                    id: "Regions",
                    displayName: "Regions",
                    template: "RegionsTab",
                    activated: loadRegionsTab
                },
                {
                    id: "Domains",
                    displayName: "Domains",
                    template: "DomainsTab",
                    activated: loadDomainsTab
                },
                {
                    id: "Oss",
                    displayName: "OS List",
                    template: "OssTab",
                    activated: loadOssTab
                },
                {
                    id: "ServiceProviderAccounts",
                    displayName: "Service Provider Accounts",
                    template: "ServiceProviderAccountsTab",
                    activated: loadServiceProviderAccountsTab
                },
                {
                    id: "Vmsizes",
                    displayName: "Vm Sizes",
                    template: "VmsizesTab",
                    activated: loadVmsizesTab
                },
                {
                    id: "settings",
                    displayName: "settings",
                    template: "settingsTab",
                    activated: loadSettingsTab
                }
        ],
        types: [
        ]
    };

    CmpWapExtensionExtensionActivationInit = function () {
        var CmpWapExtensionExtension = $.extend(this, global.CmpWapExtensionAdminExtension);

        $.extend(CmpWapExtensionExtension, {
            displayName: "Azure VM Cloud",
            viewModelUris: [
                global.CmpWapExtensionAdminExtension.Controller.adminSettingsUrl,
                global.CmpWapExtensionAdminExtension.Controller.adminProductsUrl,
            ],
            menuItems: [],
            settings: {
                skipQuickStart: true
            },
            getResources: function () {
                return resources;
            }
        });

        CmpWapExtensionExtension.onApplicationStart = onApplicationStart;        
        CmpWapExtensionExtension.setCommands = clearCommandBar();

        Shell.UI.Pivots.registerExtension(CmpWapExtensionExtension, function () {
            Exp.Navigation.initializePivots(this, navigation);
        });

        // Finally activate CmpWapExtensionExtension 
        $.extend(global.CmpWapExtensionAdminExtension, Shell.Extensions.activate(CmpWapExtensionExtension));
    };

    Shell.Namespace.define("CmpWapExtensionAdminExtension", {
        init: CmpWapExtensionExtensionActivationInit
    });

})(this, jQuery, Shell, Exp);