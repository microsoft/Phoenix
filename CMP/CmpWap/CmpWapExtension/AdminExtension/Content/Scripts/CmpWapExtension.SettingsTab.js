/// <reference path="CmpWapExtensionadmin.controller.js" />
/*globals,jQuery,trace,cdm,CmpWapExtensionAdminExtension,waz,Exp*/
(function ($, global, Shell, Exp, undefined) {
    "use strict";

    var commandsEnabled,
        passwordChanged = false;

    function renderPage(adminSettings) {
        // This is a placeholder password that is used for rendering only.
        // We want to populate the password textbox to indicate that there's already a password there even though the API hides it.
        if (adminSettings != null && adminSettings != undefined) {
            adminSettings.Password = "dummy4RenderingOnly";
        }

        $("#hw-endpointUrl").val(adminSettings.EndpointAddress ? adminSettings.EndpointAddress : null);
        $("#hw-username").val(adminSettings.Username ? adminSettings.Username : null);
        $("#hw-password").val(adminSettings.Password ? adminSettings.Password : null);
    }

    function onSettingChanged() {
        updateContextualCommands(true);
    }

    function updateContextualCommands(hasPendingChanges) {
        if (commandsEnabled !== hasPendingChanges) {
            Exp.UI.Commands.Contextual.clear();
            if (hasPendingChanges) {
                Exp.UI.Commands.Contextual.add(new Exp.UI.Command("saveSettings", "Save", Exp.UI.CommandIconDescriptor.getWellKnown("save"), true, null, onSaveSettings));
                Exp.UI.Commands.Contextual.add(new Exp.UI.Command("discardSettings", "Discard", Exp.UI.CommandIconDescriptor.getWellKnown("reset"), true, null, onDiscardSettings));
                Shell.UI.Navigation.setConfirmNavigateAway("If you leave this page then your unsaved changes will be lost.");
                commandsEnabled = true;
            } else {
                Shell.UI.Navigation.removeConfirmNavigateAway();
                commandsEnabled = false;
            }
            Exp.UI.Commands.update();
        }
    }

    // Command handlers
    function onSaveSettings() {
        var progressOperation, newSettings;

        progressOperation = new Shell.UI.ProgressOperation("Updating settings...", null /* call back */, false /*isDeterministic */);
        Shell.UI.ProgressOperations.add(progressOperation);

        newSettings = $.extend(true, {}, global.CmpWapExtensionAdminExtension.Controller.getCurrentAdminSettings());
        newSettings.ResourceProviderEndpoint = null; // backend will skip the ResourceProvider update call if not specified
        newSettings.ResellerEndpoint = {
            EndpointAddress: $("#hw-endpointUrl").val(),
            Username: $("#hw-username").val(),
            Password: passwordChanged ? $("#hw-password").val() : null
        };
        newSettings.LoadBalancerIPAddress = $("#hw-loadBalancerIP").val();

        global.CmpWapExtensionAdminExtension.Controller.updateAdminSettings(newSettings)
            .done(function (data, textStatus, jqXHR) {
                progressOperation.complete("Successfully updated settings.", Shell.UI.InteractionSeverity.information);
                global.CmpWapExtensionAdminExtension.Controller.invalidateAdminSettingsCache();
                updateContextualCommands(false);
                passwordChanged = false;
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                var message = "Update settings failed.";
                global.Exp.Utilities.failProgressOperation(progressOperation, message, Exp.Utilities.getXhrError(jqXHR));
            });
    }

    function onDiscardSettings() {
        renderPage(global.CmpWapExtensionAdminExtension.Controller.getCurrentAdminSettings());
        updateContextualCommands(false);
    }

    // Public
    function loadTab(renderData, container) {
        commandsEnabled = false;

        // Intialize the local data update event handler
        global.CmpWapExtensionAdminExtension.Controller.invalidateAdminSettingsCache()
            .done(function (url, dataSet) {
                $(dataSet.data).off("propertyChange").on("propertyChange", function () {
                    renderPage(dataSet.data);
                });
                $(dataSet.data).trigger("propertyChange");
            });

        Shell.UI.Validation.setValidationContainer("#hw-settings");  // Initialize validation container for subsequent calls to Shell.UI.Validation.validateContainer.
        $("#hw-settings").on("change.fxcontrol", onSettingChanged);

        $("#hw-password").on("keyup change", function () {
            passwordChanged = true;
        });
    }

    function cleanup() {
       
    }

    global.CmpWapExtensionAdminExtension = global.CmpWapExtensionAdminExtension || {};
    global.CmpWapExtensionAdminExtension.SettingsTab = {
        loadTab: loadTab,
        cleanup: cleanup
    };
})(jQuery, this, this.Shell, this.Exp);