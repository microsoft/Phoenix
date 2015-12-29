CREATE TABLE [dbo].[AppMap] (
    [Id]          INT           NOT NULL,
    [AppIdCode]   VARCHAR (50)  NULL,
    [Name]        VARCHAR (MAX) NULL,
    [DisplayName] VARCHAR (MAX) NULL,
    [Description] VARCHAR (MAX) NULL,
    [Config]      VARCHAR (MAX) NULL,
    [TagData]     VARCHAR (MAX) NULL,
    [TagId]       INT           NULL,
    [IsActive]    BIT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

