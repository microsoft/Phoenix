/*
	Inset script
*/
SET NOCOUNT ON
 
--------------------------------------------------
-- Insert/Update/Delete script for table BadAssets
--------------------------------------------------
 
CREATE TABLE #WorkTable (
[Id]                 INT           NOT NULL,
[AssetName]          VARCHAR (100) NOT NULL,
[AssetTypeCode]      VARCHAR (50)  NOT NULL,
[ProblemDescription] VARCHAR (MAX) NULL,
[WhoReported]        VARCHAR (100) NULL,
[WhenReported]       DATETIME      NULL,
[Config]             VARCHAR (MAX) NULL,
[TagData]            VARCHAR (MAX) NULL,
[Active]             BIT           NOT NULL,
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
UPDATE BadAssets
SET   Id = source.Id        , AssetName = source.AssetName        , AssetTypeCode = source.AssetTypeCode        , ProblemDescription = source.ProblemDescription        , WhoReported = source.WhoReported        , WhenReported = source.WhenReported        , Config = source.Config        , TagData = source.TagData        , Active = source.Active
FROM #WorkTable source
    JOIN BadAssets target
    ON      source.Id = target.Id    AND ( ISNULL(source.AssetName , '') <>  ISNULL(target.AssetName , '')    OR  ISNULL(source.AssetTypeCode , '') <>  ISNULL(target.AssetTypeCode , '')    OR  ISNULL(source.ProblemDescription , '') <>  ISNULL(target.ProblemDescription , '')    OR  ISNULL(source.WhoReported , '') <>  ISNULL(target.WhoReported , '')    OR  ISNULL(source.WhenReported , 0) <>  ISNULL(target.WhenReported , 0)    OR  ISNULL(source.Config , '') <>  ISNULL(target.Config , '')    OR  ISNULL(source.TagData , '') <>  ISNULL(target.TagData , '')    OR  ISNULL(source.Active , '') <>  ISNULL(target.Active , '')    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT BadAssets
    ([Id], [AssetName], [AssetTypeCode], [ProblemDescription], [WhoReported], [WhenReported], [Config], [TagData], [Active])
SELECT
    [Id], [AssetName], [AssetTypeCode], [ProblemDescription], [WhoReported], [WhenReported], [Config], [TagData], [Active]
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM BadAssets target
WHERE 
    source.Id = target.Id
)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE BadAssets
FROM BadAssets target
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
    PRINT 'Data for BadAssets modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
