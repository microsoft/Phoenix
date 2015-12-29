CREATE TABLE [dbo].[WapSubscriptionGroupMembership] (
    [Id]                INT           NOT NULL,
    [WapSubscriptionID] VARCHAR (100) NOT NULL,
    [GroupID]           INT           NOT NULL,
    [GroupName]         VARCHAR (100) NULL,
    [Config]            VARCHAR (MAX) NULL,
    [TagData]           VARCHAR (MAX) NULL,
    [TagId]             INT           NULL,
    [IsActive]          BIT           NOT NULL,
    CONSTRAINT [PK_WapSubscriptionGroupMembershipID] PRIMARY KEY CLUSTERED ([Id] ASC)
);

