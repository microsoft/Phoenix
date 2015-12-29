CREATE TABLE [dbo].[NetworkNIC] (
    [NetworkNICId]  INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (100) NOT NULL,
    [Description]   NVARCHAR (500) NOT NULL,
    [IsActive]      BIT            NOT NULL,
    [CreatedOn]     DATETIME       CONSTRAINT [DF_NetworkNIC_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]     NVARCHAR (256) CONSTRAINT [DF_NetworkNIC_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn] DATETIME       CONSTRAINT [DF_NetworkNIC_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy] VARCHAR (50)   CONSTRAINT [DF_NetworkNIC_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
	[ADDomain] VARCHAR (100) NULL,
    [ADDomainId] INT NULL,
	CONSTRAINT [FK_AdDomainMap_NetworkNIC] FOREIGN KEY([ADDomainId]) REFERENCES [dbo].[AdDomainMap] ([Id]), 
    CONSTRAINT [PK_NetworkNIC] PRIMARY KEY CLUSTERED ([NetworkNICId] ASC)
);



