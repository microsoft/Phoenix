/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
:r .\Data.AdDomainMap.sql
:r .\Data.Application.sql
:r .\Data.AzureAdminSubscriptionMapping.sql
:r .\Data.AzureAdminSubscriptionVmOsMapping.sql
:r .\Data.AzureRegion.sql
:r .\Data.EnvironmentType.sql
:r .\Data.IISRoleService.sql
:r .\Data.NetworkNIC.sql
:r .\Data.ResourceProviderAcctGroup.sql
:r .\Data.IpakVersionMap.sql
:r .\Data.ServerRole.sql
:r .\Data.ServerRoleDriveMapping.sql
:r .\Data.ServiceCategory.sql
:r .\Data.SQLAnalysisServicesMode.sql
:r .\Data.SQLCollation.sql
:r .\Data.SQLVersion.sql
:r .\Data.VmOs.sql
:r .\Data.VmSize.sql
:r .\Data.WapSubscriptionGroupMembership.sql