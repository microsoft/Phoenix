
CREATE view vw_DeploymentRequestExecutionTimes as
with CMPX (RequestID,RequestName,RoleName,StatusCode,ExecutionTime,Position,ExecutionStatus,Message,ExecutionID) as
(
	select RequestID
	,RequestName
	,RoleName
	,StatusCode
	,[When] as ExecutionTime
	,case when (StatusCode = 'QcVmRequest') then 'Start' 
		  when StatusCode = LS then 'End'
		  when StatusCode != LS and StatusCode != 'QcVmRequest' then 'End'
		else 'Unknown' 
	end as Position
	,case when StatusCode = 'QcVmRequest' then 'Started'
		 when StatusCode = 'Complete' then 'Completed'
		 when StatusCode = 'Rejected' then 'Rejected'
		 when StatusCode = 'Exception' then 'Failed'
		 else 'Unknown'
	end as Status
	,Message
	,DENSE_RANK() over (
				partition by RequestID
				,case when StatusCode = 'QcVmRequest' then 1 else 0 end
				order by [When] desc
				) as ExecutionID
	from ChangeLog cl
	join (select ID as RID
		,RequestName
		,LastState as LS 
		,Replace(SUBSTRING(TagData,PATINDEX('%<RoleName>%',TagData),PATINDEX('%</RoleName>%',TagData)-PATINDEX('%<RoleName>%',TagData)),'<RoleName>','') as RoleName
		from VmDeploymentRequests) vmd
		on vmd.RID = cl.RequestID
)
select CMPStart.RequestID,CMPStart.RequestName,CMPStart.RoleName
,CONVERT(VARCHAR(8),CMPEnd.ExecutionTime-CMPStart.ExecutionTime,108) as ExecutionDuration
,Convert(datetime,SWITCHOFFSET(CONVERT(datetimeoffset, CMPStart.ExecutionTime),DATENAME(TzOffset, SYSDATETIMEOFFSET()))) as StartTimePST
,Convert(datetime,SWITCHOFFSET(CONVERT(datetimeoffset, CMPEnd.ExecutionTime),DATENAME(TzOffset, SYSDATETIMEOFFSET()))) as EndTimePST
,CMPEnd.ExecutionStatus as ExecutionStatus
,CMPEnd.StatusCode as LastExecutionStep
,CMPEnd.Message
from (select * from CMPX where Position = 'Start') as CMPStart
join (select * from CMPX where Position = 'End') as CMPEnd
	on CMPStart.RequestID = CMPEnd.RequestID
		and CMPStart.ExecutionID = CMPEnd.ExecutionID