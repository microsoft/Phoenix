CREATE TABLE [dbo].[UserGroupServProdAcctGroupMap] (
    [Id]                    INT           NOT NULL,
    [UserGroupName]         VARCHAR (50)  NULL,
    [ServProvAcctGroupName] VARCHAR (50)  NULL,
    [Config]                VARCHAR (MAX) NULL,
    [IsEnabled]             BIT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

