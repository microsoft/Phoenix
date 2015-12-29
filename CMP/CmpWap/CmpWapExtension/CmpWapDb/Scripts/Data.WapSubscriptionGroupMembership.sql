/*						
--------------------------------------------------------------------------------------
Insert/Update/Delete script for table WapSubscriptionGroupMembership				
--------------------------------------------------------------------------------------
*/

CREATE TABLE #WorkTable (
    [Id]                INT           NOT NULL,
    [WapSubscriptionID] VARCHAR (100) NOT NULL,
    [GroupID]           INT           NOT NULL,
    [GroupName]         VARCHAR (100) NULL,
    [Config]            VARCHAR (MAX) NULL,
    [TagData]           VARCHAR (MAX) NULL,
    [TagId]             INT           NULL,
    [IsActive]          BIT           NOT NULL,
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
(Id, WapSubscriptionID, GroupID, GroupName, Config, TagData, TagId, IsActive)
SELECT 1, 1, 1, N'SampleGroup', N'SampleConfig', N'SampleTag', 1, 1

-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN

--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
SELECT @vUpdatedRows = @@ROWCOUNT

--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT WapSubscriptionGroupMembership
    (Id, WapSubscriptionID, GroupID, GroupName, Config, TagData, TagId, IsActive)
SELECT
    Id, WapSubscriptionID, GroupID, GroupName, Config, TagData, TagId, IsActive
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM WapSubscriptionGroupMembership target
WHERE 
    source.Id = target.Id

)

SELECT @vInsertedRows = @@ROWCOUNT

--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
SELECT @vDeletedRows = @@ROWCOUNT

 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
    RETURN
 
SuccessfulExit:
    PRINT 'Data for WapSubscriptionGroupMembership modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO