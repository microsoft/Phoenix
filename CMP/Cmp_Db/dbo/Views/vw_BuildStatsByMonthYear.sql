CREATE view vw_BuildStatsByMonthYear as
select IsNull(ExecutionStatus,'Total:') as ExecutionStatus,MonthYear,count(*) as CountOfSvrs
from
(
	
	select 
			case
				when StatusCode = 'Exception' then 'Failed'
				when StatusCode = 'Complete' then 'Completed'
				when StatusCode = 'Rejected' then 'Rejected'
				else 'In Progress'
			end as ExecutionStatus
		,datename(MONTH,LastStatusUpdate) + ' ' + datename(YEAR,LastStatusUpdate) as MonthYear
	from VmDeploymentRequests
) as data
group by MonthYear, ExecutionStatus with Rollup
--order by cast(MonthYear as date) desc