--------------------------------------------------
-- Insert/Update/Delete script for table ServerRoleDriveMapping
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT ServerRoleDriveMapping ON
 
CREATE TABLE #WorkTable (
[Id] [int]  NOT NULL
, [ServerRoleId] [int]  NOT NULL
, [Drive] [char] (1) NOT NULL
, [MemoryInGB] [int]  NOT NULL
, [CreatedOn] [datetime]  NOT NULL
, [CreatedBy] [nvarchar] (256) NOT NULL
, [LastUpdatedOn] [datetime]  NOT NULL
, [LastUpdatedBy] [varchar] (50) NOT NULL
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
UPDATE ServerRoleDriveMapping
SET   ServerRoleId = source.ServerRoleId
        , Drive = source.Drive
        , MemoryInGB = source.MemoryInGB
        , CreatedOn = source.CreatedOn
        , CreatedBy = source.CreatedBy
        , LastUpdatedOn = source.LastUpdatedOn
        , LastUpdatedBy = source.LastUpdatedBy

FROM #WorkTable source
    JOIN ServerRoleDriveMapping target
    ON      source.Id = target.Id
    AND ( ISNULL(source.ServerRoleId , 0) <>  ISNULL(target.ServerRoleId , 0)
    OR  ISNULL(source.Drive , '') <>  ISNULL(target.Drive , '')
    OR  ISNULL(source.MemoryInGB , 0) <>  ISNULL(target.MemoryInGB , 0)
    OR  ISNULL(source.CreatedOn , '') <>  ISNULL(target.CreatedOn , '')
    OR  ISNULL(source.CreatedBy , '') <>  ISNULL(target.CreatedBy , '')
    OR  ISNULL(source.LastUpdatedOn , '') <>  ISNULL(target.LastUpdatedOn , '')
    OR  ISNULL(source.LastUpdatedBy , '') <>  ISNULL(target.LastUpdatedBy , '')
    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT ServerRoleDriveMapping
    (ServerRoleId, Drive, MemoryInGB, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy)
SELECT
    ServerRoleId, Drive, MemoryInGB, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM ServerRoleDriveMapping target
WHERE 
    source.Id = target.Id

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE ServerRoleDriveMapping
FROM ServerRoleDriveMapping target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.Id = target.Id

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
SET IDENTITY_INSERT ServerRoleDriveMapping OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for ServerRoleDriveMapping modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
SET IDENTITY_INSERT ServerRoleDriveMapping OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
