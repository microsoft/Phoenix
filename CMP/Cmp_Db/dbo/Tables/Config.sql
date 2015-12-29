CREATE TABLE [dbo].[Config] (
    [Id]          INT           NOT NULL,
    [Name]        VARCHAR (50)  NULL,
    [Value]       VARCHAR (MAX) NULL,
    [Description] VARCHAR (200) NULL,
    [Region]      VARCHAR (50)  NULL,
    [Instance]    VARCHAR (50)  NULL,
    [IsActive]    BIT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

