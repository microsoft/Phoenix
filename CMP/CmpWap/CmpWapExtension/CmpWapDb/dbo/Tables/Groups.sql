CREATE TABLE [dbo].[Groups] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [GroupName]        VARCHAR (100) NOT NULL,
    [GroupDescription] VARCHAR (MAX) NULL,
    [Config]           VARCHAR (MAX) NULL,
    [TagData]          VARCHAR (MAX) NULL,
    [TagId]            INT           NULL,
    [IsActive]         BIT           NOT NULL,
    CONSTRAINT [PK_GroupsID] PRIMARY KEY CLUSTERED ([Id] ASC)
);

