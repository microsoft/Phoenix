--------------------------------------------------
-- Insert/Update/Delete script for table EnvironmentType
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT EnvironmentType ON
 
CREATE TABLE #WorkTable (
[EnvironmentTypeId] [int]  NOT NULL
, [Name] [nvarchar] (100) NOT NULL
, [Description] [nvarchar] (500) NOT NULL
, [IsActive] [bit]  NOT NULL
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
--INSERT #WorkTable 
--(EnvironmentTypeId, Name, Description, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy)
--SELECT 1, N'Prod', N'Prod', 1, N'Jan 1 2015  12:00:00:000AM', N'contoso\administrator', N'Jan 1 2015  12:00:00:000AM', N'contoso\administrator'
 
 
 
-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN
 
 
--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
-- Dev Note - Update will not update ' to NULL OR 0 to NULL
UPDATE EnvironmentType
SET   Name = source.Name
        , Description = source.Description
        , IsActive = source.IsActive
        , CreatedOn = source.CreatedOn
        , CreatedBy = source.CreatedBy
        , LastUpdatedOn = source.LastUpdatedOn
        , LastUpdatedBy = source.LastUpdatedBy

FROM #WorkTable source
    JOIN EnvironmentType target
    ON      source.EnvironmentTypeId = target.EnvironmentTypeId
    AND ( ISNULL(source.Name , '') <>  ISNULL(target.Name , '')
    OR  ISNULL(source.Description , '') <>  ISNULL(target.Description , '')
    OR  ISNULL(source.IsActive , 0) <>  ISNULL(target.IsActive , 0)
    OR  ISNULL(source.CreatedOn , '') <>  ISNULL(target.CreatedOn , '')
    OR  ISNULL(source.CreatedBy , '') <>  ISNULL(target.CreatedBy , '')
    OR  ISNULL(source.LastUpdatedOn , '') <>  ISNULL(target.LastUpdatedOn , '')
    OR  ISNULL(source.LastUpdatedBy , '') <>  ISNULL(target.LastUpdatedBy , '')
    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT EnvironmentType
    (Name, Description, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy)
SELECT
    Name, Description, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM EnvironmentType target
WHERE 
    source.EnvironmentTypeId = target.EnvironmentTypeId

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE EnvironmentType
FROM EnvironmentType target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.EnvironmentTypeId = target.EnvironmentTypeId

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
SET IDENTITY_INSERT EnvironmentType OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for EnvironmentType modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
SET IDENTITY_INSERT EnvironmentType OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
