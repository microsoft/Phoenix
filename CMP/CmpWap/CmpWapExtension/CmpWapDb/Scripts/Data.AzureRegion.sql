--------------------------------------------------
-- Insert/Update/Delete script for table AzureRegion
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT AzureRegion ON
 
CREATE TABLE #WorkTable (
[AzureRegionId] [int] NOT NULL
, [Name] [nvarchar] (100) NOT NULL
, [Description] [nvarchar] (500) NOT NULL
, [OsImageContainer] [varchar] (max) NULL
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

INSERT #WorkTable 
([AzureRegionId], [Name], [Description], [OsImageContainer], [IsActive], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy]) 
SELECT 2, N'South Central US', N'South Central US', NULL, 1, N'2015-11-08 06:41:36.110' AS DateTime, N'CONTOSO-administrator', N'2015-11-08 06:41:36.110' AS DateTime, N'CONTOSO-administrator'
UNION ALL
SELECT 3, N'West US', N'West US', NULL, 1, N'2015-11-08 06:41:36.140' AS DateTime, N'CONTOSO-administrator', N'2015-11-08 06:41:36.140' AS DateTime, N'CONTOSO-administrator'
UNION ALL
SELECT 4, N'Central US', N'Central US', NULL, 1, N'2015-11-08 06:41:36.157' AS DateTime, N'CONTOSO-administrator', N'2015-11-08 06:41:36.157' AS DateTime, N'CONTOSO-administrator'
UNION ALL
SELECT 5, N'East US 2', N'East US 2', NULL, 1, N'2015-11-08 06:41:36.170' AS DateTime, N'CONTOSO-administrator', N'2015-11-08 06:41:36.170' AS DateTime, N'CONTOSO-administrator'
UNION ALL
SELECT 6, N'North Europe', N'North Europe', NULL, 1, N'2015-11-08 06:41:36.170' AS DateTime, N'CONTOSO-administrator', N'2015-11-08 06:41:36.170' AS DateTime, N'CONTOSO-administrator'
UNION ALL
SELECT 7, N'West Europe', N'West Europe', NULL, 1, N'2015-11-08 06:41:36.187' AS DateTime, N'CONTOSO-administrator', N'2015-11-08 06:41:36.187' AS DateTime, N'CONTOSO-administrator'
UNION ALL
SELECT 8, N'Southeast Asia', N'Southeast Asia', NULL, 1, N'2015-11-08 06:41:36.203' AS DateTime, N'CONTOSO-administrator', N'2015-11-08 06:41:36.203' AS DateTime, N'CONTOSO-administrator'
UNION ALL
SELECT 9, N'East Asia', N'East Asia', NULL, 1, N'2015-11-08 06:41:36.203' AS DateTime, N'CONTOSO-administrator', N'2015-11-08 06:41:36.203' AS DateTime, N'CONTOSO-administrator'

 
-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN
 
 
--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
-- Dev Note - Update will not update ' to NULL OR 0 to NULL
UPDATE AzureRegion
SET   Name = source.Name
        , Description = source.Description
        , OsImageContainer = source.OsImageContainer
        , IsActive = source.IsActive
        , CreatedOn = source.CreatedOn
        , CreatedBy = source.CreatedBy
        , LastUpdatedOn = source.LastUpdatedOn
        , LastUpdatedBy = source.LastUpdatedBy

FROM #WorkTable source
    JOIN AzureRegion target
    ON      source.AzureRegionId = target.AzureRegionId
    AND ( ISNULL(source.Name , '') <>  ISNULL(target.Name , '')
    OR  ISNULL(source.Description , '') <>  ISNULL(target.Description , '')
    OR  ISNULL(source.OsImageContainer , '') <>  ISNULL(target.OsImageContainer , '')
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
INSERT AzureRegion
    (Name, Description, OsImageContainer, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy)
SELECT
    Name, Description, OsImageContainer, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM AzureRegion target
WHERE 
    source.AzureRegionId = target.AzureRegionId

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE AzureRegion
FROM AzureRegion target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.AzureRegionId = target.AzureRegionId

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
SET IDENTITY_INSERT AzureRegion OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for AzureRegion modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
SET IDENTITY_INSERT AzureRegion OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
