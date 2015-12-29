--------------------------------------------------
-- Insert/Update/Delete script for table NetworkNIC
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT NetworkNIC ON
 
CREATE TABLE #WorkTable (
[NetworkNICId] [int]  NOT NULL
, [Name] [nvarchar] (100) NOT NULL
, [Description] [nvarchar] (500) NOT NULL
, [IsActive] [bit]  NOT NULL
, [CreatedOn] [datetime]  NOT NULL
, [CreatedBy] [nvarchar] (256) NOT NULL
, [LastUpdatedOn] [datetime]  NOT NULL
, [LastUpdatedBy] [varchar] (50) NOT NULL
, [ADDomain] [varchar] (100) NULL
, [ADDomainId] [int] NULL
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
(NetworkNICId, Name, Description, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy, ADDomain, ADDomainId)
SELECT 1, N'CORPNET', N'CORPNET', 1, N'Jan 1 2015  12:00:00:000AM', N'contoso\administrator', N'Jan 1 2015  12:00:00:000AM', N'contoso\administrator', N'Temp', 1
 
 
 
-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN
 
 
--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
-- Dev Note - Update will not update ' to NULL OR 0 to NULL
UPDATE NetworkNIC
SET   Name = source.Name
        , Description = source.Description
        , IsActive = source.IsActive
        , CreatedOn = source.CreatedOn
        , CreatedBy = source.CreatedBy
        , LastUpdatedOn = source.LastUpdatedOn
        , LastUpdatedBy = source.LastUpdatedBy
		, ADDomain = source.ADDomain
		, ADDomainId = source.ADDomainId

FROM #WorkTable source
    JOIN NetworkNIC target
    ON      source.NetworkNICId = target.NetworkNICId
    AND ( ISNULL(source.Name , '') <>  ISNULL(target.Name , '')
    OR  ISNULL(source.Description , '') <>  ISNULL(target.Description , '')
    OR  ISNULL(source.IsActive , 0) <>  ISNULL(target.IsActive , 0)
    OR  ISNULL(source.CreatedOn , '') <>  ISNULL(target.CreatedOn , '')
    OR  ISNULL(source.CreatedBy , '') <>  ISNULL(target.CreatedBy , '')
    OR  ISNULL(source.LastUpdatedOn , '') <>  ISNULL(target.LastUpdatedOn , '')
    OR  ISNULL(source.LastUpdatedBy , '') <>  ISNULL(target.LastUpdatedBy , '')
	OR  ISNULL(source.ADDomain , '') <> ISNULL(target.ADDomain , '')
	OR  ISNULL(source.ADDomainId , '') <> ISNULL(target.ADDomainId , '') 
    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT NetworkNIC
    ( Name, Description, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy, ADDomain, ADDomainId)
SELECT
    Name, Description, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy, ADDomain, ADDomainId
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM NetworkNIC target
WHERE 
    source.NetworkNICId = target.NetworkNICId

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE NetworkNIC
FROM NetworkNIC target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.NetworkNICId = target.NetworkNICId

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
SET IDENTITY_INSERT NetworkNIC OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for NetworkNIC modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
SET IDENTITY_INSERT NetworkNIC OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
