




CREATE view [dbo].[vw_ActiveMigrationRequests]
as 
SELECT *  FROM [dbo].[VmMigrationRequests] where StatusCode not in ('QcVmRequest', 'ReadyForTransfer', 'Exception', 'Converted', 'QcVmRequestHold') and [Active]=1