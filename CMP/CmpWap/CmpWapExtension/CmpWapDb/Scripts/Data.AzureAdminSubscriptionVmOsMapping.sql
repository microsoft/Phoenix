--------------------------------------------------
-- Insert/Update/Delete script for table AzureAdminSubscriptionVmOsMapping
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT AzureAdminSubscriptionVmOsMapping ON
 
CREATE TABLE #WorkTable (
[Id] [int] NOT NULL,
	[VmOsId] [int] NOT NULL,
	[PlanId] varchar(50) NOT NULL,
	[IsActive] [bit] NOT NULL
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



-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN
 
 
--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
-- Dev Note - Update will not update ' to NULL OR 0 to NULL
UPDATE AzureAdminSubscriptionVmOsMapping
SET   VmOsId = source.VmOsId
        , PlanId = source.PlanId
        , IsActive = source.IsActive

FROM #WorkTable source
    JOIN AzureAdminSubscriptionVmOsMapping target
    ON      source.Id = target.Id
    AND ( ISNULL(source.VmOsId , 0) <>  ISNULL(target.VmOsId, 0)
    OR  ISNULL(source.PlanId , '') <>  ISNULL(target.PlanId , '')
    OR  ISNULL(source.IsActive , 0) <>  ISNULL(target.IsActive , 0)
    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT AzureAdminSubscriptionVmOsMapping
    (VmOsId, PlanId, IsActive)
SELECT
    VmOsId, PlanId, IsActive
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM AzureAdminSubscriptionVmOsMapping target
WHERE 
    source.Id = target.Id

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE AzureAdminSubscriptionVmOsMapping
FROM AzureAdminSubscriptionVmOsMapping target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.Id = target.Id

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
SET IDENTITY_INSERT AzureAdminSubscriptionVmOsMapping OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for AzureAdminSubscriptionVmOsMapping modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
SET IDENTITY_INSERT AzureAdminSubscriptionVmOsMapping OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
