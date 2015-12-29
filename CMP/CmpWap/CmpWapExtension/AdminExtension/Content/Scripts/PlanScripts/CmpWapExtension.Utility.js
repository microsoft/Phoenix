// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------
//*********************************************************************
///
/// <summary>
/// Utility functions
/// </summary>
///
//*********************************************************************
var PlanServices;
(function (PlanServices) {
    "use strict";

    $('head').append('<meta http-equiv="Pragma" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Control" content="no-cache" />');
    $('head').append('<meta http-equiv="Pragma-directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Expires" content="-1" />');

    /// Utility functions
    var UtilityMethods = (function () {
        function UtilityMethods() {
        }
        // method to kick off progression operation bar
        UtilityMethods.prototype.startProgressOperation = function (title) {
            this._progressOperation = new Shell.UI.ProgressOperation(title, null, true);
            Shell.UI.ProgressOperations.add(this._progressOperation);
        };

        // error notification of a progress operation
        UtilityMethods.prototype.commandError = function (message, messageDetail) {
            this._progressOperation.complete(message, Shell.UI.InteractionSeverity.error, Shell.UI.InteractionBehavior.ok, (message ? { detailData: messageDetail } : null));
        };

        // success notification of a progress operation
        UtilityMethods.prototype.commandComplete = function (data, message) {
            this._progressOperation.complete(message, Shell.UI.InteractionSeverity.information);
        };

        // error notification of a normal operation
        UtilityMethods.prototype.generateErrorNotification = function (message) {
            var notification = new Shell.UI.Notifications.Notification(message, Shell.UI.Notifications.Severity.error);
            Shell.UI.Notifications.add(notification);
        };

        // show spinner
        UtilityMethods.prototype.showSpinner = function () {
            console.dir(Shell.UI);
            Shell.UI.Spinner.show();
        };

        // hide spinner
        UtilityMethods.prototype.hideSpinner = function () {
            Shell.UI.Spinner.hide();
        };
        return UtilityMethods;
    })();
    PlanServices.UtilityMethods = UtilityMethods;
})(PlanServices || (PlanServices = {}));
