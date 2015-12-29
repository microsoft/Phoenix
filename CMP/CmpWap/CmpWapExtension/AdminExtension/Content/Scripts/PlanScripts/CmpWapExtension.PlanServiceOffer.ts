/// <reference path="cmpwapextension.iplanserviceinterface.ts" />
/// <reference path="cmpwapextension.azuresubops.ts" />
/// <reference path="cmpwapextension.utility.ts" />
/// <reference path="CmpWapExtension.PlanUIExtensions.ts" />
// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

declare var $;
declare var global;
declare var jQuery;
declare var Shell;

//*********************************************************************
/// 
/// <summary>
/// Service offering for plan services. This class boots up the various 
/// components to display plan services.
/// </summary>
/// 
//*********************************************************************

module PlanServices{
    "use strict";
    
    $('head').append('<meta http-equiv="Pragma" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Control" content="no-cache" />');
    $('head').append('<meta http-equiv="Pragma-directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Expires" content="-1" />');

    export class PlanService implements PlanServices.IPlanServiceInterface {
        private _planId: string;

        constructor() {
            // get plan from referrer path.
            //TODO: change below logic for more streamlined approach
            var planId = this.getPlanId(document.URL);

            if (!planId) { // if can't get plan then generate error
                var util = new UtilityMethods();
                util.generateErrorNotification("Could not get plan Id");
            }
            else
            {
                (() => {
                    this._planId = planId;                    
                })();
            }
            return;
        }

        // loading UI components
        initializeServiceOffer = (serviceOffer) => {

            var planUi = new PlanUiExtensions();
            var util = new PlanServices.UtilityMethods();

            util.showSpinner();

            planUi.loadAzureSubOnboardingUi(this._planId);                
            planUi.loadAzureSubUi(this._planId).then(() => {
                util.hideSpinner();
            }); // load Azure sub UI

            //TODO: load other UI components
        }

        executeCommand = () => {}

        // registering extension 
        initializePage(global) {
            global.CmpWapExtensionExtension = global.CmpWapExtensionAdminExtension || {};
            global.ServiceOffer.registerExtension({
                initializeServiceOffer: this.initializeServiceOffer,
                executeCommand: this.executeCommand()
            });            
        }

        getPlanId (url: string) : string {
            var planIdIndex = url.indexOf("&planId") + 8; 
            return url.substring(planIdIndex);
        }   

    }
}

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

