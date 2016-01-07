/// <reference path="CmpWapExtension.Utility.ts" />
// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

//*********************************************************************
/// 
/// <summary>
/// Actual rendering of plan UI extensions
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

    export class PlanUiExtensions {

        private _planId: string; // plan Id
        private _selectedRowAllSubscriptions: any; // selected row
        private _selectedRowPlanSubscriptions: any; // selected row for plan subscriptions table
        private  _idPrefix: string = "op-id-";
        private _planConfigSettings: Models.Configuration;
        private _planSubscriptions;
        private _azureSubscriptions;

        constructor() {
            $(".hs-empty-header").hide(); // hide default message
            $("#plan-config-body").hide();


            //Set up events to dynamically change fields reacting to ARM or ASM
            $("#subscriptionType").change( () => {
                this.toggleArmOrAsm();
            });
        }

        loadAzureSubOnboardingUi(palnId: string): any {
            //configure add button
            this._planId = palnId;
            $("#azuresub-btn-add").click(() => {
                var planId = () => {return this._planId };
                var iD = 0;
                var name = $("#name").val();
                var description = $("#description").val();
                var resourceGroup = $("#resourceGroup").val();
                var accountId = $("#accountId").val();
                var accountType = $("#accountType").val();
                var certificateThumbprint = $("#certificateThumbprint").val();
                var tenantID = $("#tenantID").val();
                var clientID = $("#clientID").val();
                var clientKey = $("#clientKey").val();

                var azOps = new AzureSubOps.AzureOps(this._planId);
                azOps.addAzureSubscriptions(iD, name, description, resourceGroup, accountId, accountType, certificateThumbprint, tenantID, clientID, clientKey)
                    .then(() => {
                            this.showSubscriptionMessage("Subscription added.", false);
                            this.loadAzureSubUi(this._planId);
                        },
                        (failMessage) => {
                            this.showSubscriptionMessage(failMessage, true);
                            this.loadAzureSubUi(this._planId);
                        }
                    );
            });
            $("#name").change(() => { $("#resourceGroup").val($("#name").val()) });
            $("#save-plan-config").click(() => {
                this.savePlanConfigSettings(this._planId ,this._planConfigSettings);
            });
            $("#azuresub-btn-add-to-plan").click(() => {
                this.addPlanSubscription(this._selectedRowAllSubscriptions);
            });
            $("#azuresub-btn-remove-from-plan").click(() => {
                this.removePlanSubscription(this._selectedRowPlanSubscriptions);
            });
            $("#plan-config-body").show();
        }

        loadAzureSubUi(planId: string) : any {
            var deferred = $.Deferred();

            (() => {
                this._planId = planId;
            })();

            // columns for grid to output azure subscriptions
            var columns = [
                { name: "ID", field: "ID", sortable: false },
                { name: "Name", field: "Name", sortable: false },
                { name: "Description", field: "Description", sortable: false },
                { name: "Resource Group", field: "ResourceGroup", sortable: false },
                { name: "Account ID", field: "AccountID", sortable: false },
                { name: "Account Type", field: "AccountType", sortable: false },
                { name: "Certificate Thumbprint", field: "CertificateThumbprint", sortable: false },
                { name: "Active", field: "Active", sortable: false }
            ];

            //get list of Azure subscriptions associated with a plan
            var azOps = new AzureSubOps.AzureOps(planId);
            azOps.getAzureSubscriptions()
                .then((value) => {
                    // add grid to output azure subscriptions
                    this._azureSubscriptions = value;
                    $("#azuresub-grid-container").fxGrid({
                        columns: columns,
                        rowSelect: (e, args) => {
                            //Bind an event when a row is selected
                            var selectedrow = null;
                            selectedrow = args.selected;
                            this._selectedRowAllSubscriptions = selectedrow;
                            $("#azuresub-btn-rem").show(); // show remove button on select
                        },
                        data: this._azureSubscriptions,
                        selectable: true,
                        multiselect: false,
                        maxHeight: 150
                    });
                    this.loadPlanConfigSettings(planId);
                    deferred.resolve(value);
                }, (error) => {
                    var util = new UtilityMethods();
                    util.generateErrorNotification(error);
                    deferred.reject(error);
                });

            azOps.getResourceGroups()
                .then((value) => {
                    var listItems = "";
                    for (var i = 0; i < value.length; i++) {
                        listItems += "<option value='" + value[i].Name + "'>" + "</option>";
                    }
                    $("#resourceGroupTxt").html(listItems);	
                },
                (error) => {
                    
                });

            return deferred.promise();
        }

        loadPlanConfigSettings(planConfigID: string) : void {
            var azOps = new AzureSubOps.AzureOps(this._planId);
            this._planConfigSettings = azOps.getPlanConfiguration(planConfigID);
            azOps.getPlanConfiguration(planConfigID).then(
                (planConfigData) => {
                    this._planConfigSettings = planConfigData;
                    //Load subscription settings
                    this.loadPlanSubscriptionSettings(this._planConfigSettings.AzureSubscriptions);
                    //Load OS settings
                    if (this._planConfigSettings.OperatingSystems.length > 0) {
                        var sectionName = "osType";
                        $("#" + sectionName).show();
                        this.loadSettingsIntoPage(sectionName, this._planConfigSettings.OperatingSystems);
                    }
                    //Load VM Sizes settings
                    if (this._planConfigSettings.VmSizes.length > 0) {
                        var sectionName = "vmSizes";
                        $("#" + sectionName).show();
                        this.loadSettingsIntoPage(sectionName, this._planConfigSettings.VmSizes);
                    }
                    //Load AzureRegion settings
                    if (this._planConfigSettings.AzureRegions.length > 0) {
                        var sectionName = "azureRegions";
                        $("#" + sectionName).show();
                        this.loadSettingsIntoPage(sectionName, this._planConfigSettings.AzureRegions);
                    }
                },
                (failMessage) => {
                    this.showPlanConfigMessage(failMessage, true);
                });
        }

        clearSettings() : void {
            $(".plan-config-setting-row").empty();
        }

        loadPlanSubscriptionSettings(planSubscriptions) {
            var columns = [
                { name: "ID", field: "ID", sortable: false },
                { name: "Name", field: "Name", sortable: false },
                { name: "Description", field: "Description", sortable: false },
                { name: "Resource Group", field: "ResourceGroup", sortable: false },
                { name: "AccountID", field: "AccountID", sortable: false },
                { name: "Account Type", field: "AccountType", sortable: false },
                { name: "Certificate Thumbprint", field: "CertificateThumbprint", sortable: false },
                { name: "Active", field: "Active", sortable: false }
            ];
            this._planSubscriptions = this.populatePlanSubscriptionObject(planSubscriptions, this._azureSubscriptions);
            $("#plansub-grid-container").fxGrid({
                columns: columns,
                rowSelect: (e, args) => {
                    //Bind an event when a row is selected
                    var selectedrow = null;
                    selectedrow = args.selected;
                    this._selectedRowPlanSubscriptions = selectedrow;
                },
                data: this._planSubscriptions,
                selectable: true,
                multiselect: false,
                maxHeight: 150
            });
            if (this._planSubscriptions.length == 0) {
                this.refeshDisplayedPlanSubscriptions();
            }
        }

        addPlanSubscription(newPlanSubscription) {
            var alreadyExists = false;
            for (var x = 0; x < this._planSubscriptions.length; x++) {
                if (this._planSubscriptions[x].ID == newPlanSubscription.dataItem.ID) {
                    this._planSubscriptions[x].IsDeleted = false;
                    alreadyExists = true;
                    break;
                }
            }
            if (!alreadyExists) {
                this._planSubscriptions.push(newPlanSubscription.dataItem);
            }
            this.refeshDisplayedPlanSubscriptions();
        }

        removePlanSubscription(removedPlanSubscription) {
            for (var x = 0; x < this._planSubscriptions.length; x++) {
                if (this._planSubscriptions[x].ID == removedPlanSubscription.dataItem.ID) {
                    this._planSubscriptions[x].IsDeleted = true;
                }
            }
            this.refeshDisplayedPlanSubscriptions();
        }

        refeshDisplayedPlanSubscriptions() {
            var nonDeletedPlanSubscriptions = this._planSubscriptions.filter(notDeletedValues => notDeletedValues.IsDeleted != true);
            if (nonDeletedPlanSubscriptions.length == 0) {
                nonDeletedPlanSubscriptions.push({ ID: "no subscriptions"});
            }
            $("#plansub-grid-container").fxGrid("option", "data", nonDeletedPlanSubscriptions);
            $("#plansub-grid-container").fxGrid("refresh");
        }

        populatePlanSubscriptionObject(planSubscriptionIds, azureSubscriptions) {
            var populatedPlanSubscriptions = [];
            for (var i = 0; i < azureSubscriptions.length; i++) {
                for (var j = 0; j < planSubscriptionIds.length; j++) {
                    if (azureSubscriptions[i].ID == planSubscriptionIds[j].Id) {
                        populatedPlanSubscriptions.push(azureSubscriptions[i]);
                        break;
                    }
                }
            }
            return populatedPlanSubscriptions;
        }

        loadSettingsIntoPage(section: string, planConfigSectionSettings: Array<Models.Setting>) : void {
            var optionsCount = planConfigSectionSettings.length;
            var section1 = "#" + section + "-options-row1";
            var section2 = "#" + section + "-options-row2";
            var section3 = "#" + section + "-options-row3";
            for (var i = 0; i < optionsCount; i++) {
                if (i % 3 == 0) {
                    $(section1).append(this.makeCheckBoxOptionHTML(section, planConfigSectionSettings[i]));
                }
                else if (i % 3 == 1) {
                    $(section2).append(this.makeCheckBoxOptionHTML(section, planConfigSectionSettings[i]));
                }
                else if (i % 3 == 2) {
                    $(section3).append(this.makeCheckBoxOptionHTML(section, planConfigSectionSettings[i]));
                }
            }
        }

        savePlanConfigSettings(planConfigID: string, planConfigSettings: Models.Configuration) : void {
            this.loadUpdatedPlanConfigSettingsIntoObject(planConfigSettings);
            var azOps = new AzureSubOps.AzureOps(this._planId);
            this.pageLoading();
            azOps.setPlanConfiguration(planConfigID, planConfigSettings).then(
                () => {
                    this.showPlanConfigMessage("Plan configuration saved", false);
                    this.pageFinishedLoading();
                },
                (failMessage) => {
                    this.showPlanConfigMessage(failMessage, true);
                    this.pageFinishedLoading();
                });
        }
        
        loadUpdatedPlanConfigSettingsIntoObject(planConfigSettings: Models.Configuration): void {
            //Load OS settings
            if (planConfigSettings.OperatingSystems.length > 0) {
                for (var i = 0; i < planConfigSettings.OperatingSystems.length; i++) {
                    var setting = planConfigSettings.OperatingSystems[i];
                    var sectionName = "osType"
                    setting.IsSelected = $("#" + this._idPrefix + sectionName + setting.Id).is(':checked');
                }
            }
            //Load VM Sizes settings
            if (planConfigSettings.VmSizes.length > 0) {
                for (var i = 0; i < planConfigSettings.VmSizes.length; i++) {
                    var setting = planConfigSettings.VmSizes[i];
                    var sectionName = "vmSizes"
                    setting.IsSelected = $("#" + this._idPrefix + sectionName + setting.Id).is(':checked');
                }
            }
            //Load AzureRegions settings
            if (planConfigSettings.AzureRegions.length > 0) {
                for (var i = 0; i < planConfigSettings.AzureRegions.length; i++) {
                    var setting = planConfigSettings.AzureRegions[i];
                    var sectionName = "azureRegions";
                    setting.IsSelected = $("#" + this._idPrefix + sectionName + setting.Id).is(':checked');
                }
            }
            //Load Subscriptions Into object
            planConfigSettings.AzureSubscriptions = [];
            for (var x = 0; x < this._planSubscriptions.length; x++) {
                var planSubscription = { Id: this._planSubscriptions[x].ID, IsSelected: (this._planSubscriptions[x].IsDeleted != true), Name: this._planSubscriptions[x].Name }
                planConfigSettings.AzureSubscriptions.push(planSubscription);
            }
        }

        makeCheckBoxOptionHTML(section: string ,option : Models.Setting) {
            var unit = $("<span class=\"unit\"></span>");
            unit.attr("class", "unit");
            var checkBox = $("<input id=\"" + this._idPrefix + section + option.Id +"\">");
            checkBox.attr("class", "storageLoggingOptions");
            checkBox.attr("aria-label", option.Name);
            checkBox.prop("type", "checkbox");
            checkBox.prop('checked', option.IsSelected);
            unit.append(checkBox);
            unit.append(" " + option.Name);
            unit.append($("<br>"));
            return unit;
        }

        pageLoading() {
            $("#plan-config-body").fadeOut(1000);
        }

        pageFinishedLoading() {
            $("#plan-config-body").fadeIn(1000);
        }

        showSubscriptionMessage(messageText: string, isAnErrorMessage: boolean) {
            this.showMessage("#subscription-message", messageText, isAnErrorMessage);
        }

        showPlanConfigMessage(messageText: string, isAnErrorMessage: boolean) {
            this.showMessage("#plan-config-message", messageText, isAnErrorMessage);
        }

        showMessage(messageContainer: string, messageText: string, isAnErrorMessage: boolean) {
            var message = this.makeMessage(messageText, isAnErrorMessage);
            $(messageContainer).empty();
            $(messageContainer).append(message);
        }

        makeMessage(messageText: string, isAnErrorMessage: boolean) {
            var message = $("<div></div>");
            var messageClass = "notif-message";
            if (isAnErrorMessage) {
                messageClass = messageClass + " notif-message-error";
            }
            message.attr("class", messageClass);
            message.text(messageText);
            return message;
        }

        toggleArmOrAsm(): void {
            var subType = $("#subscriptionType").val();
            if (subType == "ARM") {
                //alert("Debug: ARM SELECTED");
                $("#certificateThumbprint").prop("disabled", true);//.prop("data-val", false);
                //$("#certificateThumbprint").settings.ignore = "";
                $("#tenantID").prop("disabled", false);//.prop("data-val", true);
                //$("#tenantID").settings.ignore = ".ignore";
                $("#clientID").prop("disabled", false);//.prop("data-val", true);
                //$("#clientID").settings.ignore = ".ignore";
                $("#clientKey").prop("disabled", false);//.prop("data-val", true);
                //$("#clientKey").settings.ignore = ".ignore";
            }
            if (subType == "ASM") {
                //alert("Debug: ASM SELECTED");
                $("#certificateThumbprint").prop("disabled", false);//.prop("data-val", false);
                //$("#certificateThumbprint").settings.ignore = "";
                $("#tenantID").prop("disabled", true);//.prop("data-val", true);
                //$("#tenantID").settings.ignore = ".ignore";
                $("#clientID").prop("disabled", true);//.prop("data-val", true);
                //$("#clientID").settings.ignore = ".ignore";
                $("#clientKey").prop("disabled", true);//.prop("data-val", true);
                //$("#clientKey").settings.ignore = ".ignore";
            }

            //Esto esta fallando. El unobstrusive es null. Moverle.
            //$.validator.unobtrusive.parse($("#certificateThumbprint"));
            //$.validator.unobtrusive.parse($("#tenantID"));
            //$.validator.unobtrusive.parse($("#clientID"));
            //$.validator.unobtrusive.parse($("#clientKey"));

            //alert("Debug: Exiting handler");
        }
    }
} 
