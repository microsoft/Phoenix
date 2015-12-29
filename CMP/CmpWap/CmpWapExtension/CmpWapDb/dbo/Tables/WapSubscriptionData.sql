CREATE TABLE [dbo].[WapSubscriptionData] (
    [Id]                           INT           NOT NULL,
    [WapSubscriptionID]            VARCHAR (100) NOT NULL,
    [DefaultObjectCreationGroupID] INT           NOT NULL,
    [Config]                       VARCHAR (MAX) NULL,
    [TagData]                      VARCHAR (MAX) NULL,
    [TagId]                        INT           NULL,
    [IsActive]                     BIT           NOT NULL,
    CONSTRAINT [PK_WapSubscriptionDataId] PRIMARY KEY CLUSTERED ([Id] ASC)
);

