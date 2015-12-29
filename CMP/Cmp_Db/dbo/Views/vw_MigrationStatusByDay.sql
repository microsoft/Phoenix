
CREATE view [dbo].[vw_MigrationStatusByDay] as
select
	 convert(date,StartTimePST) as StartDatePST
	 ,ExecutionStatus as Status
	 ,count(RequestID) as NumberOfBuilds
	,CONVERT(VARCHAR(8),CAST(AVG(CAST(EndTimePST-StartTimePST AS float)) AS datetime),108) as AvgCompleteDuration
	,CONVERT(VARCHAR(8),CAST(Max(CAST(EndTimePST-StartTimePST AS float)) AS datetime),108) as MaxCompleteDuration
	,CONVERT(VARCHAR(8),CAST(Min(CAST(EndTimePST-StartTimePST AS float)) AS datetime),108) as MinCompleteDuration
from vw_MigrationRequestExecutionTimes
where ExecutionStatus in ('Completed','Rejected','Failed')
group by ExecutionStatus,convert(date,StartTimePST)