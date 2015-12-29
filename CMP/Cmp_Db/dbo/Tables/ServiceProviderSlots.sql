CREATE TABLE [dbo].[ServiceProviderSlots] (
    [Id]                       INT            NOT NULL,
    [ServiceProviderAccountId] INT            NULL,
    [TypeCode]                 VARCHAR (50)   NULL,
    [ServiceProviderSlotName]  VARCHAR (250)  NULL,
    [Description]              VARCHAR (1024) NULL,
    [TagData]                  VARCHAR (MAX)  NULL,
    [TagInt]                   INT            NULL,
    [Active]                   BIT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

