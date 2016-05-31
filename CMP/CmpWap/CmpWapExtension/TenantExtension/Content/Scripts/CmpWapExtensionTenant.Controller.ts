/*globals window,jQuery,cdm,CmpWapExtensionTenantExtension,waz,Exp*/

declare var Shell;
declare var jQuery;
declare var Exp;
declare var waz;
declare var CmpWapExtensionTenantExtension;
declare var planListUrl;

(($, global, undefined?) => {
    "use strict";

    $('head').append('<meta http-equiv="Pragma" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Control" content="no-cache" />');
    $('head').append('<meta http-equiv="Pragma-directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Expires" content="-1" />');

    var baseUrl = "/CmpWapExtensionTenant", listVMsUrl = baseUrl + "/ListVMs", listDomainsUrl = baseUrl + "/ListDomains", domainType = "CmpWapExtension", nameResolutionUrl = baseUrl + "/NameResolution", vmGetUrl = baseUrl + "/GetVm", vmOpsUrl = baseUrl + "/VmOp", detachedDisksGetUrl = baseUrl + "/GetDetachedDisks", vmGetOpsQueueTaskUrl = baseUrl + "/GetVmOpsQueueTask", vmDahsboardData, mainDashboardrenderArea, mainDashboardrenderData;

    //*************************************************************************
    // Navigates to the main list for the extension
    //*************************************************************************
    function navigateToListView() {
        Shell.UI.Navigation.navigate("#Workspaces/{0}/CmpWapExtension").format(CmpWapExtensionTenantExtension.name);
    }

    //*************************************************************************
    // Returns a list of all virtual machines for a set of WAP subscriptions
    //*************************************************************************
    function getVMs(subscriptionIds) {
        return makeAjaxCall(listVMsUrl, { subscriptionIds: subscriptionIds });
    }

    //*************************************************************************
    // Loads (and reloads) the data set in the virtual machine list
    //*************************************************************************
    function getVmsDataSet(triggerRefreshing) {
        if (triggerRefreshing) {
            var subIds = [];
            var subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("CmpWapExtension");

            subscriptionRegisteredToService.forEach(function (sub) {
                subIds.push(sub.id);
            });

            global.Exp.Data.getData({
                url: listVMsUrl,
                ajaxData: {
                    subscriptionIds: subIds//JSON.stringify(subIds)
                },
                forceCacheRefresh: true
            });
        }
        return global.Exp.Data.getLocalDataSet(listVMsUrl);
    }

    function postVmOps(dataToPost) {
        var subIds = [];
        var Opcode = dataToPost.Opcode, subscriptionRegisteredToService = dataToPost.subscriptionId, VmId = dataToPost.VmId, sData = dataToPost.sData, iData = dataToPost.iData, IsMultiOp = dataToPost.IsMultiOp;

        subscriptionRegisteredToService = typeof subscriptionRegisteredToService !== 'undefined' ? subscriptionRegisteredToService : global.Exp.Rdfe.getSubscriptionsRegisteredToService("CmpWapExtension");

        subscriptionRegisteredToService.forEach(function (sub) {
            subIds.push(sub.id);
        });

        return makeAjaxCall(vmOpsUrl, { subscriptionId: subIds, Opcode: Opcode, VmId: VmId, sData: sData, iData: iData, IsMultiOp: IsMultiOp });
    }

    function makeAjaxCall(url, data) {
        return Shell.Net.ajaxPost({
            url: url,
            data: data
        });
    }

    //*************************************************************************
    // Returns the data set for the main virtual machine list associated
    // with the current WAP subscription
    //*************************************************************************
    function getLocalPlanDataSet() {
        return Exp.Data.getLocalDataSet(planListUrl);
    }

    //*************************************************************************
    // Returns the data set for the list of virtual machines
    //*************************************************************************
    function getVMsData(subscriptionId) {
        return Exp.Data.getData("vm{0}").format(subscriptionId), {
            ajaxData: {
                subscriptionIds: subscriptionId
            },
            url: listVMsUrl,
            forceCacheRefresh: true
        };
    }

    //*************************************************************************
    // Load main page
    //*************************************************************************
    function loadMainPage() {
        var url = "https://" + location.host;

        //does't put originating page in history
        window.location.replace(url);
        return;
    }

    //*************************************************************************
    // Returns details on a virtual machine
    //*************************************************************************
    function getVmdashboardData(vmId) {
        var subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("CmpWapExtension");
        return makeAjaxCall(vmGetUrl, { subscriptionId: subscriptionRegisteredToService[0].id, vmId: vmId });
    }

    //*************************************************************************
    // Returns VM Ops Queue task response
    //*************************************************************************
    function getVmOpsQueueTask(vmId) {
        var subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("CmpWapExtension");
        return makeAjaxCall(vmGetOpsQueueTaskUrl, { subscriptionId: subscriptionRegisteredToService[0].id, vmId: vmId });
    }

    //*************************************************************************
    // Returns a list of detached disks accessible to a given virtual machine
    //*************************************************************************
    function getDetachedDisks(vmId) {
        var subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("CmpWapExtension");
        return makeAjaxCall(detachedDisksGetUrl, { subscriptionId: subscriptionRegisteredToService[0].id, vmId: vmId });
    }

    //*************************************************************************
    // Displays a JSON-formatted representation of domain list data
    //*************************************************************************
    function gotTheDomainListData2(data) {
        var z = data;
        alert(JSON.stringify(data));
    }

    //*************************************************************************
    // Gets and displays a list of domains
    //*************************************************************************
    function getDomainNames(subscriptionId : string[]) {
        $.post(listDomainsUrl, { subscriptionIds: subscriptionId }).done(function (data) {
            gotTheDomainListData2(data.data);
        });
    }

    // TODO: Can we use the waz.dataWrapper in the sample?
    /*function createFileShare(subscriptionId, fileShareName, size, fileServerName) {
    return new waz.dataWrapper(Exp.Data.getLocalDataSet(listFileSharesUrl,true))
    .add(
    {
    Name: fileShareName,
    SubscriptionId: subscriptionId,
    Size: size,
    FileServerName: fileServerName
    },
    Shell.Net.ajaxPost({
    data: {
    subscriptionId: subscriptionId,
    Name: fileShareName,
    Size: size,
    FileServerName: fileServerName
    },
    url: baseUrl + "/CreateFileShare"
    })
    );
    }*/
    //*************************************************************************
    // Resolves a set of security groups
    //*************************************************************************
    function nameResolution(securitygroups) {
        var subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("CmpWapExtension");
        return makeAjaxCall(nameResolutionUrl, { subscriptionId: subscriptionRegisteredToService[0].id, securitygroups: securitygroups });
    }

    //*************************************************************************
    // Submits a request to create a new virtual machine
    //*************************************************************************
    function createVm(subscriptionId, vmAppName, vmAppId, envResourcegroupname, vmServerName, vmDomain, vmAdminName, vmAdminPassword,
        vmSourceImage, vmSize, vmRegion, vmRole, vmDiskSpec, vmConfig, vmTagData, servicecategory, nic1, Msitmonitored,
        sqlConfig, iisconfig, environmentclass, accountAdminLiveEmailId, osCode, azureApiName) {
        return new waz.dataWrapper(Exp.Data.getLocalDataSet(listVMsUrl, true))
            .add(
            {
                Name: vmServerName,
                SubscriptionId: subscriptionId,
                VmAppName: vmAppName,
                VmAppId: vmAppId,
                EnvResourceGroupName: envResourcegroupname,
                VmDomain: vmDomain,
                VmAdminName: vmAdminName,
                VmAdminPassword: vmAdminPassword,
                VmSourceImage: vmSourceImage,
                VmSize: vmSize,
                VmRegion: vmRegion,
                VmRole: "DEFAULT",
                VmDiskSpec: vmDiskSpec,
                VmConfig: vmConfig,
                VmTagData: vmTagData,
                ServiceCategory: servicecategory,
                Nic1: nic1,
                Msitmonitored: Msitmonitored,
                sqlconfig: sqlConfig,
                IIsconfig: iisconfig,
                EnvironmentClass: "DEFAULT",
                AccountAdminLiveEmailId: accountAdminLiveEmailId,
                OsCode: osCode,
                AzureApiName: azureApiName
            },
            Shell.Net.ajaxPost({
                data: {
                    Name: vmServerName,
                    SubscriptionId: subscriptionId,
                    VmAppName: vmAppName,
                    VmAppId: vmAppId,
                    EnvResourceGroupName: envResourcegroupname,
                    VmDomain: vmDomain,
                    VmAdminName: vmAdminName,
                    VmAdminPassword: vmAdminPassword,
                    VmSourceImage: vmSourceImage,
                    VmSize: vmSize,
                    VmRegion: vmRegion,
                    VmRole: "DEFAULT",
                    VmDiskSpec: vmDiskSpec,
                    VmConfig: vmConfig,
                    VmTagData: vmTagData,
                    ServiceCategory: servicecategory,
                    Nic1: nic1,
                    Msitmonitored: Msitmonitored,
                    sqlconfig: sqlConfig,
                    IIsconfig: iisconfig,
                    EnvironmentClass: "DEFAULT",
                    AccountAdminLiveEmailId: accountAdminLiveEmailId,
                    OsCode: osCode,
                    AzureApiName: azureApiName
                },
                url: baseUrl + "/CreateVM"
            })
            );
    }

    //*************************************************************************
    // Submits a request to create a new virtual machine using static template
    //*************************************************************************
    function createVmFromStaticTemplate(subscriptionId, TemplateText) {
        return new waz.dataWrapper(Exp.Data.getLocalDataSet(listVMsUrl, true))
            .add(
            {
                Name: subscriptionId,
                Template: TemplateText                
            },
            Shell.Net.ajaxPost({
                data: {
                    Name: subscriptionId,
                    Template: TemplateText                                   
                },
                url: baseUrl + "/CreateVmFromStaticTemplate"
            })
            );
    }

    global.CmpWapExtensionTenantExtension = global.CmpWapExtensionTenantExtension || {};
    global.CmpWapExtensionTenantExtension.Controller = {
        //createFileShare: createFileShare,
        createVm: createVm,
        createVmFromStaticTemplate: createVmFromStaticTemplate,
        //listFileSharesUrl: listFileSharesUrl,
        listVMsUrl: listVMsUrl,
        listDomainsUrl: listDomainsUrl,
        //getFileShares: getFileShares,
        getVMs: getVMs,
        getLocalPlanDataSet: getLocalPlanDataSet,
        //getfileSharesData: getfileSharesData,
        getVMsData: getVMsData,
        navigateToListView: navigateToListView,
        getDomainNames: getDomainNames,
        getVmsDataSet: getVmsDataSet,
        nameResolution: nameResolution,
        getVmdashboardData: getVmdashboardData,
        getDetachedDisks: getDetachedDisks,
        postVmOps: postVmOps,
        getVmOpsQueueTask: getVmOpsQueueTask,
        loadMainPage: loadMainPage
    };
})(jQuery, this);
