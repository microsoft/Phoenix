using System;
using System.Threading.Tasks;
using CmpInterfaceModel;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AzureAdminClientLib
{
    public class AzureActiveDir
    {
        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="tenantId"></param>
        ///  <param name="clientId"></param>
        ///  <param name="clientKey"></param>
        ///  <returns></returns>
        ///  
        //*********************************************************************
        public static async Task<AuthenticationResult> GetAdUserToken(
            string tenantId, string clientId, string clientKey)
        {
            try
            {
                var authenticationContext =
                    new AuthenticationContext(
                        "https://login.windows.net/" + tenantId);

                var credential =
                    new ClientCredential(clientId, clientKey);

                return await authenticationContext.AcquireTokenAsync("https://management.core.windows.net/", credential);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in AzureActiveDir:GetAdUserToken() : " +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }
    }
}

/*** 
 
https://azure.microsoft.com/en-us/documentation/articles/powershell-azure-resource-manager/
https://msdn.microsoft.com/en-us/library/azure/dn790557.aspx
https://azure.microsoft.com/en-us/documentation/articles/resource-group-authenticate-service-principal/
https://azure.microsoft.com/en-us/documentation/articles/role-based-access-control-powershell/
 
*** Sample powershell session to create an AD app and get creds for ARM access ***
 
PS C:\> Add-AzureRmAccount
 
PS C:\> $azureAdApplication = New-AzureRmADApplication -DisplayName "ArmAccessNApp" -HomePage "https://ArmAccessNApp" -IdentifierUris "https://YArmAccessNApp" -Password "xxx"
PS C:\> $azureAdApplication
 
Type                    : Application
ApplicationId           : 602f5a0c-323b-4bc6-81e7-e0e4a513cb52
ApplicationObjectId     : e5a951b4-7da1-446f-a834-c3f5e2a2d071
AvailableToOtherTenants : False
AppPermissions          : {{
                            "claimValue": "user_impersonation",
                            "description": "Allow the application to access ArmAccessNApp on behalf of the signed-in
                          user.",
                            "directAccessGrantTypes": [],
                            "displayName": "Access ArmAccessNApp",
                            "impersonationAccessGrantTypes": [
                              {
                                "impersonated": "User",
                                "impersonator": "Application"
                              }
                            ],
                            "isDisabled": false,
                            "origin": "Application",
                            "permissionId": "a6adc8b3-4890-4ff0-b1ac-57c511caff34",
                            "resourceScopeType": "Personal",
                            "userConsentDescription": "Allow the application to access ArmAccessNApp on your behalf.",
                            "userConsentDisplayName": "Access ArmAccessNApp",
                            "lang": null
                          }}
 
PS C:\> New-AzureRmADServicePrincipal -ApplicationId $azureAdApplication.ApplicationId
 
DisplayName                    Type                           ObjectId
-----------                    ----                           --------
ArmAccessNApp                                                 bd265677-f8ce-4a10-a415-33ac60dab3cb
 
PS C:\> New-AzureRmRoleAssignment -RoleDefinitionName Owner -ServicePrincipalName $azureAdApplication.ApplicationId
 
ServicePrincipalName : https://ArmAccessNApp
RoleAssignmentId     : /subscriptions/00f885db-ef1d-4545-ad2d-64c0caf93384/providers/Microsoft.Authorization/roleAssign
                       ments/ab41a3b6-6e8d-4253-89ec-e7214901eab3
DisplayName          : ArmAccessNApp
RoleDefinitionName   : Reader
Actions              : {*read}
NotActions           : {}
Scope                : /subscriptions/00f885db-ef1d-4545-ad2d-64c0caf93384
ObjectId             : bd265677-f8ce-4a10-a415-33ac60dab3cb
 
PS C:\> $subscription = Get-AzureRmSubscription | where { $_.IsCurrent }
PS C:\> $creds = Get-Credential
 
cmdlet Get-Credential at command pipeline position 1
Supply values for the following parameters:
Credential
PS C:\> Add-AzureRmAccount -Credential $creds -ServicePrincipal -Tenant $subscription.TenantId
VERBOSE: Account "602f5a0c-323b-4bc6-81e7-e0e4a513cb52" has been added.
VERBOSE: Subscription "Cloud Mgmt Platform Test Public" is selected as the default subscription.
VERBOSE: To view all the subscriptions, please use Get-AzureSubscription.
VERBOSE: To switch to a different subscription, please use Select-AzureSubscription.
 
Id                             Type       Subscriptions                          Tenants
--                             ----       -------------                          -------
602f5a0c-323b-4bc6-81e7-e0e4a5 ServicePri 00f885db-ef1d-4545-ad2d-64c0caf93384   72f988bf-86f1-41af-91ab-2d7cd011db47
13cb52                         ncipal
 
*** Get values from the from the last line above ***
 
var tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
var clientId = "602f5a0c-323b-4bc6-81e7-e0e4a513cb52";
var clientKey = "xxx";
 
*** then call the GetAdUserToken method like this ***
 
var adToken = AzureActiveDir.GetAdUserToken(tenantId, clientId, clientKey);
 
 
*/
