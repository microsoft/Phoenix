CREATE view vw_CMPDailyMailCompletedBuilds as
select 0 as RowOrder,StartDatePST, RoleName,CompletedBuilds,AvgCompleteDuration,MaxCompleteDuration,MinCompleteDuration from vw_CompletedBuildsByDayByRole where StartDatePST = convert(date,dateadd(hour,-7,getutcdate()))
union 
select 1 as RowOrder,StartDatePST,'Total', Sum(CompletedBuilds)
	,convert(varchar(8),CONVERT(TIME, DATEADD(SECOND, AVG(DATEDIFF(SECOND, 0, AvgCompleteDuration)), 0)))
	,convert(varchar(8),CONVERT(TIME, DATEADD(SECOND, AVG(DATEDIFF(SECOND, 0, MaxCompleteDuration)), 0)))
	,convert(varchar(8),CONVERT(TIME, DATEADD(SECOND, AVG(DATEDIFF(SECOND, 0, MinCompleteDuration)), 0))) from vw_CompletedBuildsByDayByRole where StartDatePST = convert(date,dateadd(hour,-7,getutcdate())) group by StartDatePST