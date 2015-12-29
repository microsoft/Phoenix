--------------------------------------------------
-- Insert/Update/Delete script for table AdDomainMap
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT AdDomainMap ON

CREATE TABLE #WorkTable (
[Id] [int] NOT NULL
, [DomainShortName] [varchar] (max) NULL
, [DomainFullName] [varchar] (max) NULL
, [JoinCredsUserName] [varchar] (max) NULL
, [JoinCredsPasword] [varchar] (max) NULL
, [IsActive] [bit]  NULL
, [Config] [varchar] (max) NULL
, [ServerOU] [varchar] (200) NULL
, [WorkstationOU] [varchar] (200) NULL
, [DefaultVmAdminMember] [varchar] (max) NULL
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
--SET IDENTITY_INSERT #WorkTable ON

INSERT #WorkTable 
(Id, DomainShortName, DomainFullName, JoinCredsUserName, JoinCredsPasword, IsActive, Config, ServerOU, WorkstationOU, DefaultVmAdminMember)
SELECT 1, N'CONTOSO', N'contoso.int', N'contoso\Administrator', NULL, 1, NULL, N'CN=Computers,DC=contoso,DC=int', N'CN=Computers,DC=contoso,DC=int', N'CONTOSO\DomainAdmins'
 
--SET IDENTITY_INSERT #WorkTable OFF

-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN
 
 
--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
-- Dev Note - Update will not update ' to NULL OR 0 to NULL
UPDATE AdDomainMap
SET   DomainShortName = source.DomainShortName
        , DomainFullName = source.DomainFullName
        , JoinCredsUserName = source.JoinCredsUserName
        , JoinCredsPasword = source.JoinCredsPasword
        , IsActive = source.IsActive
        , Config = source.Config
        , ServerOU = source.ServerOU
        , WorkstationOU = source.WorkstationOU
        , DefaultVmAdminMember = source.DefaultVmAdminMember

FROM #WorkTable source
    JOIN AdDomainMap target
    ON      source.Id = target.Id
    AND ( ISNULL(source.DomainShortName , '') <>  ISNULL(target.DomainShortName , '')
    OR  ISNULL(source.DomainFullName , '') <>  ISNULL(target.DomainFullName , '')
    OR  ISNULL(source.JoinCredsUserName , '') <>  ISNULL(target.JoinCredsUserName , '')
    OR  ISNULL(source.JoinCredsPasword , '') <>  ISNULL(target.JoinCredsPasword , '')
    OR  ISNULL(source.IsActive , 0) <>  ISNULL(target.IsActive , 0)
    OR  ISNULL(source.Config , '') <>  ISNULL(target.Config , '')
    OR  ISNULL(source.ServerOU , '') <>  ISNULL(target.ServerOU , '')
    OR  ISNULL(source.WorkstationOU , '') <>  ISNULL(target.WorkstationOU , '')
    OR  ISNULL(source.DefaultVmAdminMember , '') <>  ISNULL(target.DefaultVmAdminMember , '')
    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT AdDomainMap
    (DomainShortName, DomainFullName, JoinCredsUserName, JoinCredsPasword, IsActive, Config, ServerOU, WorkstationOU, DefaultVmAdminMember)
SELECT
    DomainShortName, DomainFullName, JoinCredsUserName, JoinCredsPasword, IsActive, Config, ServerOU, WorkstationOU, DefaultVmAdminMember
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM AdDomainMap target
WHERE 
    source.Id = target.Id

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE AdDomainMap
FROM AdDomainMap target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.Id = target.Id

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
	SET IDENTITY_INSERT AdDomainMap OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for AdDomainMap modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
	SET IDENTITY_INSERT AdDomainMap OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
