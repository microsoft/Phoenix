CREATE FUNCTION [dbo].fnCheckUniqueRecord(@RequestName varchar(50) )
RETURNS SMALLINT
AS
BEGIN
DECLARE @IsExists bit
 SET @IsExists=
 CASE
WHEN 
((select count(ID) from  [dbo].[VmDeploymentRequests] where RequestName=@RequestName and StatusCode not in ('Complete','Rejected')) > 0 )
THEN 1

ELSE 0

END

RETURN @IsExists
END;