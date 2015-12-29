/*globals window,jQuery,cdm, CmpWapExtensionAdminExtension*/
(function ($, global, undefined) {
    "use strict";

    var baseUrl = "/CmpWapExtensionAdmin",
        adminSettingsUrl = baseUrl + "/AdminSettings",
        adminProductsUrl = baseUrl + "/Products",
        adminFileServersUrl = baseUrl + "/FileServers",
        adminVmsUrl = baseUrl + "/Vms",
        adminDomainsUrl = baseUrl + "/Domains",
        adminOssUrl = baseUrl + "/Oss",
        adminVmSizesUrl = baseUrl + "/Vmsizes",
        adminRegionsUrl = baseUrl + "/Regions",
        adminAppsUrl = baseUrl + "/Apps",
        adminCreateVmUrl = baseUrl + "/CreateVm",
        adminVmopUrl = baseUrl + "/Vmop",
        adminServiceProviderAccountsUrl = baseUrl + "/ServProvAccts",
        adminUpdateServiceProviderAccountUrl = baseUrl + "/UpdateServiceProviderAccount";

    function makeAjaxCall(url, data) {
        return Shell.Net.ajaxPost({
            url: url,
            data: data
        });
    }



    function getAppsDataSet() {
        return makeAjaxCall(adminAppsUrl);
    }

    function getVmsDataSet() {
        return makeAjaxCall(adminVmsUrl);
    }
    function updateAdminSettings(newSettings) {
        return makeAjaxCall(baseUrl + "/UpdateAdminSettings", newSettings);
    }

    function addServiceProviderAccount(
    id, name, description, resourceGroup, accountId, accountType, certificateThumbprint) {
        //return makeAjaxCall(baseUrl + "/UpdateAdminSettings", newSettings);

        return Shell.Net.ajaxPost(
        {
            data:
            {
                ID: id,
                Name: name,
                Description: description,
                ResourceGroup: resourceGroup,
                AccountID: accountId,
                AccountType: accountType,
                CertificateThumbprint: certificateThumbprint
            },
            url: adminUpdateServiceProviderAccountUrl
        });

        return makeAjaxCall(adminUpdateServiceProviderAccountUrl, {
            Name: name,
            Description: description,
            ResourceGroup: resourceGroup,
            AccountID: accountId,
            AccountType: accountType,
            CertificateThumbprint: certificateThumbprint

            /*ID 
            OwnerNamesCSV
            Config
            TagData
            TagID
            ExpirationDate
            CertificateBlob
            AccountPassword
            Active
            AzRegion
            AzAffinityGroup
            AzVNet
            AzSubnet
            AzStorageContainerUrl
            CoreCountMax
            CoreCountCurrent*/
        });
    }


    function invalidateAdminSettingsCache() {
        return global.Exp.Data.getData({
            url: global.CmpWapExtensionAdminExtension.Controller.adminSettingsUrl,
            dataSetName: CmpWapExtensionAdminExtension.Controller.adminSettingsUrl,
            forceCacheRefresh: true
        });
    }

    function getCurrentAdminSettings() {
        return makeAjaxCall(global.CmpWapExtensionAdminExtension.Controller.adminSettingsUrl);
    }

    function isResourceProviderRegistered() {
        global.Shell.UI.Spinner.show();
        global.CmpWapExtensionAdminExtension.Controller.getCurrentAdminSettings()
        .done(function (response) {
            if (response && response.data.EndpointAddress) {
                return true;
            }
            else {
                return false;
            }
        })
         .always(function () {
             global.Shell.UI.Spinner.hide();
         });
    }

    // Public
    global.CmpWapExtensionAdminExtension = global.CmpWapExtensionAdminExtension || {};
    global.CmpWapExtensionAdminExtension.Controller = {
        adminSettingsUrl: adminSettingsUrl,
        adminProductsUrl: adminProductsUrl,
        adminFileServersUrl: adminFileServersUrl,
        updateAdminSettings: updateAdminSettings,
        getCurrentAdminSettings: getCurrentAdminSettings,
        getAppsDataSet: getAppsDataSet,
        getVmsDataSet:getVmsDataSet,
        invalidateAdminSettingsCache: invalidateAdminSettingsCache,
        isResourceProviderRegistered: isResourceProviderRegistered,
        adminVmsUrl: adminVmsUrl,
        adminDomainsUrl: adminDomainsUrl,
        adminOssUrl: adminOssUrl,
        adminVmSizesUrl: adminVmSizesUrl,
        adminRegionsUrl: adminRegionsUrl,
        adminCreateVmUrl: adminCreateVmUrl,
        adminVmopUrl: adminVmopUrl,
        adminAppsUrl: adminAppsUrl,
        adminServiceProviderAccountsUrl: adminServiceProviderAccountsUrl,
        addServiceProviderAccount: addServiceProviderAccount
    };
})(jQuery, this);
