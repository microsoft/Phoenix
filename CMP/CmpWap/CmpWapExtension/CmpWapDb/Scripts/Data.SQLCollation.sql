--------------------------------------------------
-- Insert/Update/Delete script for table SQLCollation
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT SQLCollation ON
 
CREATE TABLE #WorkTable (
[SQLCollationId] [int]  NOT NULL
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
 
 
 
-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN
 
 
--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
-- Dev Note - Update will not update ' to NULL OR 0 to NULL
UPDATE SQLCollation
SET   Name = source.Name
        , Description = source.Description
        , IsActive = source.IsActive
        , CreatedOn = source.CreatedOn
        , CreatedBy = source.CreatedBy
        , LastUpdatedOn = source.LastUpdatedOn
        , LastUpdatedBy = source.LastUpdatedBy

FROM #WorkTable source
    JOIN SQLCollation target
    ON      source.SQLCollationId = target.SQLCollationId
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
INSERT SQLCollation
    (Name, Description, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy)
SELECT
    Name, Description, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM SQLCollation target
WHERE 
    source.SQLCollationId = target.SQLCollationId

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE SQLCollation
FROM SQLCollation target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.SQLCollationId = target.SQLCollationId

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
SET IDENTITY_INSERT SQLCollation OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for SQLCollation modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
SET IDENTITY_INSERT SQLCollation OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
