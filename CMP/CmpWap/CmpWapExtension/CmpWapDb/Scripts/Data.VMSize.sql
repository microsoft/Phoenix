--------------------------------------------------
-- Insert/Update/Delete script for table VmSize
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT VmSize ON
 
CREATE TABLE #WorkTable (
[VmSizeId] [int] NOT NULL
, [Name] [nvarchar] (100) NOT NULL
, [Description] [nvarchar] (500) NOT NULL
, [Cores] [int]  NOT NULL
, [Memory] [int]  NOT NULL
, [MaxDataDiskCount] [int]  NOT NULL
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
(VmSizeId, Name, Description, Cores, Memory, MaxDataDiskCount, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy)
SELECT 37, N'Standard_A0', N'Standard_A0 - 1 Cores, 768 MB, 1 Disk', 1, 768, 1, 1, N'2015-11-08 06:41:36.250' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.250' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 38, N'Standard_A1', N'Standard_A1 - 1 Cores, 1 GB, 2 Disk', 1, 1792, 2, 1, N'2015-11-08 06:41:36.267' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.267' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 39, N'Standard_A2', N'Standard_A2 - 2 Cores, 3 GB, 4 Disk', 2, 3584, 4, 1, N'2015-11-08 06:41:36.280' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.280' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 40, N'Standard_A3', N'Standard_A3 - 4 Cores, 7 GB, 8 Disk', 4, 7168, 8, 1, N'2015-11-08 06:41:36.297' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.297' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 41, N'Standard_A5', N'Standard_A5 - 2 Cores, 14 GB, 4 Disk', 2, 14336, 4, 1, N'2015-11-08 06:41:36.313' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.313' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 42, N'Standard_A4', N'Standard_A4 - 8 Cores, 14 GB, 16 Disk', 8, 14336, 16, 1, N'2015-11-08 06:41:36.313' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.313' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 43, N'Standard_A6', N'Standard_A6 - 4 Cores, 28 GB, 8 Disk', 4, 28672, 8, 1, N'2015-11-08 06:41:36.327' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.327' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 44, N'Standard_A7', N'Standard_A7 - 8 Cores, 57 GB, 16 Disk', 8, 57344, 16, 1, N'2015-11-08 06:41:36.343' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.343' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 45, N'Basic_A0', N'Basic_A0 - 1 Cores, 768 MB, 1 Disk', 1, 768, 1, 1, N'2015-11-08 06:41:36.343' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.343' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 46, N'Basic_A1', N'Basic_A1 - 1 Cores, 1 GB, 2 Disk', 1, 1792, 2, 1, N'2015-11-08 06:41:36.360' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.360' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 47, N'Basic_A2', N'Basic_A2 - 2 Cores, 3 GB, 4 Disk', 2, 3584, 4, 1, N'2015-11-08 06:41:36.373' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.373' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 48, N'Basic_A3', N'Basic_A3 - 4 Cores, 7 GB, 8 Disk', 4, 7168, 8, 1, N'2015-11-08 06:41:36.390' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.390' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 49, N'Basic_A4', N'Basic_A4 - 8 Cores, 14 GB, 16 Disk', 8, 14336, 16, 1, N'2015-11-08 06:41:36.390' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.390' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 50, N'Standard_A8', N'Standard_A8 - 8 Cores, 57 GB, 16 Disk', 8, 57344, 16, 1, N'2015-11-08 06:41:36.407' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.407' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 51, N'Standard_A9', N'Standard_A9 - 16 Cores, 114 GB, 16 Disk', 16, 114688, 16, 1, N'2015-11-08 06:41:36.407' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.407' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 52, N'Standard_A10', N'Standard_A10 - 8 Cores, 57 GB, 16 Disk', 8, 57344, 16, 1, N'2015-11-08 06:41:36.420' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.420' AS DateTime, N'CMP WAP Extension Installer'
UNION ALL
SELECT 53, N'Standard_A11', N'Standard_A11 - 16 Cores, 114 GB, 16 Disk', 16, 114688, 16, 1, N'2015-11-08 06:41:36.437' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.437' AS DateTime, N'CMP WAP Extension Installer'


-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN
 
 
--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
-- Dev Note - Update will not update ' to NULL OR 0 to NULL
UPDATE VmSize
SET   Name = source.Name
        , Description = source.Description
        , Cores = source.Cores
        , Memory = source.Memory
        , MaxDataDiskCount = source.MaxDataDiskCount
        , IsActive = source.IsActive
        , CreatedOn = source.CreatedOn
        , CreatedBy = source.CreatedBy
        , LastUpdatedOn = source.LastUpdatedOn
        , LastUpdatedBy = source.LastUpdatedBy

FROM #WorkTable source
    JOIN VmSize target
    ON      source.Name = target.Name
    AND ( ISNULL(source.Name , '') <>  ISNULL(target.Name , '')
    OR  ISNULL(source.Description , '') <>  ISNULL(target.Description , '')
    OR  ISNULL(source.Cores , 0) <>  ISNULL(target.Cores , 0)
    OR  ISNULL(source.Memory , 0) <>  ISNULL(target.Memory , 0)
    OR  ISNULL(source.MaxDataDiskCount , 0) <>  ISNULL(target.MaxDataDiskCount , 0)
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
INSERT VmSize
    (Name, Description, Cores, Memory, MaxDataDiskCount, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy)
SELECT
    Name, Description, Cores, Memory, MaxDataDiskCount, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM VmSize target
WHERE 
    source.VmSizeId = target.VmSizeId

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE VmSize
FROM VmSize target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.Name = target.Name

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
SET IDENTITY_INSERT VmSize OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for VmSize modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
SET IDENTITY_INSERT VmSize OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
