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

module PlanServices {
    "use strict";

    $('head').append('<meta http-equiv="Pragma" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Control" content="no-cache" />');
    $('head').append('<meta http-equiv="Pragma-directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Expires" content="-1" />');

    /// Utility functions  

    export class UtilityMethods {
        private _progressOperation: any;
        public loadConfig: boolean;

  // method to kick off progression operation bar
        startProgressOperation(title: string) {
            this._progressOperation = new Shell.UI.ProgressOperation(
                title,
                null,
                true
            );
            Shell.UI.ProgressOperations.add(this._progressOperation);            
        }

        // error notification of a progress operation
        commandError(message, messageDetail) {
            this._progressOperation.complete(
                message,
                Shell.UI.InteractionSeverity.error,
                Shell.UI.InteractionBehavior.ok,
                (message ? { detailData: messageDetail } : null)
            );
        }
        
        // success notification of a progress operation
        commandComplete(data, message) {
            this._progressOperation.complete(
                message,
                Shell.UI.InteractionSeverity.information
            );
        }

        // error notification of a normal operation
        generateErrorNotification(message: string) {
            var notification = new Shell.UI.Notifications.Notification(
                message,
                Shell.UI.Notifications.Severity.error
            );
            Shell.UI.Notifications.add(notification);
        }

        // show spinner 
        showSpinner() {
            console.dir(Shell.UI);
            Shell.UI.Spinner.show();
        }

        // hide spinner
        hideSpinner() {
            Shell.UI.Spinner.hide();
        }
    }
} 