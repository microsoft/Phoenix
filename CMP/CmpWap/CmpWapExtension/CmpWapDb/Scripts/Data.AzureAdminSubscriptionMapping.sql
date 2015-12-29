--------------------------------------------------
-- Insert/Update/Delete script for table AzureAdminSubscriptionMapping
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT AzureAdminSubscriptionMapping ON
 
CREATE TABLE #WorkTable (
[Id] [int] NOT NULL,
	[SubId] [nvarchar](50) NULL,
	[PlanId] [nvarchar](50) NULL,	
	[IsActive][Bit]
)
GO
 
DECLARE 
     @vInsertedRows INT
    , @vUpdatedRows INT
    , @vDeletedRows INT
    , @vNow         DATETIME
 
SELECT @vNow = GETDATE()
 
--------------------------------------------------
-- Populate base temp table. 
--------------------------------------------------

INSERT #WorkTable 
([Id], [SubId], [PlanId], [IsActive]) 
Select 1, 22, 'phoenifjy4jzt', 0
UNION ALL
Select 2, 23, 'phoenifjy4jzt', 0
UNION ALL
Select 3, 24, 'phoenifjy4jzt', 1
UNION ALL
Select 4, 25, 'phoenifjy4jzt', 1

-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN
 
 
--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
-- Dev Note - Update will not update ' to NULL OR 0 to NULL
UPDATE AzureAdminSubscriptionMapping
SET   SubId = source.SubId
        , PlanId = source.PlanId
        , IsActive = source.IsActive

FROM #WorkTable source
    JOIN AzureAdminSubscriptionMapping target
    ON      source.Id = target.Id
    AND ( ISNULL(source.SubId , '') <>  ISNULL(target.SubId, '')
    OR  ISNULL(source.PlanId , '') <>  ISNULL(target.PlanId , '')
    OR  ISNULL(source.IsActive , 0) <>  ISNULL(target.IsActive , 0)
    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT AzureAdminSubscriptionMapping
    (SubId, PlanId, IsActive)
SELECT
    SubId, PlanId, IsActive
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM AzureAdminSubscriptionMapping target
WHERE 
    source.Id = target.Id

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE AzureAdminSubscriptionMapping
FROM AzureAdminSubscriptionMapping target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.Id = target.Id

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
SET IDENTITY_INSERT AzureAdminSubscriptionMapping OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for AzureAdminSubscriptionMapping modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
SET IDENTITY_INSERT AzureAdminSubscriptionMapping OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
