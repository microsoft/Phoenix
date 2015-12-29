CREATE TABLE [dbo].[VmOsMap] (
    [Id]             INT           NOT NULL,
    [DisplayName]    VARCHAR (MAX) NULL,
    [AzureImageName] VARCHAR (MAX) NULL,
    [TagData]        VARCHAR (MAX) NULL,
    [IsActive]       BIT           NULL,
    [Config]         VARCHAR (MAX) NULL,
    [Description]    VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

