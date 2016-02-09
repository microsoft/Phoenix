// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

declare var $;
declare var global;

//*********************************************************************
/// 
/// <summary>
/// CRUD operations for Azure Subscriptions
/// </summary>
/// 
//*********************************************************************

module AzureSubOps {
    import Configuration = PlanServices.Models.Configuration;

    $('head').append('<meta http-equiv="Pragma" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Control" content="no-cache" />');
    $('head').append('<meta http-equiv="Pragma-directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Cache-Directive" content="no-cache" />');
    $('head').append('<meta http-equiv="Expires" content="-1" />');

    export class AzureOps
    {
        private _planId: string;
        private dataset: any[];
        private baseUrl: string;

        constructor(planId: string) {
            this._planId = planId;
            this.baseUrl = "/CmpWapExtensionAdmin";
        }

        getAzureSubscriptions(): any {

            var adminServiceProviderAccountsUrl = this.baseUrl + "/ServProvAccts";

            if (this.dataset == null || this.dataset.length === 0) {
                var deferred = $.Deferred();
                $.post(adminServiceProviderAccountsUrl, {})
                .done((data) => {
                    this.dataset = data.data;
                        if (this.dataset) {
                            deferred.resolve(this.dataset);
                        } else {
                            deferred.reject("Could not get Service Provider Accounts.");
                        }
                    })
                .fail((jqXhr, textStatus, errorThrown) => {
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
        }

        getPlanConfiguration(planId: string): any {
            var planConfigurationUrl = this.baseUrl + "/PlanConfiguration?planId=" + planId;
            var errorDetail = "Could not get plan configuration for " + planId;
            var deferred = $.Deferred();

            $.post(planConfigurationUrl, {})
                .done((data) => { 
                    if (data) {
                        deferred.resolve(data);
                    } else {
                        deferred.reject(errorDetail);
                    }
                })
                .fail((jqXhr) => {
                    if (jqXhr.responseText != null) {
                        errorDetail = JSON.parse(jqXhr.responseText).message;
                    }
                    deferred.reject(errorDetail);
                });

            return deferred.promise();
        }

        setPlanConfiguration(planId: string, configuration: Configuration) {
            var planConfigurationUrl = this.baseUrl + "/SetPlanConfiguration?planId=" + planId;
            var errorDetail = "Could not set plan configuration for " + planId;
            var deferred = $.Deferred();

            $.ajax({
                    url: planConfigurationUrl,
                    type: "POST",
                    data: JSON.stringify(configuration),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json"
                }).done((data) => {
                    if (data) {
                        deferred.resolve(data);
                    } else {
                        deferred.reject(errorDetail);
                    }
                }).fail((jqXhr) => {
                    if (jqXhr.responseText != null) {
                        errorDetail = JSON.parse(jqXhr.responseText).message;
                    }
                    deferred.reject(errorDetail);
                });

            return deferred;
        }

        getResourceGroups(): any {
            var resourceGroupUrl = this.baseUrl + "/ResourceGroups";
            var deferred = $.Deferred();
            $.post(resourceGroupUrl, {})
            .done((data) => {
                if (data.data) {
                    deferred.resolve(data.data);
                } else {
                    deferred.reject("Could not get Resource Groups.");
                }
            })
            .fail((jqXhr, textStatus, errorThrown) => {
                if (jqXhr.responseText != null) {
                    var messageDetail = JSON.parse(jqXhr.responseText).message;
                    deferred.reject(messageDetail);
                } else {
                    deferred.reject("Could not get Resource Groups.");
                }

            });
            return deferred.promise();
        }
        
        addAzureSubscriptions(id, name, description, resourceGroup, accountId, accountType, certificateThumbprint, tenantID, clientID, clientKey) {
            var deferred = $.Deferred();
            var adminUpdateServiceProviderAccountUrl = this.baseUrl + "/UpdateServiceProviderAccount";
            
            if (name !== "" 
                && description !== "" 
                && resourceGroup !== "" 
                && accountType !== "" 
                //&& certificateThumbprint !== ""
                ) {
                $.post(adminUpdateServiceProviderAccountUrl,
                    {
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
                    }).done(() => {
                        //new PlanServices.PlanUiExtensions().loadAzureSubUi(this._planId);
                        deferred.resolve();
                    }).fail((jqXhr, textStatus, errorThrown) => {
                        deferred.reject("Could not add Azure Subscription. Please double-check your AAD creds and your web config for the CMP service. Error:");
                    });
            }
            else {
                deferred.reject("Required field missing.");
            }

            return deferred.promise();
        }

        deleteAzureSubscriptions(id: string) {
            //stub todo
        }

        updatePlanConfiguration(planId: string, configuration: Configuration) {
            var planConfigurationUrl = this.baseUrl + "/UpdatePlanConfiguration/" + planId;
            var errorDetail = "Could not update plan configuration for " + planId;
            var deferred = $.Deferred();

            $.post(planConfigurationUrl, configuration)
                .done((data) => {
                if (data.data) {
                    deferred.resolve(data.data);
                } else {
                    deferred.reject(errorDetail);
                }
            })
                .fail((jqXhr) => {
                if (jqXhr.responseText != null) {
                    errorDetail = JSON.parse(jqXhr.responseText).message;
                }
                deferred.reject(errorDetail);
            });
        }
    }
} 