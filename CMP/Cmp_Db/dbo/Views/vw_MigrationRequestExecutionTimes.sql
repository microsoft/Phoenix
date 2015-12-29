CREATE view [dbo].[vw_MigrationRequestExecutionTimes] as
with CMPSubmitted (ReqID,MaxStartTime) as
(
	select RequestID as ReqID
		,Max([When]) as MaxStartTime
	from ChangeLog
	where StatusCode = 'Submitted'
	group by RequestID
)
,CMPX (RequestID,RequestName,StatusCode,ExecutionTime,Position,ExecutionStatus,Message,ExecutionID) as
(
	select RequestID
	,RequestName	
	,StatusCode
	,[When] as ExecutionTime
	,case when (StatusCode = 'Submitted') then 'Start' 
		  when StatusCode = LS then 'End'
		  when StatusCode != LS and StatusCode != 'Submitted' then 'End'
		else 'Unknown' 
	end as Position
	,case when StatusCode = 'Submitted' then 'Started'
		 when StatusCode = 'Complete' then 'Completed'
		 when StatusCode = 'Rejected' then 'Rejected'
		 when StatusCode = 'Exception' then 'Failed'
		 else 'Unknown'
	end as Status
	,Message
	,DENSE_RANK() over (
				partition by RequestID
				,case when StatusCode = 'Submitted' and [When] = cs.MaxStartTime then 1 else 0 end
				order by [When] desc
				) as ExecutionID
	from ChangeLog cl	
	join (select ID as RID
		,RequestName
		,LastState as LS 		
		from VmDeploymentRequests
		where RequestType = 'migrateVM'
		) vmd
		on vmd.RID = cl.RequestID
	join CMPSubmitted cs
		on cs.ReqID = vmd.RID
)
select CMPStart.RequestID,CMPStart.RequestName
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