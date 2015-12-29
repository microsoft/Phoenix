
CREATE view [dbo].[vw_AzureLocalComputerDetails] as
select ID,
	   TargetVmName AS ServerName,
	   TargetVmName + '\ITSVC0' AS LocalAdminUsername
	   ,isnull(cast(Config as xml).query('*/AzureVmConfig/RoleList/Role[1]/ConfigurationSets[1]/ConfigurationSet[1]/AdminPassword').value('.','varchar(250)'),'') as LocalAdminPassword
from VmDeploymentRequests