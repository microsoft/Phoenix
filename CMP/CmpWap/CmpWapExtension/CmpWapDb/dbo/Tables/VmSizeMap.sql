CREATE TABLE [dbo].[VmSizeMap] (
    [Id]            INT           NOT NULL,
    [DisplayName]   VARCHAR (MAX) NULL,
    [AzureSizeName] VARCHAR (MAX) NULL,
    [TagData]       VARCHAR (MAX) NULL,
    [IsActive]      BIT           NULL,
    [Config]        VARCHAR (MAX) NULL,
    [Description]   VARCHAR (MAX) NULL,
    [CpuCoreCount]  INT           NULL,
    [RamMB]         INT           NULL,
    [DiskSizeOS]    INT           NULL,
    [DiskSizeTemp]  INT           NULL,
    [DataDiskCount] INT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

