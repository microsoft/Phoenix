create proc sp_Get_InFlightRequests (@TargetVmName varchar(255)) as
set nocount on

select top 1 convert(varchar,isnull(ID,0))
from VmDeploymentRequests
where 
	--match the vm name and the request type
	(TargetVmName = @TargetVmName)
	and
	--only return in-flight requests
	StatusCode not in ('Complete','Rejected')
order by WhenRequested desc