/*
/// <reference path="domaintenant.controller.js" />
/// <reference path="domaintenant.domainstab.js" />
*/
/// <disable>JS2076.IdentifierIsMiscased</disable>
/*global,jQuery,trace,cdm, waz*/

(function ($, global, Shell, Exp, undefined) {
    "use strict";

    $('head').append('<meta http-equiv="Pragma" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Control" content="no-cache" />');
    $('head').append('<meta http-equiv="Pragma-directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Expires" content="-1" />');

    var commandsEnabled, tabContainer, passwordChanged = false;

    //*************************************************************************
    // Renders the page
    //*************************************************************************
    function renderPage(userInfo, container) {
    }

    //*************************************************************************
    // Called when the settings have been changed
    //*************************************************************************
    function onSettingChanged() {
        updateContextualCommands(true);
    }

    //*************************************************************************
    // Updates the application bar availability states
    //*************************************************************************
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

    //*************************************************************************
    // Saves any pending setting changes
    //*************************************************************************
    function onSaveSettings() {
    }

    //*************************************************************************
    // Discards any pending setting changes
    //*************************************************************************
    function onDiscardSettings() {
    }

    //*************************************************************************
    // Initializes the view
    //*************************************************************************
    function loadTab(renderData, container) {
        commandsEnabled = false;

        Shell.UI.Validation.setValidationContainer("#hw-settings"); // Initialize validation container for subsequent calls to Shell.UI.Validation.validateContainer.
        $("#hw-settings").on("change.fxcontrol", onSettingChanged);

        $("#hw-password").on("keyup change", function () {
            passwordChanged = true;
        });
    }

    //*************************************************************************
    // Clears out the view
    //*************************************************************************
    function cleanup() {
    }

    global.CmpWapExtensionTenantExtension = global.CmpWapExtensionTenantExtension || {};
    global.CmpWapExtensionTenantExtension.SettingsTab = {
        loadTab: loadTab,
        cleanup: cleanup
    };
})(jQuery, this, this.Shell, this.Exp);
