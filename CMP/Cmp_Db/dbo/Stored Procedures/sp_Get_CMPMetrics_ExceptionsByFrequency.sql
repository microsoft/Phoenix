


-- =============================================
-- Author:		<Author: Ken Hill>
-- Create date: <Create Date: 4/29/2015>
-- Description:	<Description: Exceptions By Frequency>
-- =============================================
CREATE PROCEDURE [dbo].[sp_Get_CMPMetrics_ExceptionsByFrequency]
	-- Add the parameters for the stored procedure here
	@BeginDate date,
	@EndDate date
AS
BEGIN
SELECT DISTINCT[Message] as 'Cause'
      ,Count([Message]) as 'Count'
  FROM 
     (SELECT 
	     CASE
		    WHEN [Message] LIKE '%IPAK IIS Installation%' AND [Message] LIKE '%Access is denied%' THEN 'Exception in RunningSequences() : Exception in CheckSequencesStatus() : Breaking error in sequence: ''IPAK IIS Installation'' in state: ''RunningSequences'' Message: ''Connecting to remote server xxx failed with the following error message : Access is denied.'''
			WHEN [Message] LIKE '%has not been synchronized with Azure%' THEN 'Exception in ProcessQcVmRequestPassed() : Exception in CheckServiceNameAvailability() : Exception in GetLeastUsedContainer() : Exception in ContainerSpaceInfo.GetContainerSpaces (guid): _containerSpaceList has not been synchronized with Azure'
			WHEN [Message] LIKE '%IPAK SQLInstallation%' AND [Message] LIKE '%Access is denied%' THEN 'Exception in RunningSequences() : Exception in CheckSequencesStatus() : Breaking error in sequence: ''IPAK SQL Installation'' in state: ''RunningSequences'' Message: ''Connecting to remote server xxx failed with the following error message : Access is denied.'''
			WHEN [Message] LIKE '%Unable to locate VM%' THEN 'Exception in ProcessCreateVm() : Exception in DeleteVm() : Exception in VmOps.Delete() : Unable to locate VM ''xxxxxxxxxxxx'''
			WHEN [Message] LIKE '%IPAK IIS Installation%' AND [Message] LIKE '%WinRM cannot%' THEN 'Exception in RunningSequences() : Exception in CheckSequencesStatus() : Breaking error in sequence: ''IPAK IIS Installation'' in state: ''RunningSequences'' Message: ''Connecting to remote server xxxxxxxxxxxxx failed with the following error message : WinRM cannot complete the operation. Verify that the specified computer name is valid, that the computer is accessible over the network, and that a firewall exception for the WinRM service is enabled and allows access from this computer.'''
			WHEN [Message] LIKE '%IPAK SQL Installation%' AND [Message] LIKE '%WinRM cannot%' THEN 'Exception in RunningSequences() : Exception in CheckSequencesStatus() : Breaking error in sequence: ''IPAK SQL Installation'' in state: ''RunningSequences'' Message: ''Connecting to remote server xxxxxxxxxxxxx failed with the following error message : WinRM cannot complete the operation. Verify that the specified computer name is valid, that the computer is accessible over the network, and that a firewall exception for the WinRM service is enabled and allows access from this computer.'''
			WHEN [Message] LIKE '%IPAK SQL Installation%' AND [Message] LIKE '%The client cannot connect%' THEN 'Exception in RunningSequences() : Exception in CheckSequencesStatus() : Breaking error in sequence: ''IPAK SQL Installation'' in state: ''RunningSequences'' Message: ''Connecting to remote server xxxxxxxxxxxx failed with the following error message : The client cannot connect to the destination specified in the request.'''
			WHEN [Message] LIKE '%The request operation did not complete within the allotted timeout of 00:01:10.%' THEN 'The request operation did not complete within the allotted timeout of 00:01:10. The time allotted to this operation may have been a portion of a longer timeout.'
			WHEN [Message] LIKE '%Exception in ProcessorVm.TestPsConnection() : Exception in VirtualMachineRemotePowerShell.GetPublicPort() : Virtual Machine:%' 
			    THEN 'Exception in ProcessorVm.TestPsConnection() : Exception in VirtualMachineRemotePowerShell.GetPublicPort() : Virtual Machine: ''xxxxxxxxxxxx'' not found in config'
	        WHEN [Message] LIKE '%The remote server returned an error: (409) Conflict.%' THEN 'CreateVM Exception : Web Error making REST API call.  Message: The remote server returned an error: (409) Conflict.  A virtual machine with name xxxxxxxxxxxx already exists in the deployment.'
			ELSE Replace(Replace(Replace([Message],CHAR(10),' '),CHAR(13),' '),CHAR(9),' ')
         END AS [Message], [When], StatusCode
	  FROM[dbo].[ChangeLog]) a
  Where CONVERT(date, [When]) >= @BeginDate
  AND CONVERT(date, [When]) <= @EndDate
  AND StatusCode = 'Exception'
  Group By [Message] WITH ROLLUP
  ORDER BY COUNT DESC
END
RETURN