--------------------------------------------------
-- Insert/Update/Delete script for table AzureRoleSize
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT AzureRoleSize ON
 
CREATE TABLE #WorkTable (
[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](max) NOT NULL,
	[CoreCount] [int] NOT NULL DEFAULT ((0)),
	[DiskCount] [int] NOT NULL DEFAULT ((0)),
	[RamMb] [float] NOT NULL DEFAULT ((0.0)),
	[DiskSizeRoleOs] [int] NOT NULL DEFAULT ((0)),
	[DiskSizeRoleApps] [float] NOT NULL DEFAULT ((0.0)),
	[DiskSizeVmOs] [int] NOT NULL DEFAULT ((0)),
	[DiskSizeVmTemp] [int] NOT NULL DEFAULT ((0)),
	[CanBeService] [bit] NOT NULL DEFAULT ((1)),
	[CanBeVm] [bit] NOT NULL DEFAULT ((0)),
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
([Name], [CoreCount], [DiskCount], [RamMb], [DiskSizeRoleOs], [DiskSizeRoleApps], [DiskSizeVmOs], [DiskSizeVmTemp], [CanBeService], [CanBeVm])
SELECT N'Standard_A0', 1, 0, 768, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_A1', 1, 0, 1792, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_A2', 2, 0, 3584, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_A3', 4, 0, 7168, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_A4', 8, 0, 14336, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Basic_A0', 1, 0, 768, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Basic_A1', 1, 0, 1792, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Basic_A2', 2, 0, 3584, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Basic_A3', 4, 0, 7168, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Basic_A4', 8, 0, 14336, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_A5', 2, 0, 14336, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_A6', 4, 0, 28672, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_A7', 8, 0, 57344, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D1', 1, 0, 3584, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D2', 2, 0, 7168, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D3', 4, 0, 14336, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D4', 8, 0, 28672, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D11', 2, 0, 14336, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D12', 4, 0, 28672, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D13', 8, 0, 57344, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D14', 16, 0, 114688, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D1_v2', 1, 0, 3584, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D2_v2', 2, 0, 7168, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D3_v2', 4, 0, 14336, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D4_v2', 8, 0, 28672, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D5_v2', 16, 0, 57344, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D11_v2', 2, 0, 14336, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D12_v2', 4, 0, 28672, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D13_v2', 8, 0, 57344, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_D14_v2', 16, 0, 114688, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_DS1', 1, 0, 3584, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_DS2', 2, 0, 7168, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_DS3', 4, 0, 14336, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_DS4', 8, 0, 28672, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_DS11', 2, 0, 14336, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_DS12', 4, 0, 28672, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_DS13', 8, 0, 57344, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_DS14', 16, 0, 114688, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_A8', 8, 0, 57344, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_A9', 16, 0, 114688, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_A10', 8, 0, 57344, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_A11', 16, 0, 114688, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_G1', 2, 0, 28672, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_G2', 4, 0, 57344, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_G3', 8, 0, 114688, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_G4', 16, 0, 229376, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_G5', 32, 0, 458752, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_GS1', 2, 0, 28672, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_GS2', 4, 0, 57344, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_GS3', 8, 0, 114688, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_GS4', 16, 0, 229376, 0, 0, 1047552, 0, 0, 0
UNION ALL
SELECT N'Standard_GS5', 32, 0, 458752, 0, 0, 1047552, 0, 0, 0

-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN
 
 
--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
-- Dev Note - Update will not update ' to NULL OR 0 to NULL
UPDATE AzureRoleSize
SET   Name = source.Name
        , CoreCount = source.CoreCount
        , DiskCount = source.DiskCount
        , RamMb = source.RamMb
        , DiskSizeRoleOs = source.DiskSizeRoleOs
        , DiskSizeRoleApps = source.DiskSizeRoleApps
        , DiskSizeVmOs = source.DiskSizeVmOs
        , DiskSizeVmTemp = source.DiskSizeVmTemp
        , CanBeService = source.CanBeService
		, CanBeVm = source.CanBeVm

FROM #WorkTable source
    JOIN AzureRoleSize target
    ON      source.Name = target.Name
    AND ( ISNULL(source.Name , '') <>  ISNULL(target.Name , '')
    OR  ISNULL(source.CoreCount , 0) <>  ISNULL(target.CoreCount , 0)
    OR  ISNULL(source.DiskCount , 0) <>  ISNULL(target.DiskCount , 0)
    OR  ISNULL(source.RamMb , 0) <>  ISNULL(target.RamMb , 0)
    OR  ISNULL(source.DiskSizeRoleOs , 0) <>  ISNULL(target.DiskSizeRoleOs , 0)
    OR  ISNULL(source.DiskSizeRoleApps , 0) <>  ISNULL(target.DiskSizeRoleApps , 0)
    OR  ISNULL(source.DiskSizeVmOs , 0) <>  ISNULL(target.DiskSizeVmOs , 0)
    OR  ISNULL(source.DiskSizeVmTemp , 0) <>  ISNULL(target.DiskSizeVmTemp , 0)
    OR  ISNULL(source.CanBeService , 0) <>  ISNULL(target.CanBeService , 0)
	OR  ISNULL(source.CanBeVm , 0) <>  ISNULL(target.CanBeVm , 0)
    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT AzureRoleSize
    (Name, CoreCount, DiskCount, RamMb, DiskSizeRoleOs, DiskSizeRoleApps, DiskSizeVmOs, DiskSizeVmTemp, CanBeService, CanBeVm)
SELECT
    Name, CoreCount, DiskCount, RamMb, DiskSizeRoleOs, DiskSizeRoleApps, DiskSizeVmOs, DiskSizeVmTemp, CanBeService, CanBeVm
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM AzureRoleSize target
WHERE 
    source.Id = target.Id

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE AzureRoleSize
FROM AzureRoleSize target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.Name = target.Name

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
SET IDENTITY_INSERT AzureRoleSize OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for AzureRoleSize modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
SET IDENTITY_INSERT AzureRoleSize OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
