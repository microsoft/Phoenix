# PowerShell script to create Azure AD Application in AAD tenant for Phoenix Resource Provider.
# Copyright (c) Microsoft Corporation. All rights reserved.

# NOTE: This script requires Windows Azure Powershell 1.0 http://aka.ms/webpi-azps

#Requires -RunAsAdministrator
#Requires -Modules @{ModuleName="Azure";ModuleVersion="1.0.1"}

Param(  
	[Parameter(Mandatory=$True)]
	[string]$tenantId,
	[Parameter(Mandatory=$True)]
	[string]$subscriptionId,
	[Parameter(Mandatory=$True)]
	[string]$appKey
)
 
$spn = "https://cmpworker"
$homepage = "https://github.com/microsoft/Project-Phoenix"
$displayName = "CMP Worker"

Login-AzureRmAccount | Out-Null
$subscription = Select-AzureRmSubscription -TenantId $tenantId -SubscriptionId $subscriptionId
$cmpApp = New-AzureRmADApplication -DisplayName $displayName -HomePage $homepage -IdentifierUris $spn -Password $appKey
$cmpSPN = New-AzureRmADServicePrincipal -ApplicationId $cmpApp.ApplicationId
Start-Sleep -s 3
$cmpRole = New-AzureRmRoleAssignment -RoleDefinitionName Contributor -ServicePrincipalName $cmpApp.ApplicationId
 
Write-Output "Subscription ID: $subscriptionId"
Write-Output "Tenant ID: $tenantId"
Write-Output "App ID: $($cmpApp.ApplicationId)"
Write-Output "App Key: $appKey"
