--------------------------------------------------
-- Insert/Update/Delete script for table VmOs
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT VmOs ON
 
CREATE TABLE #WorkTable (
[VmOsId] [int] NOT NULL
, [Name] [nvarchar] (250) NULL
, [Description] [nvarchar] (500) NULL
, [OsFamily] [nvarchar] (1000) NULL
, [AzureImageName] [varchar] (max) NULL
, [IsCustomImage] [bit] NOT NULL
, [IsActive] [bit]  NOT NULL
, [CreatedOn] [datetime]  NOT NULL
, [CreatedBy] [nvarchar] (256) NOT NULL
, [LastUpdatedOn] [datetime]  NOT NULL
, [LastUpdatedBy] [varchar] (50) NOT NULL
, [AzureImagePublisher] [varchar](100) NULL
, [AzureImageOffer] [varchar](100) NULL
, [AzureWindowsOSVersion] [varchar](100) NULL
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
(VmOsId, Name, Description, OsFamily, AzureImageName, IsCustomImage, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy, AzureImagePublisher, AzureImageOffer, AzureWindowsOSVersion)
SELECT 1, N'MicrosoftWindowsServer, WindowsServer, 2008-R2-SP1', N'', N'', N'', 0, 1, N'2015-11-08 06:41:41.937' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.937' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'2008-R2-SP1'
UNION ALL
SELECT 2, N'MicrosoftWindowsServer, WindowsServer, 2012-Datacenter', N'', N'', N'', 0, 1, N'2015-11-08 06:41:41.950' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.950' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'2012-Datacenter'
UNION ALL
SELECT 3, N'MicrosoftWindowsServer, WindowsServer, 2012-R2-Datacenter', N'', N'', N'', 0, 1, N'2015-11-08 06:41:41.967' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.967' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'2012-R2-Datacenter'
UNION ALL
SELECT 4, N'MicrosoftWindowsServer, WindowsServer, 2016-Technical-Preview-3-with-Containers', N'', N'', N'', 0, 0, N'2015-11-08 06:41:41.967' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.967' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'2016-Technical-Preview-3-with-Containers'
UNION ALL
SELECT 5, N'MicrosoftWindowsServer, WindowsServer, 2016-Technical-Preview-4-Nano-Server', N'', N'', N'', 0, 0, N'2015-11-08 06:41:41.983' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.983' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'2016-Technical-Preview-4-Nano-Server'
UNION ALL
SELECT 6, N'MicrosoftWindowsServer, WindowsServer, 2016-Technical-Preview-Nano-Server', N'', N'', N'', 0, 0, N'2015-11-08 06:41:41.983' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.983' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'2016-Technical-Preview-Nano-Server'
UNION ALL
SELECT 7, N'MicrosoftWindowsServer, WindowsServer, Windows-Server-Technical-Preview', N'', N'', N'', 0, 0, N'2015-11-08 06:41:41.997' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.997' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'Windows-Server-Technical-Preview'
 
-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN
 
 
--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
-- Dev Note - Update will not update ' to NULL OR 0 to NULL
UPDATE VmOs
SET   Name = source.Name
        , Description = source.Description
        , OsFamily = source.OsFamily
        , AzureImageName = source.AzureImageName
        , IsCustomImage = source.IsCustomImage
        , IsActive = source.IsActive
        , CreatedOn = source.CreatedOn
        , CreatedBy = source.CreatedBy
        , LastUpdatedOn = source.LastUpdatedOn
        , LastUpdatedBy = source.LastUpdatedBy
		, AzureImagePublisher = source.AzureImagePublisher
		, AzureImageOffer = source.AzureImageOffer
		, AzureWindowsOSVersion = source.AzureWindowsOSVersion

FROM #WorkTable source
    JOIN VmOs target
    ON      source.Name = target.Name
    AND ( ISNULL(source.Name , '') <>  ISNULL(target.Name , '')
    OR  ISNULL(source.Description , '') <>  ISNULL(target.Description , '')
    OR  ISNULL(source.OsFamily , '') <>  ISNULL(target.OsFamily , '')
    OR  ISNULL(source.AzureImageName , '') <>  ISNULL(target.AzureImageName , '')
    OR  ISNULL(source.IsCustomImage , 0) <>  ISNULL(target.IsCustomImage , 0)
    OR  ISNULL(source.IsActive , 0) <>  ISNULL(target.IsActive , 0)
    OR  ISNULL(source.CreatedOn , '') <>  ISNULL(target.CreatedOn , '')
    OR  ISNULL(source.CreatedBy , '') <>  ISNULL(target.CreatedBy , '')
    OR  ISNULL(source.LastUpdatedOn , '') <>  ISNULL(target.LastUpdatedOn , '')
    OR  ISNULL(source.LastUpdatedBy , '') <>  ISNULL(target.LastUpdatedBy , '')
    OR  ISNULL(source.AzureImagePublisher , '') <>  ISNULL(target.AzureImagePublisher , '')
    OR  ISNULL(source.AzureImageOffer , '') <>  ISNULL(target.AzureImageOffer , '')
    OR  ISNULL(source.AzureWindowsOSVersion , '') <>  ISNULL(target.AzureWindowsOSVersion , '')
    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT VmOs
    (Name, Description, OsFamily, AzureImageName, IsCustomImage, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy, AzureImagePublisher, AzureImageOffer, AzureWindowsOSVersion)
SELECT
    Name, Description, OsFamily, AzureImageName, IsCustomImage, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy, AzureImagePublisher, AzureImageOffer, AzureWindowsOSVersion
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM VmOs target
WHERE 
    source.VmOsId = target.VmOsId

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE VmOs
FROM VmOs target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.Name = target.Name

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
SET IDENTITY_INSERT VmOs OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for VmOs modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
SET IDENTITY_INSERT VmOs OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
