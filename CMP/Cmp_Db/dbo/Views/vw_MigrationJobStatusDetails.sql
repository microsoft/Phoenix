CREATE view vw_MigrationJobStatusDetails as
SELECT 
	VMDR.ID AS [CMPRequestID],
	VMDR.TargetVMName AS [VMName],
	VMDR.StatusCode,
	VMMR.SourceServerName as [VMSourceName],
	[AD Domain] = CASE
	WHEN VMDR.TagData NOT LIKE '%<ActiveDirectoryDomain>%' THEN 'No info in TAGDATA'
	WHEN VMDR.TagData LIKE '%<ActiveDirectoryDomain>%' THEN
	SUBSTRING (VMDR.TagData,(PATINDEX('%<ActiveDirectoryDomain>%', VMDR.TagData))+23,PATINDEX('%</ActiveDirectoryDomain>%', VMDR.TagData)-PATINDEX('%<ActiveDirectoryDomain>%',VMDR.TagData)-23)
	ELSE 'Domain is both present/not present - debug'
	END,
	[Org Division] = CASE
		   WHEN VMDR.TagData IS NULL THEN 'No info in TAGDATA'
		   WHEN VMDR.Tagdata LIKE '<WapReq>%' THEN 'WAP Request'
		   WHEN VMDR.TagData LIKE '<MyCapReq>%' THEN SUBSTRING (VMDR.TagData,(PATINDEX('%<OrgDivision>%', VMDR.TagData))+13,PATINDEX('%</OrgDivision>%', VMDR.TagData)-PATINDEX('%<OrgDivision>%',VMDR.TagData)-13)
		   WHEN VMDR.TagData LIKE '%<Request>AzureMig:%' THEN Replace(SUBSTRING(VMDR.TagData,PATINDEX('%<OrgDivison>%',VMDR.TagData),PATINDEX('%</OrgDivison>%',VMDR.TagData)-PATINDEX('%<OrgDivison>%',VMDR.TagData)),'<OrgDivison>','')			
		   ELSE 'Other Data'
	END,
	VMDR.StatusCode AS [Status],
	VMDR.ParentAppName AS [Application Name],
	VMDR.ParentAppID AS [Application ID],
	VMDR.WhoRequested AS [Requestor],
	VMDR.StatusMessage AS [Status Message],
	isnull(CAST(VRT.ValidationResults AS XML).query('*/Result').value('.','varchar(50)'),'') AS ValidationResult,
[Azure IP Address] = CASE
	WHEN VMDR.Config LIKE '%<VmAddress />%' THEN 'IP Address not listed'
	WHEN VMDR.Config LIKE '%<VmAddress>https://%' THEN SUBSTRING (VMDR.Config,(PATINDEX('%<VmAddress>https://%',VMDR.Config))+19,(PATINDEX('%:5986</VmAddress>%', VMDR.Config))-(PATINDEX('%<VmAddress>%',VMDR.Config)+19))
	WHEN VMDR.Config LIKE '%<VmAddress>%' THEN SUBSTRING (VMDR.Config,(PATINDEX('%<VmAddress>%',VMDR.Config))+11,(PATINDEX('%</VmAddress>%', VMDR.Config))-(PATINDEX('%<VmAddress>%',VMDR.Config)+11))
	ELSE 'IP Address Not Listed'
END,
	DATEADD(HH,-8,VMDR.WhenRequested) AS StartTimePST,
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
	IsNull(VMDR.Warnings,'') AS [Warnings]
FROM 
(
	select RequestID as rID
		,StartTimePST
	from vw_MigrationRequestExecutionTimes
	--and convert(date,StartTimePST) = convert(date,dateadd(hour,-7,getutcdate()))
) as mig
join VMDeploymentRequests VMDR
	on mig.rID = VMDR.ID
LEFT OUTER JOIN (SELECT ID, ValidationResults FROM VMDeploymentRequests) VRT ON VMDR.ID = VRT.ID
LEFT JOIN VMMigrationRequests VMMR
	on VMMR.VMDeploymentRequestID = VMDR.ID
where VMDR.StatusCode in ('Complete','Rejected','Exception')