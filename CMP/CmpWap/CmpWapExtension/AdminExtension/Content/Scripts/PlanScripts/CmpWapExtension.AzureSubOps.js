// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

//*********************************************************************
///
/// <summary>
/// CRUD operations for Azure Subscriptions
/// </summary>
///
//*********************************************************************
var AzureSubOps;
(function (AzureSubOps) {
    $('head').append('<meta http-equiv="Pragma" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Control" content="no-cache" />');
    $('head').append('<meta http-equiv="Pragma-directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Expires" content="-1" />');

    var AzureOps = (function () {
        function AzureOps(planId) {
            this._planId = planId;
            this.baseUrl = "/CmpWapExtensionAdmin";
        }
        AzureOps.prototype.getAzureSubscriptions = function () {
            var _this = this;
            var adminServiceProviderAccountsUrl = this.baseUrl + "/ServProvAccts";

            if (this.dataset == null || this.dataset.length === 0) {
                var deferred = $.Deferred();
                $.post(adminServiceProviderAccountsUrl, {}).done(function (data) {
                    _this.dataset = data.data;
                    if (_this.dataset) {
                        deferred.resolve(_this.dataset);
                    } else {
                        deferred.reject("Could not get Service Provider Accounts.");
                    }
                }).fail(function (jqXhr, textStatus, errorThrown) {
                    if (jqXhr.responseText != null) {
                        var messageDetail = JSON.parse(jqXhr.responseText).message;
                        deferred.reject(messageDetail);
                    } else {
                        deferred.reject("Could not get Service Provider Accounts.");
                    }
                });
                return deferred.promise();
            } else {
                return this.dataset;
            }
        };

        AzureOps.prototype.getPlanConfiguration = function (planId) {
            var planConfigurationUrl = this.baseUrl + "/PlanConfiguration?planId=" + planId;
            var errorDetail = "Could not get plan configuration for " + planId;
            var deferred = $.Deferred();

            $.post(planConfigurationUrl, {}).done(function (data) {
                if (data) {
                    deferred.resolve(data);
                } else {
                    deferred.reject(errorDetail);
                }
            }).fail(function (jqXhr) {
                if (jqXhr.responseText != null) {
                    errorDetail = JSON.parse(jqXhr.responseText).message;
                }
                deferred.reject(errorDetail);
            });

            return deferred.promise();
        };

        AzureOps.prototype.setPlanConfiguration = function (planId, configuration) {
            var planConfigurationUrl = this.baseUrl + "/SetPlanConfiguration?planId=" + planId;
            var errorDetail = "Could not set plan configuration for " + planId;
            var deferred = $.Deferred();

            $.ajax({
                url: planConfigurationUrl,
                type: "POST",
                data: JSON.stringify(configuration),
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            }).done(function (data) {
                if (data) {
                    deferred.resolve(data);
                } else {
                    deferred.reject(errorDetail);
                }
            }).fail(function (jqXhr) {
                if (jqXhr.responseText != null) {
                    errorDetail = JSON.parse(jqXhr.responseText).message;
                }
                deferred.reject(errorDetail);
            });

            return deferred;
        };

        AzureOps.prototype.getResourceGroups = function () {
            var resourceGroupUrl = this.baseUrl + "/ResourceGroups";
            var deferred = $.Deferred();
            $.post(resourceGroupUrl, {}).done(function (data) {
                if (data.data) {
                    deferred.resolve(data.data);
                } else {
                    deferred.reject("Could not get Resource Groups.");
                }
            }).fail(function (jqXhr, textStatus, errorThrown) {
                if (jqXhr.responseText != null) {
                    var messageDetail = JSON.parse(jqXhr.responseText).message;
                    deferred.reject(messageDetail);
                } else {
                    deferred.reject("Could not get Resource Groups.");
                }
            });
            return deferred.promise();
        };

        AzureOps.prototype.addAzureSubscriptions = function (id, name, description, resourceGroup, accountId, accountType, certificateThumbprint, tenantID, clientID, clientKey) {
            var deferred = $.Deferred();
            var adminUpdateServiceProviderAccountUrl = this.baseUrl + "/UpdateServiceProviderAccount";

            if (name !== "" && description !== "" && resourceGroup !== "" && accountType !== "" && certificateThumbprint !== "") {
                $.post(adminUpdateServiceProviderAccountUrl, {
                    ID: id,
                    Name: name,
                    Description: description,
                    ResourceGroup: resourceGroup,
                    AccountID: accountId,
                    AccountType: accountType,
                    CertificateThumbprint: certificateThumbprint,
                    TenantID: tenantID,
                    ClientID: clientID,
                    ClientKey: clientKey
                }).done(function () {
                    //new PlanServices.PlanUiExtensions().loadAzureSubUi(this._planId);
                    deferred.resolve();
                }).fail(function (jqXhr, textStatus, errorThrown) {
                    deferred.reject("Could not add azure subscription");
                });
            } else {
                deferred.reject("Required field missing.");
            }

            return deferred.promise();
        };

        AzureOps.prototype.deleteAzureSubscriptions = function (id) {
            //stub todo
        };

        AzureOps.prototype.updatePlanConfiguration = function (planId, configuration) {
            var planConfigurationUrl = this.baseUrl + "/UpdatePlanConfiguration/" + planId;
            var errorDetail = "Could not update plan configuration for " + planId;
            var deferred = $.Deferred();

            $.post(planConfigurationUrl, configuration).done(function (data) {
                if (data.data) {
                    deferred.resolve(data.data);
                } else {
                    deferred.reject(errorDetail);
                }
            }).fail(function (jqXhr) {
                if (jqXhr.responseText != null) {
                    errorDetail = JSON.parse(jqXhr.responseText).message;
                }
                deferred.reject(errorDetail);
            });
        };
        return AzureOps;
    })();
    AzureSubOps.AzureOps = AzureOps;
})(AzureSubOps || (AzureSubOps = {}));
