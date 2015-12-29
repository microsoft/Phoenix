--------------------------------------------------
-- Insert/Update/Delete script for table IpakVersionMap
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT IpakVersionMap ON
 
CREATE TABLE #WorkTable (
[Id] [int]  NOT NULL
, [VersionCode] [varchar] (50) NULL
, [VersionName] [varchar] (200) NULL
, [AzureRegion] [varchar] (50) NULL
, [AdDomain] [varchar] (200) NULL
, [Config] [varchar] (max) NULL
, [TagData] [varchar] (max) NULL
, [IpakDirLocation] [varchar] (250) NULL
, [IpakFullFileLocation] [varchar] (250) NULL
, [IsActive] [bit]  NULL
, [AdminName] [varchar] (100) NULL
, [QfeVersion] [varchar] (100) NULL
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
UPDATE IpakVersionMap
SET   VersionCode = source.VersionCode
        , VersionName = source.VersionName
        , AzureRegion = source.AzureRegion
        , AdDomain = source.AdDomain
        , Config = source.Config
        , TagData = source.TagData
        , IpakDirLocation = source.IpakDirLocation
        , IpakFullFileLocation = source.IpakFullFileLocation
        , IsActive = source.IsActive
        , AdminName = source.AdminName
        , QfeVersion = source.QfeVersion

FROM #WorkTable source
    JOIN IpakVersionMap target
    ON      source.Id = target.Id
    AND ( ISNULL(source.VersionCode , '') <>  ISNULL(target.VersionCode , '')
    OR  ISNULL(source.VersionName , '') <>  ISNULL(target.VersionName , '')
    OR  ISNULL(source.AzureRegion , '') <>  ISNULL(target.AzureRegion , '')
    OR  ISNULL(source.AdDomain , '') <>  ISNULL(target.AdDomain , '')
    OR  ISNULL(source.Config , '') <>  ISNULL(target.Config , '')
    OR  ISNULL(source.TagData , '') <>  ISNULL(target.TagData , '')
    OR  ISNULL(source.IpakDirLocation , '') <>  ISNULL(target.IpakDirLocation , '')
    OR  ISNULL(source.IpakFullFileLocation , '') <>  ISNULL(target.IpakFullFileLocation , '')
    OR  ISNULL(source.IsActive , 0) <>  ISNULL(target.IsActive , 0)
    OR  ISNULL(source.AdminName , '') <>  ISNULL(target.AdminName , '')
    OR  ISNULL(source.QfeVersion , '') <>  ISNULL(target.QfeVersion , '')
    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT IpakVersionMap
    (VersionCode, VersionName, AzureRegion, AdDomain, Config, TagData, IpakDirLocation, IpakFullFileLocation, IsActive, AdminName, QfeVersion)
SELECT
    VersionCode, VersionName, AzureRegion, AdDomain, Config, TagData, IpakDirLocation, IpakFullFileLocation, IsActive, AdminName, QfeVersion
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM IpakVersionMap target
WHERE 
    source.Id = target.Id

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE IpakVersionMap
FROM IpakVersionMap target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.Id = target.Id

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
SET IDENTITY_INSERT IpakVersionMap OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for IpakVersionMap modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
SET IDENTITY_INSERT IpakVersionMap OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
