CREATE TABLE [dbo].[SQLCollation] (
    [SQLCollationId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (100) NOT NULL,
    [Description]    NVARCHAR (500) NOT NULL,
    [IsActive]       BIT            NOT NULL,
    [CreatedOn]      DATETIME       CONSTRAINT [DF_SQLCollation_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]      NVARCHAR (256) CONSTRAINT [DF_SQLCollation_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn]  DATETIME       CONSTRAINT [DF_SQLCollation_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]  VARCHAR (50)   CONSTRAINT [DF_SQLCollation_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    CONSTRAINT [PK_SQLCollation] PRIMARY KEY CLUSTERED ([SQLCollationId] ASC)
);

