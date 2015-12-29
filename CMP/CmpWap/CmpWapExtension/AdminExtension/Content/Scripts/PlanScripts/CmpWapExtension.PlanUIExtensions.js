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
var PlanServices;
(function (PlanServices) {
    "use strict";

    $('head').append('<meta http-equiv="Pragma" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Control" content="no-cache" />');
    $('head').append('<meta http-equiv="Pragma-directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Expires" content="-1" />');

    var PlanUiExtensions = (function () {
        function PlanUiExtensions() {
            var _this = this;
            this._idPrefix = "op-id-";
            $(".hs-empty-header").hide(); // hide default message
            $("#plan-config-body").hide();

            //Set up events to dynamically change fields reacting to ARM or ASM
            $("#subscriptionType").change(function () {
                _this.toggleArmOrAsm();
            });
        }
        PlanUiExtensions.prototype.loadAzureSubOnboardingUi = function (palnId) {
            var _this = this;
            //configure add button
            this._planId = palnId;
            $("#azuresub-btn-add").click(function () {
                var planId = function () {
                    return _this._planId;
                };
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

                var azOps = new AzureSubOps.AzureOps(_this._planId);
                azOps.addAzureSubscriptions(iD, name, description, resourceGroup, accountId, accountType, certificateThumbprint, tenantID, clientID, clientKey).then(function () {
                    _this.showSubscriptionMessage("Subscription added.", false);
                    _this.loadAzureSubUi(_this._planId);
                }, function (failMessage) {
                    _this.showSubscriptionMessage(failMessage, true);
                    _this.loadAzureSubUi(_this._planId);
                });
            });
            $("#name").change(function () {
                $("#resourceGroup").val($("#name").val());
            });
            $("#save-plan-config").click(function () {
                _this.savePlanConfigSettings(_this._planId, _this._planConfigSettings);
            });
            $("#azuresub-btn-add-to-plan").click(function () {
                _this.addPlanSubscription(_this._selectedRowAllSubscriptions);
            });
            $("#azuresub-btn-remove-from-plan").click(function () {
                _this.removePlanSubscription(_this._selectedRowPlanSubscriptions);
            });
            $("#plan-config-body").show();
        };

        PlanUiExtensions.prototype.loadAzureSubUi = function (planId) {
            var _this = this;
            var deferred = $.Deferred();

            (function () {
                _this._planId = planId;
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
            azOps.getAzureSubscriptions().then(function (value) {
                // add grid to output azure subscriptions
                _this._azureSubscriptions = value;
                $("#azuresub-grid-container").fxGrid({
                    columns: columns,
                    rowSelect: function (e, args) {
                        //Bind an event when a row is selected
                        var selectedrow = null;
                        selectedrow = args.selected;
                        _this._selectedRowAllSubscriptions = selectedrow;
                        $("#azuresub-btn-rem").show(); // show remove button on select
                    },
                    data: _this._azureSubscriptions,
                    selectable: true,
                    multiselect: false,
                    maxHeight: 150
                });
                _this.loadPlanConfigSettings(planId);
                deferred.resolve(value);
            }, function (error) {
                var util = new PlanServices.UtilityMethods();
                util.generateErrorNotification(error);
                deferred.reject(error);
            });

            azOps.getResourceGroups().then(function (value) {
                var listItems = "";
                for (var i = 0; i < value.length; i++) {
                    listItems += "<option value='" + value[i].Name + "'>" + "</option>";
                }
                $("#resourceGroupTxt").html(listItems);
            }, function (error) {
            });

            return deferred.promise();
        };

        PlanUiExtensions.prototype.loadPlanConfigSettings = function (planConfigID) {
            var _this = this;
            var azOps = new AzureSubOps.AzureOps(this._planId);
            this._planConfigSettings = azOps.getPlanConfiguration(planConfigID);
            azOps.getPlanConfiguration(planConfigID).then(function (planConfigData) {
                _this._planConfigSettings = planConfigData;

                //Load subscription settings
                _this.loadPlanSubscriptionSettings(_this._planConfigSettings.AzureSubscriptions);

                //Load OS settings
                if (_this._planConfigSettings.OperatingSystems.length > 0) {
                    var sectionName = "osType";
                    $("#" + sectionName).show();
                    _this.loadSettingsIntoPage(sectionName, _this._planConfigSettings.OperatingSystems);
                }

                //Load VM Sizes settings
                if (_this._planConfigSettings.VmSizes.length > 0) {
                    var sectionName = "vmSizes";
                    $("#" + sectionName).show();
                    _this.loadSettingsIntoPage(sectionName, _this._planConfigSettings.VmSizes);
                }

                //Load AzureRegion settings
                if (_this._planConfigSettings.AzureRegions.length > 0) {
                    var sectionName = "azureRegions";
                    $("#" + sectionName).show();
                    _this.loadSettingsIntoPage(sectionName, _this._planConfigSettings.AzureRegions);
                }
            }, function (failMessage) {
                _this.showPlanConfigMessage(failMessage, true);
            });
        };

        PlanUiExtensions.prototype.clearSettings = function () {
            $(".plan-config-setting-row").empty();
        };

        PlanUiExtensions.prototype.loadPlanSubscriptionSettings = function (planSubscriptions) {
            var _this = this;
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
                rowSelect: function (e, args) {
                    //Bind an event when a row is selected
                    var selectedrow = null;
                    selectedrow = args.selected;
                    _this._selectedRowPlanSubscriptions = selectedrow;
                },
                data: this._planSubscriptions,
                selectable: true,
                multiselect: false,
                maxHeight: 150
            });
            if (this._planSubscriptions.length == 0) {
                this.refeshDisplayedPlanSubscriptions();
            }
        };

        PlanUiExtensions.prototype.addPlanSubscription = function (newPlanSubscription) {
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
        };

        PlanUiExtensions.prototype.removePlanSubscription = function (removedPlanSubscription) {
            for (var x = 0; x < this._planSubscriptions.length; x++) {
                if (this._planSubscriptions[x].ID == removedPlanSubscription.dataItem.ID) {
                    this._planSubscriptions[x].IsDeleted = true;
                }
            }
            this.refeshDisplayedPlanSubscriptions();
        };

        PlanUiExtensions.prototype.refeshDisplayedPlanSubscriptions = function () {
            var nonDeletedPlanSubscriptions = this._planSubscriptions.filter(function (notDeletedValues) {
                return notDeletedValues.IsDeleted != true;
            });
            if (nonDeletedPlanSubscriptions.length == 0) {
                nonDeletedPlanSubscriptions.push({ ID: "no subscriptions" });
            }
            $("#plansub-grid-container").fxGrid("option", "data", nonDeletedPlanSubscriptions);
            $("#plansub-grid-container").fxGrid("refresh");
        };

        PlanUiExtensions.prototype.populatePlanSubscriptionObject = function (planSubscriptionIds, azureSubscriptions) {
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
        };

        PlanUiExtensions.prototype.loadSettingsIntoPage = function (section, planConfigSectionSettings) {
            var optionsCount = planConfigSectionSettings.length;
            var section1 = "#" + section + "-options-row1";
            var section2 = "#" + section + "-options-row2";
            var section3 = "#" + section + "-options-row3";
            for (var i = 0; i < optionsCount; i++) {
                if (i % 3 == 0) {
                    $(section1).append(this.makeCheckBoxOptionHTML(section, planConfigSectionSettings[i]));
                } else if (i % 3 == 1) {
                    $(section2).append(this.makeCheckBoxOptionHTML(section, planConfigSectionSettings[i]));
                } else if (i % 3 == 2) {
                    $(section3).append(this.makeCheckBoxOptionHTML(section, planConfigSectionSettings[i]));
                }
            }
        };

        PlanUiExtensions.prototype.savePlanConfigSettings = function (planConfigID, planConfigSettings) {
            var _this = this;
            this.loadUpdatedPlanConfigSettingsIntoObject(planConfigSettings);
            var azOps = new AzureSubOps.AzureOps(this._planId);
            this.pageLoading();
            azOps.setPlanConfiguration(planConfigID, planConfigSettings).then(function () {
                _this.showPlanConfigMessage("Plan configuration saved", false);
                _this.pageFinishedLoading();
            }, function (failMessage) {
                _this.showPlanConfigMessage(failMessage, true);
                _this.pageFinishedLoading();
            });
        };

        PlanUiExtensions.prototype.loadUpdatedPlanConfigSettingsIntoObject = function (planConfigSettings) {
            //Load OS settings
            if (planConfigSettings.OperatingSystems.length > 0) {
                for (var i = 0; i < planConfigSettings.OperatingSystems.length; i++) {
                    var setting = planConfigSettings.OperatingSystems[i];
                    var sectionName = "osType";
                    setting.IsSelected = $("#" + this._idPrefix + sectionName + setting.Id).is(':checked');
                }
            }

            //Load VM Sizes settings
            if (planConfigSettings.VmSizes.length > 0) {
                for (var i = 0; i < planConfigSettings.VmSizes.length; i++) {
                    var setting = planConfigSettings.VmSizes[i];
                    var sectionName = "vmSizes";
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
                var planSubscription = { Id: this._planSubscriptions[x].ID, IsSelected: (this._planSubscriptions[x].IsDeleted != true), Name: this._planSubscriptions[x].Name };
                planConfigSettings.AzureSubscriptions.push(planSubscription);
            }
        };

        PlanUiExtensions.prototype.makeCheckBoxOptionHTML = function (section, option) {
            var unit = $("<span class=\"unit\"></span>");
            unit.attr("class", "unit");
            var checkBox = $("<input id=\"" + this._idPrefix + section + option.Id + "\">");
            checkBox.attr("class", "storageLoggingOptions");
            checkBox.attr("aria-label", option.Name);
            checkBox.prop("type", "checkbox");
            checkBox.prop('checked', option.IsSelected);
            unit.append(checkBox);
            unit.append(" " + option.Name);
            unit.append($("<br>"));
            return unit;
        };

        PlanUiExtensions.prototype.pageLoading = function () {
            $("#plan-config-body").fadeOut(1000);
        };

        PlanUiExtensions.prototype.pageFinishedLoading = function () {
            $("#plan-config-body").fadeIn(1000);
        };

        PlanUiExtensions.prototype.showSubscriptionMessage = function (messageText, isAnErrorMessage) {
            this.showMessage("#subscription-message", messageText, isAnErrorMessage);
        };

        PlanUiExtensions.prototype.showPlanConfigMessage = function (messageText, isAnErrorMessage) {
            this.showMessage("#plan-config-message", messageText, isAnErrorMessage);
        };

        PlanUiExtensions.prototype.showMessage = function (messageContainer, messageText, isAnErrorMessage) {
            var message = this.makeMessage(messageText, isAnErrorMessage);
            $(messageContainer).empty();
            $(messageContainer).append(message);
        };

        PlanUiExtensions.prototype.makeMessage = function (messageText, isAnErrorMessage) {
            var message = $("<div></div>");
            var messageClass = "notif-message";
            if (isAnErrorMessage) {
                messageClass = messageClass + " notif-message-error";
            }
            message.attr("class", messageClass);
            message.text(messageText);
            return message;
        };

        PlanUiExtensions.prototype.toggleArmOrAsm = function () {
            var subType = $("#subscriptionType").val();
            if (subType == "ARM") {
                $("#certificateThumbprint").prop("disabled", true); //.prop("data-val", false);
                $("#certificateThumbprint").settings.ignore = "";
                $("#tenantID").prop("disabled", false); //.prop("data-val", true);
                $("#tenantID").settings.ignore = ".ignore";
                $("#clientID").prop("disabled", false); //.prop("data-val", true);
                $("#clientID").settings.ignore = ".ignore";
                $("#clientKey").prop("disabled", false); //.prop("data-val", true);
                $("#clientKey").settings.ignore = ".ignore";
            }
            if (subType == "ASM") {
                $("#certificateThumbprint").prop("disabled", false); //.prop("data-val", false);
                $("#certificateThumbprint").settings.ignore = "";
                $("#tenantID").prop("disabled", true); //.prop("data-val", true);
                $("#tenantID").settings.ignore = ".ignore";
                $("#clientID").prop("disabled", true); //.prop("data-val", true);
                $("#clientID").settings.ignore = ".ignore";
                $("#clientKey").prop("disabled", true); //.prop("data-val", true);
                $("#clientKey").settings.ignore = ".ignore";
            }
            $.validator.unobtrusive.parse($("#certificateThumbprint"));
            $.validator.unobtrusive.parse($("#tenantID"));
            $.validator.unobtrusive.parse($("#clientID"));
            $.validator.unobtrusive.parse($("#clientKey"));
        };
        return PlanUiExtensions;
    })();
    PlanServices.PlanUiExtensions = PlanUiExtensions;
})(PlanServices || (PlanServices = {}));
