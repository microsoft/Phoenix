CREATE TABLE [dbo].[IpakVersionMap] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [VersionCode]          VARCHAR (50)  NULL,
    [VersionName]          VARCHAR (200) NULL,
    [AzureRegion]          VARCHAR (50)  NULL,
    [AdDomain]             VARCHAR (200) NULL,
    [Config]               VARCHAR (MAX) NULL,
    [TagData]              VARCHAR (MAX) NULL,
    [IpakDirLocation]      VARCHAR (250) NULL,
    [IpakFullFileLocation] VARCHAR (250) NULL,
    [IsActive]             BIT           NULL,
    [AdminName]            VARCHAR (100) NULL,
    [QfeVersion]           VARCHAR (100) NULL,
    CONSTRAINT [PK_IpakVersionMap] PRIMARY KEY CLUSTERED ([Id] ASC)
);

