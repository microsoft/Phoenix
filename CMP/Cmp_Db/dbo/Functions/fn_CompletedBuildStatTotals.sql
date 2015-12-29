create FUNCTION fn_CompletedBuildStatTotals
(
	@StartDate datetime=null
	,@NumberOfDays int=null
)
RETURNS @returntable TABLE
(
	StartDate date
	,CompletedBuilds int
	,NumberOfDays int
	,AvgCompleteDuration varchar(8)
	,MaxCompleteDuration varchar(8)
	,MinCompleteDuration varchar(8)
)
AS
BEGIN

insert into @returntable
select
	 isNull(convert(date,@StartDate),convert(date,CAST(Min(CAST(StartTimePST AS float)) AS datetime))) as StartDate
	,count(RequestID) as CompletedBuilds
	,isNull(@NumberOfDays,datediff(day,Min(StartTimePST),getdate())) as NumberOfDays
	,CONVERT(VARCHAR(8),CAST(AVG(CAST(EndTimePST-StartTimePST AS float)) AS datetime),108) as AvgCompleteDuration
	,CONVERT(VARCHAR(8),CAST(Max(CAST(EndTimePST-StartTimePST AS float)) AS datetime),108) as MaxCompleteDuration
	,CONVERT(VARCHAR(8),CAST(Min(CAST(EndTimePST-StartTimePST AS float)) AS datetime),108) as MinCompleteDuration
from vw_DeploymentRequestExecutionTimes
where ExecutionStatus = 'Completed'
--if no completed time was passed in then assume we want all time
and 1 = case when @StartDate is null and @NumberOfDays is null then 1
			 when @StartDate is not null and @NumberOfDays is null and StartTimePST >= @StartDate then 1
			 when @StartDate is not null and @NumberOfDays is not null
				and StartTimePST >= @StartDate and EndTimePST <= dateadd(day,@NumberOfDays,@StartDate) then 1
			 when @StartDate is null and @NumberOfDays is not null and StartTimePST >= dateadd(day,-@NumberOfDays,getdate()) then 1
			 else 0
		end
	RETURN
END