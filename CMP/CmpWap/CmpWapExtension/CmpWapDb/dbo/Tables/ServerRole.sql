CREATE TABLE [dbo].[ServerRole] (
    [ServerRoleId]  INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (100) NOT NULL,
    [Description]   NVARCHAR (500) NOT NULL,
    [IsActive]      BIT            NOT NULL,
    [CreatedOn]     DATETIME       CONSTRAINT [DF_ServerRole_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]     NVARCHAR (256) CONSTRAINT [DF_ServerRole_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn] DATETIME       CONSTRAINT [DF_ServerRole_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy] VARCHAR (50)   CONSTRAINT [DF_ServerRole_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    CONSTRAINT [PK_ServerRole] PRIMARY KEY CLUSTERED ([ServerRoleId] ASC)
);

