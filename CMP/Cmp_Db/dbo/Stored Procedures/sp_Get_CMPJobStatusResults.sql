






CREATE proc [dbo].[sp_Get_CMPJobStatusResults] (@JobID INT = null) as
--declare @JobID int = null
;WITH ValidationResultsTable AS (SELECT ID, ValidationResults FROM VMDeploymentRequests where 1 = case when @JobID is null then 1 when @JobID is not null and @JobID = ID then 1 end)

SELECT 
	VMDR.ID AS [CMPRequestID],
	isnull(cast(VMDR.Config as xml).query('*/CmdbConfig/RfcNmuber').value('.','varchar(50)'),'') as RfcNumber, --pull the RFC from config xml
	VMDR.RequestType,
	VMDR.TargetVMName AS [VMName],
	VMMR.SourceServerName as [VMSourceName],
	[AD Domain] = CASE
       WHEN VMDR.RequestType = 'MigrateVM' THEN
       CASE 
              WHEN VMDR.Config LIKE '%<MachineDomain />%' THEN 'New VM Domain not listed'
              WHEN VMDR.Config LIKE '%<MachineDomain>%' THEN SUBSTRING (VMDR.Config,(PATINDEX('%<MachineDomain>%', VMDR.Config))+15,PATINDEX('%</MachineDomain>%', VMDR.Config)-PATINDEX('%<MachineDomain>%',VMDR.Config)-15)
              ELSE 'New VM Domain not present'
       END
       WHEN VMDR.RequestType = 'NewVM' THEN
       CASE   
      WHEN VMDR.TagData NOT LIKE '%<ActiveDirectoryDomain>%' THEN 'Domain Info not listed'
      WHEN VMDR.TagData LIKE '%<ActiveDirectoryDomain>%' THEN
      SUBSTRING (VMDR.TagData,(PATINDEX('%<ActiveDirectoryDomain>%', VMDR.TagData))+23,PATINDEX('%</ActiveDirectoryDomain>%', VMDR.TagData)-PATINDEX('%<ActiveDirectoryDomain>%',VMDR.TagData)-23)
      ELSE 'Domain Info not present'
       END    
       ELSE 'Debug - neither NewVM nor MigrateVM'
END,
	[Org Division] = CASE
	   WHEN VMDR.Config LIKE '%<OrgDivision />%' THEN 'Org Division not listed'
       WHEN VMDR.Config LIKE '%<OrgDivison>%' THEN SUBSTRING (VMDR.Config, (PATINDEX('%<OrgDivison>%', VMDR.Config))+12, PATINDEX('%</OrgDivison>%',VMDR.Config)-PATINDEX('%<OrgDivison>%',VMDR.Config)-12 )
       ELSE 'Org Division not present'
END,
	VMDR.StatusCode AS [Status],
	VMDR.ParentAppName AS [Application Name],
	VMDR.ParentAppID AS [Application ID],
	VMDR.WhoRequested AS [Requestor],
	VMDR.StatusMessage AS [Status Message],
	CAST(VRT.ValidationResults AS XML).query('*/Result').value('.','varchar(50)') AS ValidationResult,
[Azure IP Address] = CASE
	WHEN VMDR.Config LIKE '%<VmAddress />%' THEN 'IP Address not listed'
	WHEN VMDR.Config LIKE '%<VmAddress>https://%' THEN SUBSTRING (VMDR.Config,(PATINDEX('%<VmAddress>https://%',VMDR.Config))+19,(PATINDEX('%:5986</VmAddress>%', VMDR.Config))-(PATINDEX('%<VmAddress>%',VMDR.Config)+19))
	WHEN VMDR.Config LIKE '%<VmAddress>%' THEN SUBSTRING (VMDR.Config,(PATINDEX('%<VmAddress>%',VMDR.Config))+11,(PATINDEX('%</VmAddress>%', VMDR.Config))-(PATINDEX('%<VmAddress>%',VMDR.Config)+11))
	ELSE 'IP Address not present'
END,
	DATEADD(HH,-8,VMDR.WhenRequested) AS [Submitted],
	[Submitted Elapsed Time] = CASE
		WHEN VMDR.StatusCode = 'Complete' THEN DATEDIFF(MI,VMDR.WhenRequested,VMDR.CurrentStateStartTime)
		WHEN VMDR.StatusCode NOT IN ('Complete') THEN DATEDIFF(MI,VMDR.WhenRequested,GETUTCDATE())
		ELSE 'N/A'
	END,
	DATEADD(HH,-8,VMDR.CurrentStateStartTime) AS [Last Update],
	[Last Update Elapsed Time] = CASE
		WHEN VMDR.StatusCode = 'Complete' THEN DATEDIFF(MI,VMDR.LastStatusUpdate,VMDR.CurrentStateStartTime)
		WHEN VMDR.StatusCode NOT IN ('Complete') THEN DATEDIFF(MI,VMDR.CurrentStateStartTime,GETUTCDATE())
		ELSE 'N/A'
	END,
	VMDR.ExceptionMessage AS [Exception],
	VMDR.Warnings AS [Warnings]
FROM VMDeploymentRequests VMDR
LEFT OUTER JOIN ValidationResultsTable VRT ON VMDR.ID = VRT.ID
LEFT JOIN VMMigrationRequests VMMR
	on VMMR.VMDeploymentRequestID = VMDR.ID
WHERE 1 = case when @JobID is null then 1 when @JobID is not null and @JobID = VMDR.ID then 1 end