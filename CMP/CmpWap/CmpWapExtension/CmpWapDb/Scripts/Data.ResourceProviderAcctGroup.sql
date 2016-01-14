--------------------------------------------------
-- Insert/Update/Delete script for table ResourceProviderAcctGroup
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT ResourceProviderAcctGroup ON
 
CREATE TABLE #WorkTable (
[ResourceProviderAcctGroupId] [int]  NOT NULL
, [Name] [varchar] (50) NOT NULL
, [DomainId] [int]  NOT NULL
, [NetworkNICId] [int]  NOT NULL
, [EnvironmentTypeId] [int]  NOT NULL
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
--([ResourceProviderAcctGroupId], [Name], [DomainId], [NetworkNICId], [EnvironmentTypeId], [IsActive], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy])
--SELECT 1, N'SampleGroup', 1, 1, 1, 1, N'09/29/2015', N'contoso\administrator', N'09/29/2015', N'contoso\administrator'
 
-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN
 
 
--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
-- Dev Note - Update will not update ' to NULL OR 0 to NULL
UPDATE ResourceProviderAcctGroup
SET   Name = source.Name
        , DomainId = source.DomainId
        , NetworkNICId = source.NetworkNICId
        , EnvironmentTypeId = source.EnvironmentTypeId
        , IsActive = source.IsActive
        , CreatedOn = source.CreatedOn
        , CreatedBy = source.CreatedBy
        , LastUpdatedOn = source.LastUpdatedOn
        , LastUpdatedBy = source.LastUpdatedBy

FROM #WorkTable source
    JOIN ResourceProviderAcctGroup target
    ON      source.ResourceProviderAcctGroupId = target.ResourceProviderAcctGroupId
    AND ( ISNULL(source.Name , '') <>  ISNULL(target.Name , '')
    OR  ISNULL(source.DomainId , 0) <>  ISNULL(target.DomainId , 0)
    OR  ISNULL(source.NetworkNICId , 0) <>  ISNULL(target.NetworkNICId , 0)
    OR  ISNULL(source.EnvironmentTypeId , 0) <>  ISNULL(target.EnvironmentTypeId , 0)
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
INSERT ResourceProviderAcctGroup
    (Name, DomainId, NetworkNICId, EnvironmentTypeId, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy)
SELECT
    Name, DomainId, NetworkNICId, EnvironmentTypeId, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM ResourceProviderAcctGroup target
WHERE 
    source.ResourceProviderAcctGroupId = target.ResourceProviderAcctGroupId

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE ResourceProviderAcctGroup
FROM ResourceProviderAcctGroup target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.ResourceProviderAcctGroupId = target.ResourceProviderAcctGroupId

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
SET IDENTITY_INSERT ResourceProviderAcctGroup OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for ResourceProviderAcctGroup modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
SET IDENTITY_INSERT ResourceProviderAcctGroup OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
