
create view [dbo].[vw_CompletedMigrationsByDay] as
select
	 convert(date,StartTimePST) as StartDatePST
	 ,count(RequestID) as CompletedBuilds
	,CONVERT(VARCHAR(8),CAST(AVG(CAST(EndTimePST-StartTimePST AS float)) AS datetime),108) as AvgCompleteDuration
	,CONVERT(VARCHAR(8),CAST(Max(CAST(EndTimePST-StartTimePST AS float)) AS datetime),108) as MaxCompleteDuration
	,CONVERT(VARCHAR(8),CAST(Min(CAST(EndTimePST-StartTimePST AS float)) AS datetime),108) as MinCompleteDuration
from vw_MigrationRequestExecutionTimes
where ExecutionStatus = 'Completed'
group by convert(date,StartTimePST)