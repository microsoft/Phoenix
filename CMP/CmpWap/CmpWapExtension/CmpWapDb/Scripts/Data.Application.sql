--------------------------------------------------
-- Insert/Update/Delete script for table Application
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT Application ON
 
CREATE TABLE #WorkTable (
[ApplicationId] [int]  NOT NULL
, [Name] [nvarchar] (300) NOT NULL
, [Code] [nvarchar] (300) NOT NULL
, [HasService] [bit]  NOT NULL
, [SubscriptionId] NVARCHAR (100) NOT NULL
, [CIOwner] [varchar] (150) NULL
, [IsActive] [bit]  NOT NULL
, [CreatedOn] [datetime]  NOT NULL
, [CreatedBy] [nvarchar] (256) NOT NULL
, [LastUpdatedOn] [datetime]  NOT NULL
, [LastUpdatedBy] [varchar] (50) NOT NULL
, [Region] [nvarchar] (50) NOT NULL
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
UPDATE Application
SET   Name = source.Name
        , Code = source.Code
        , HasService = source.HasService
        , CIOwner = source.CIOwner
        , IsActive = source.IsActive
		, SubscriptionId = source.SubscriptionId
        , CreatedOn = source.CreatedOn
        , CreatedBy = source.CreatedBy
        , LastUpdatedOn = source.LastUpdatedOn
        , LastUpdatedBy = source.LastUpdatedBy
		, Region = source.Region

FROM #WorkTable source
    JOIN Application target
    ON      source.ApplicationId = target.ApplicationId
    AND ( ISNULL(source.Name , '') <>  ISNULL(target.Name , '')
    OR  ISNULL(source.Code , '') <>  ISNULL(target.Code , '')
    OR  ISNULL(source.HasService , 0) <>  ISNULL(target.HasService , 0)
    OR  ISNULL(source.CIOwner , '') <>  ISNULL(target.CIOwner , '')
    OR  ISNULL(source.IsActive , 0) <>  ISNULL(target.IsActive , 0)
	OR	ISNULL(source.SubscriptionId, '') <> ISNULL(target.SubscriptionId, '')
    OR  ISNULL(source.CreatedOn , '') <>  ISNULL(target.CreatedOn , '')
    OR  ISNULL(source.CreatedBy , '') <>  ISNULL(target.CreatedBy , '')
    OR  ISNULL(source.LastUpdatedOn , '') <>  ISNULL(target.LastUpdatedOn , '')
    OR  ISNULL(source.LastUpdatedBy , '') <>  ISNULL(target.LastUpdatedBy , '')
    OR  ISNULL(source.Region , '') <>  ISNULL(target.Region , '')
    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT Application
    (Name, Code, HasService, CIOwner, IsActive, SubscriptionId, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy, Region)
SELECT
    Name, Code, HasService, CIOwner, IsActive, SubscriptionId, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy, Region
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM Application target
WHERE 
    source.ApplicationId = target.ApplicationId

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE Application
FROM Application target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.ApplicationId = target.ApplicationId

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
--SET IDENTITY_INSERT Application OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for Application modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
--SET IDENTITY_INSERT Application OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
