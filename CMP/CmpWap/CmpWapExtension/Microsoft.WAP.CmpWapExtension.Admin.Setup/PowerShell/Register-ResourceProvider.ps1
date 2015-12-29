# PowerShell script to register Windows Azure Pack resource provider.
# Copyright (c) Microsoft Corporation. All rights reserved.

# NOTE: This script is designed to run on a machine where MgmtSvc-AdminAPI is installed.
# The *-MgmtSvcResourceProviderConfiguration cmdlets resolve the connection string and encryption key parameters from the web.config of the MgmtSvc-AdminAPI web site.

Param(  
	[Parameter(Mandatory=$True)]
   [string]$AdminMachineHostName,
   [Parameter(Mandatory=$True)]
   [string]$TenantMachineHostName
)


$rpName = 'CmpWapExtension'

Write-Host -ForegroundColor Green "Get existing resource provider '$rpName'..."
$rp = Get-MgmtSvcResourceProviderConfiguration -Name $rpName
if ($rp -ne $null)
{
    Write-Host -ForegroundColor Green "Remove existing resource provider '$rpName' $($rp.InstanceId)..."
    $rp = Remove-MgmtSvcResourceProviderConfiguration -Name $rpName -InstanceId $rp.InstanceId
}
else
{
    Write-Host -ForegroundColor Green "Resource provider '$rpName' not found."
}

$hostName = "$AdminMachineHostName" + ":30666"
$userName = "admin"
$password = "pass@word1"
$TenantHostName = $TenantMachineHostName + ":30666/"

$rpSettings = @{
    'Name' = $rpName;
    'DisplayName' = 'Cmp Wap Extension';
    'InstanceDisplayName' = 'Cmp Wap Extension';
    'AdminForwardingAddress' = "https://" + $hostName + "/admin";
    'AdminAuthenticationMode' = 'Basic';
    'AdminAuthenticationUserName' = $userName;
    'AdminAuthenticationPassword' = $password; 	
    'TenantForwardingAddress' = "https://" + $TenantHostName;	
    'TenantAuthenticationMode' = 'Basic';
    'TenantAuthenticationUserName' = $userName;
    'TenantAuthenticationPassword' = $password;
    'TenantSourceUriTemplate' = '{subid}/services/CmpWapExtension/{*path}';
    'TenantTargetUriTemplate' = 'subscriptions/{subid}/{*path}';	
    'NotificationForwardingAddress' = "https://" + $hostName + "/admin";
    'NotificationAuthenticationMode' = 'Basic';
    'NotificationAuthenticationUserName' = $userName;
    'NotificationAuthenticationPassword' = $password;
}

Write-Host -ForegroundColor Green "Create new resource provider '$rpName'..."
$rp = New-MgmtSvcResourceProviderConfiguration @rpSettings
Write-Host -ForegroundColor Green "Created new resource provider '$rpName'."

Write-Host -ForegroundColor Green "Add new resource provider '$rpName'..."
$rp = Add-MgmtSvcResourceProviderConfiguration -ResourceProvider $rp
Write-Host -ForegroundColor Green "Added new resource provider '$rpName'."

Write-Host -ForegroundColor Green "Get existing resource provider '$rpName' as Xml..."
 Get-MgmtSvcResourceProviderConfiguration -Name $rpName -as XmlString
