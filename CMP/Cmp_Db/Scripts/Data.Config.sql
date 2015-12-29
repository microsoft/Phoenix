/*
Insert Config data
*/
 
--------------------------------------------------
-- Insert/Update/Delete script for table Config
--------------------------------------------------
 
CREATE TABLE #WorkTable (
[Id]          INT           NOT NULL,
[Name]        VARCHAR (50)  NULL,
[Value]       VARCHAR (MAX) NULL,
[Description] VARCHAR (200) NULL,
[Region]      VARCHAR (50)  NULL,
[Instance]    VARCHAR (50)  NULL,
[IsActive]    BIT           NULL,
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
([Id], [Name], [Value], [Description], [Region], [Instance], [IsActive])
SELECT 0, N'BlockSubmittedQueue', N'false', N'BlockSubmittedQueue', NULL, NULL, 1
 
UNION ALL
 
SELECT 1, N'BlockAllQueues', N'false', N'BlockAllQueues', NULL, NULL, 1
 
-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN
 
 
--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
-- Dev Note - Update will not update ' to NULL OR 0 to NULL
UPDATE Config
SET   Id = source.Id        , Name = source.Name        , Value = source.Value        , Description = source.Description        , Region = source.Region        , Instance = source.Instance        , IsActive = source.IsActive
FROM #WorkTable source
    JOIN Config target
    ON      source.Id = target.Id    AND ( ISNULL(source.Name , '') <>  ISNULL(target.Name , '')    OR  ISNULL(source.Value , '') <>  ISNULL(target.Value , '')    OR  ISNULL(source.Description , '') <>  ISNULL(target.Description , '')    OR  ISNULL(source.Region , '') <>  ISNULL(target.Region , '')    OR  ISNULL(source.Instance , '') <>  ISNULL(target.Instance , '')    OR  ISNULL(source.IsActive , '') <>  ISNULL(target.IsActive , '')    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT Config
    ([Id], [Name], [Value], [Description], [Region], [Instance], [IsActive])
SELECT
    [Id], [Name], [Value], [Description], [Region], [Instance], [IsActive]
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM Config target
WHERE 
    source.Id = target.Id
)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE Config
FROM Config target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.Id = target.Id
)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
    RETURN
 
SuccessfulExit:
    PRINT 'Data for Config modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
