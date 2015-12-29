CREATE TABLE [dbo].[WAPMAPPINGDATA] (
    [Id]                INT           NOT NULL,
    [WapSubscriptionID] VARCHAR (100) NULL,
    [TargetVmName]      VARCHAR (256) NULL,
    [AdminUser]         VARCHAR (100) NULL
);


GO
CREATE UNIQUE CLUSTERED INDEX [Idx_WAPMAPPINGDATA]
    ON [dbo].[WAPMAPPINGDATA]([Id] ASC);

