/// <reference path="cmpwapextension.iplanserviceinterface.ts" />
/// <reference path="cmpwapextension.azuresubops.ts" />
/// <reference path="cmpwapextension.utility.ts" />
/// <reference path="CmpWapExtension.PlanUIExtensions.ts" />
// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

//*********************************************************************
///
/// <summary>
/// Service offering for plan services. This class boots up the various
/// components to display plan services.
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

    var PlanService = (function () {
        function PlanService() {
            var _this = this;
            // loading UI components
            this.initializeServiceOffer = function (serviceOffer) {
                var planUi = new PlanServices.PlanUiExtensions();
                var util = new PlanServices.UtilityMethods();

                util.showSpinner();

                planUi.loadAzureSubOnboardingUi(_this._planId);
                planUi.loadAzureSubUi(_this._planId).then(function () {
                    util.hideSpinner();
                }); // load Azure sub UI
                //TODO: load other UI components
            };
            this.executeCommand = function () {
            };
            // get plan from referrer path.
            //TODO: change below logic for more streamlined approach
            var planId = this.getPlanId(document.URL);

            if (!planId) {
                var util = new PlanServices.UtilityMethods();
                util.generateErrorNotification("Could not get plan Id");
            } else {
                (function () {
                    _this._planId = planId;
                })();
            }
            return;
        }
        // registering extension
        PlanService.prototype.initializePage = function (global) {
            global.CmpWapExtensionExtension = global.CmpWapExtensionAdminExtension || {};
            global.ServiceOffer.registerExtension({
                initializeServiceOffer: this.initializeServiceOffer,
                executeCommand: this.executeCommand()
            });
        };

        PlanService.prototype.getPlanId = function (url) {
            var planIdIndex = url.indexOf("&planId") + 8;
            return url.substring(planIdIndex);
        };
        return PlanService;
    })();
    PlanServices.PlanService = PlanService;
})(PlanServices || (PlanServices = {}));

//load page
$(document).ready(new PlanServices.PlanService().initializePage(this));
/*
(function ($, global) {
function initializePage() {
global.CmpWapExtensionExtension = global.CmpWapExtensionAdminExtension || {};
global.ServiceOffer.registerExtension({
initializeServiceOffer: new PlanServices.PlanService().initializeServiceOffer,
executeCommand: new PlanServices.PlanService().executeCommand()
});
}
$(document).ready(initializePage);
})(jQuery, this);*/
