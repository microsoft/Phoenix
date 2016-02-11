
/*
	Insert for ServiceProviderAccounts 
	Change to your Azure account
*/
 
--------------------------------------------------
-- Insert/Update/Delete script for table ServiceProviderAccounts
--------------------------------------------------
 
CREATE TABLE #WorkTable (
[ID]                         INT            NOT NULL,
[Name]                       VARCHAR (100)  NULL,
[Description]                VARCHAR (100)  NULL,
[OwnerNamesCSV]              VARCHAR (1024) NULL,
[Config]                     VARCHAR (MAX)  NULL,
[TagData]                    VARCHAR (MAX)  NULL,
[TagID]                      INT            NULL,
[AccountType]                VARCHAR (50)   NULL,
[ExpirationDate]             DATETIME       NULL,
[CertificateBlob]            VARCHAR (MAX)  NULL,
[CertificateThumbprint]      VARCHAR (100)  NULL,
[AccountID]                  VARCHAR (100)  NULL,
[AccountPassword]            VARCHAR (100)  NULL,
[Active]                     BIT            NULL,
[AzRegion]                   VARCHAR (50)   NULL,
[AzAffinityGroup]            VARCHAR (50)   NULL,
[AzVNet]                     VARCHAR (50)   NULL,
[AzSubnet]                   VARCHAR (50)   NULL,
[AzStorageContainerUrl]      VARCHAR (100)  NULL,
[ResourceGroup]              VARCHAR (50)   NULL,
[CoreCountMax]               INT            NULL,
[CoreCountCurrent]           INT            NULL,
[VnetCountMax]               INT            NULL,
[VnetCountCurrent]           INT            NULL,
[StorageAccountCountMax]     INT            NULL,
[StorageAccountCountCurrent] INT            NULL,
[VmsPerVnetCountMax]         INT            NULL,
[VmsPerServiceCountMax]      INT            NULL,
[AzureADClientId]			 VARCHAR (50)   NULL,
[AzureADTenantId]			 VARCHAR (50)   NULL,
[AzureADClientKey]			 VARCHAR (max)   NULL
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
UPDATE ServiceProviderAccounts
SET   Id = source.Id
        , Name = source.Name
        , Description = source.Description
        , OwnerNamesCSV = source.OwnerNamesCSV
		, Config = source.Config
        , TagData = source.TagData
        , TagID = source.TagID
		, AccountType = source.AccountType
		, ExpirationDate = source.ExpirationDate
		, CertificateBlob = source.CertificateBlob
		, CertificateThumbprint = source.CertificateThumbprint
		, AccountID = source.AccountID
		, AccountPassword = source.AccountPassword
		, Active = source.Active
		, AzRegion = source.AzRegion
		, AzAffinityGroup =source.AzAffinityGroup
		, AzVNet = source.AzVNet
		, AzSubnet = source.AzSubnet
		, AzStorageContainerUrl = source.AzStorageContainerUrl
		, ResourceGroup = source.ResourceGroup
		, CoreCountMax = source.CoreCountMax
		, CoreCountCurrent = source.CoreCountCurrent
		, VnetCountMax = source.VnetCountMax
		, VnetCountCurrent = source.VnetCountCurrent
		, StorageAccountCountMax = source.StorageAccountCountMax
		, StorageAccountCountCurrent = source.StorageAccountCountCurrent
		, VmsPerVnetCountMax = source.VmsPerVnetCountMax
		, VmsPerServiceCountMax = source.VmsPerServiceCountMax
		, AzureADTenantId = source.AzureADTenantId
		, AzureADClientId = source.AzureADClientId
		, AzureADClientKey = source.AzureADClientKey

FROM #WorkTable source
    JOIN ServiceProviderAccounts target
    ON      source.Id = target.Id
    AND ( ISNULL(source.Name , '') <>  ISNULL(target.Name , '')
    OR  ISNULL(source.Description , '') <>  ISNULL(target.Description , '')
    OR  ISNULL(source.OwnerNamesCSV , '') <>  ISNULL(target.OwnerNamesCSV , '')
    OR  ISNULL(source.Config , '') <>  ISNULL(target.Config , '')
    OR  ISNULL(source.TagData , '') <>  ISNULL(target.TagData , '')
	OR  ISNULL(source.TagID , '') <>  ISNULL(target.TagID , '')
    OR  ISNULL(source.AccountType , '') <>  ISNULL(target.AccountType , '')
    OR  ISNULL(source.ExpirationDate , '') <>  ISNULL(target.ExpirationDate , '')
    OR  ISNULL(source.CertificateBlob , '') <>  ISNULL(target.CertificateBlob , '')
    OR  ISNULL(source.CertificateThumbprint , '') <>  ISNULL(target.CertificateThumbprint , '')
    OR  ISNULL(source.AccountID , '') <>  ISNULL(target.AccountID , '')
    OR  ISNULL(source.AccountPassword , '') <>  ISNULL(target.AccountPassword , '')
    OR  ISNULL(source.Active , '') <>  ISNULL(target.Active , '')
    OR  ISNULL(source.AzRegion , '') <>  ISNULL(target.AzRegion , '')
    OR  ISNULL(source.AzAffinityGroup , '') <>  ISNULL(target.AzAffinityGroup , '')
    OR  ISNULL(source.AzVNet , '') <>  ISNULL(target.AzVNet , '')
    OR  ISNULL(source.AzSubnet , '') <>  ISNULL(target.AzSubnet , '')
	OR  ISNULL(source.AzStorageContainerUrl , '') <>  ISNULL(target.AzStorageContainerUrl , '')
	OR  ISNULL(source.ResourceGroup , '') <>  ISNULL(target.ResourceGroup , '')
	OR  ISNULL(source.CoreCountMax , '') <>  ISNULL(target.CoreCountMax , '')
	OR  ISNULL(source.CoreCountCurrent , '') <>  ISNULL(target.CoreCountCurrent , '')
	OR  ISNULL(source.VnetCountMax , '') <>  ISNULL(target.VnetCountMax , '')
	OR  ISNULL(source.VnetCountCurrent , '') <>  ISNULL(target.VnetCountCurrent , '')
	OR  ISNULL(source.StorageAccountCountMax , '') <>  ISNULL(target.StorageAccountCountMax , '')
	OR  ISNULL(source.StorageAccountCountCurrent , '') <>  ISNULL(target.StorageAccountCountCurrent , '')
	OR  ISNULL(source.VmsPerVnetCountMax , '') <>  ISNULL(target.VmsPerVnetCountMax , '')
	OR  ISNULL(source.VmsPerServiceCountMax , '') <>  ISNULL(target.VmsPerServiceCountMax , '')
	OR  ISNULL(source.AzureADTenantId , '') <>  ISNULL(target.AzureADTenantId , '')
	OR  ISNULL(source.AzureADClientId , '') <>  ISNULL(target.AzureADClientId , '')
	OR  ISNULL(source.AzureADClientKey , '') <>  ISNULL(target.AzureADClientKey, '')
    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT ServiceProviderAccounts
    ([ID], [Name], [Description], [OwnerNamesCSV], [Config], [TagData], [TagID], [AccountType], [ExpirationDate], [CertificateBlob], [CertificateThumbprint], [AccountID], [AccountPassword], [Active], [AzRegion], [AzAffinityGroup], [AzVNet], [AzSubnet], [AzStorageContainerUrl], [ResourceGroup], [CoreCountMax], [CoreCountCurrent], [VnetCountMax], [VnetCountCurrent], [StorageAccountCountMax], [StorageAccountCountCurrent], [VmsPerVnetCountMax], [VmsPerServiceCountMax], [AzureADTenantId], [AzureADClientId], [AzureADClientKey])
SELECT
    [ID], [Name], [Description], [OwnerNamesCSV], [Config], [TagData], [TagID], [AccountType], [ExpirationDate], [CertificateBlob], [CertificateThumbprint], [AccountID], [AccountPassword], [Active], [AzRegion], [AzAffinityGroup], [AzVNet], [AzSubnet], [AzStorageContainerUrl], [ResourceGroup], [CoreCountMax], [CoreCountCurrent], [VnetCountMax], [VnetCountCurrent], [StorageAccountCountMax], [StorageAccountCountCurrent], [VmsPerVnetCountMax], [VmsPerServiceCountMax], [AzureADTenantId], [AzureADClientId],  [AzureADClientKey]
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM ServiceProviderAccounts target
WHERE 
    source.Id = target.Id

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE ServiceProviderAccounts
FROM ServiceProviderAccounts target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.Id = target.Id

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
    RETURN
 
SuccessfulExit:
    PRINT 'Data for ServiceProviderAccounts modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
 