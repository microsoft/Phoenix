CREATE TABLE [dbo].[ResourceProviderAcctGroup] (
    [ResourceProviderAcctGroupId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]                        VARCHAR (50)   NOT NULL,
    [DomainId]                      INT   NOT NULL,
    [NetworkNICId]                     INT   NOT NULL,
	[EnvironmentTypeId]                INT NOT NULL,
    [IsActive]                    BIT            NOT NULL,
    [CreatedOn]                   DATETIME       CONSTRAINT [DF_ResourceProviderAcctGroup_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]                   NVARCHAR (256) CONSTRAINT [DF_ResourceProviderAcctGroup_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn]               DATETIME       CONSTRAINT [DF_ResourceProviderAcctGroup_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]               VARCHAR (50)   CONSTRAINT [DF_ResourceProviderAcctGroup_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    CONSTRAINT [PK_ResourceProviderAcctGroup] PRIMARY KEY CLUSTERED ([ResourceProviderAcctGroupId] ASC), 
    CONSTRAINT [FK_ResourceProviderAcctGroup_AdDomainMap] FOREIGN KEY ([DomainId]) REFERENCES [AdDomainMap]([Id]),
	CONSTRAINT [FK_ResourceProviderAcctGroup_NetworkNIC] FOREIGN KEY ([NetworkNICId]) REFERENCES [NetworkNIC]([NetworkNICId]),
	CONSTRAINT [FK_ResourceProviderAcctGroup_EnvironmentType] FOREIGN KEY ([EnvironmentTypeId]) REFERENCES [EnvironmentType]([EnvironmentTypeId])
);

