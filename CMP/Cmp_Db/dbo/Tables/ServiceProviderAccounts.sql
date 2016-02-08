CREATE TABLE [dbo].[ServiceProviderAccounts] (
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
	[AzureADTenantId]			VARCHAR(50)		NULL,
	[AzureADClientId]			VARCHAR(50)		NULL,
	[AzureADClientKey]			VARCHAR(MAX)		NULL	
);


GO
CREATE CLUSTERED INDEX [ServiceProviderAccounts_Index]
    ON [dbo].[ServiceProviderAccounts]([ID] ASC);

