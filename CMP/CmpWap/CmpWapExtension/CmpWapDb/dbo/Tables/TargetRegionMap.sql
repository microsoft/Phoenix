CREATE TABLE [dbo].[TargetRegionMap] (
    [Id]              INT           NOT NULL,
    [DisplayName]     VARCHAR (50)  NULL,
    [AzureReqionName] VARCHAR (50)  NULL,
    [TagData]         VARCHAR (MAX) NULL,
    [IsActive]        BIT           NULL,
    [Config]          VARCHAR (MAX) NULL,
    [Description]     VARCHAR (MAX) NULL,
    CONSTRAINT [PK_TargetRegionMap] PRIMARY KEY CLUSTERED ([Id] ASC)
);

