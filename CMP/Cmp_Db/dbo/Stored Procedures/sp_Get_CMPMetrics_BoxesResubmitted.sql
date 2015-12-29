
-- =============================================
-- Author:		<Author: Ken Hill>
-- Create date: <Create Date: 4/29/2015>
-- Description:	<Description: Exceptions By Frequency>
-- =============================================
CREATE PROCEDURE [dbo].[sp_Get_CMPMetrics_BoxesResubmitted]
	-- Add the parameters for the stored procedure here
	@BeginDate date,
	@EndDate date
AS
BEGIN
SELECT DISTINCT ID, TargetVmName, StatusCode, 
   Replace(Replace(Replace([Message],CHAR(10),' '),CHAR(13),' '),CHAR(9),' ') as [Message], 
   CASE
      When Warnings LIKE '%administrators%' Then 'Adding entity ''user'' to ''administrators'' group'
      ELSE Replace(Replace(Replace(Warnings,CHAR(10),' '),CHAR(13),' '),CHAR(9),' ')
	  ---ELSE Replace(Replace(Warnings,CHAR(10),' '),CHAR(13),' ')
   END AS Warnings,
   COUNT([Message]) as Frequency FROM
(SELECT b.ID, a.[Message], b.RequestType, b.TargetVmName, a.[When], a.StatusCode, b.StatusMessage, b.Warnings FROM
(SELECT RequestID, [Message],StatusCode, [When]
  FROM [dbo].[ChangeLog]
  Where CONVERT(date, [When]) >= @BeginDate
  AND CONVERT(date, [When]) <= @EndDate
  AND StatusCode = 'Exception') a
INNER JOIN [dbo].[VmDeploymentRequests] b ON a.RequestID = b.ID) c
GROUP BY ID, TargetVmName, StatusCode, Warnings, [Message] 
ORDER BY ID ASC 
END
RETURN