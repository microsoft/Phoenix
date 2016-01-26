CREATE TABLE [dbo].[Containers] (
    [ContainerId]    INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (300) NOT NULL,
    [Code]           NVARCHAR (300) NULL,
    [Type]           NVARCHAR (50)  NOT NULL,
    [HasService]     BIT            NULL,
    [CIOwner]        VARCHAR (150)  NULL,
    [IsActive]       BIT            NOT NULL,
    [SubscriptionId] NVARCHAR (100) NOT NULL,
    [CreatedOn]      DATETIME       NOT NULL,
    [CreatedBy]      NVARCHAR (256) NOT NULL,
    [LastUpdatedOn]  DATETIME       NOT NULL,
    [LastUpdatedBy]  VARCHAR (50)   NOT NULL,
    [Region]         NVARCHAR (50)  NULL,
    [Path]           NVARCHAR (255) NULL,
    [Config]         VARCHAR (MAX)  NULL
);

GO
CREATE CLUSTERED INDEX [ci_azure_fixup_dbo_Containers]
    ON [dbo].[Containers]([ContainerId] ASC);