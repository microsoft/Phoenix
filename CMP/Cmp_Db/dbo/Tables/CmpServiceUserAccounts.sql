CREATE TABLE [dbo].[CmpServiceUserAccounts] (
    [Id]                        INT            NOT NULL,
    [DisplayName]               VARCHAR (100)  NULL,
    [Name]                      VARCHAR (50)   NULL,
    [Password]                  VARCHAR (1024) NULL,
    [Config]                    VARCHAR (MAX)  NULL,
    [TagData]                   VARCHAR (MAX)  NULL,
    [Domain]                    VARCHAR (50)   NULL,
    [AssociatedCorpAccountName] VARCHAR (100)  NULL,
    [StatusCode]                VARCHAR (50)   NULL,
    [AccountTypeCode]           VARCHAR (50)   NULL,
    [IsActive]                  BIT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

