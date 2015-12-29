/// <disable>JS2076.IdentifierIsMiscased</disable>
declare var global;
declare var $;
declare var jQuery;

((global, $, undefined?) => {
    "use strict";

    // Variables    
    var subscriptions;
    var subscriptionId;
    var fileShareName;
    var size;
    var fileServerName;

    //*************************************************************************
    // ViewModel for creating file shares
    //*************************************************************************
    function QuickCreateViewModel() {
        this.subscriptions = this.getSubscriptions();
        this.subscriptionId = this.subscriptions.length ? this.subscriptions[0].id : null;
        var _selectors = {
            container: "#hw-create-fileshare-container",
            fileShareName: "#fileShareName",
            size: "#size",
            subscriptions: "#subscriptions",
            fileServerName: "#fileServerName"
        }
    }

    //*************************************************************************
    // Called when opening Quick Create
    //*************************************************************************
    function onOpened() {
        // using AppExtension subscription drop down as it handles disabled and single subscriptions properly
        global.AppExtension.UserContext.populateOrHideSubscriptionsDropDown("subscriptions", null, null, null, null, "CmpWapExtension");
    }

    //*************************************************************************
    // Called when clicking the OK button
    //*************************************************************************
    function onOkClicked() {
        global.CmpWapExtensionTenantExtension.Controller.createFileShare(this.subscriptionId, this.fileShareName, this.size, this.fileServerName);
    }

    //*************************************************************************
    // Returns a list of WAP subscriptions registered to the extension
    //*************************************************************************
    function getSubscriptions() {
        return global.Exp.Rdfe.getSubscriptionsRegisteredToService(global.CmpWapExtensionTenantExtension.serviceName);
    }

    global.CmpWapExtensionTenantExtension = global.CmpWapExtensionTenantExtension || {};
    global.CmpWapExtensionTenantExtension.ViewModels = {
        QuickCreateViewModel: QuickCreateViewModel,
        onOpened: onOpened,
        onOkClicked: onOkClicked
    };
})(jQuery, this);