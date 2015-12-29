CREATE TABLE [dbo].[SQLVersion] (
    [SQLVersionId]  INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (100) NOT NULL,
    [Description]   NVARCHAR (500) NOT NULL,
    [IsActive]      BIT            NOT NULL,
    [CreatedOn]     DATETIME       CONSTRAINT [DF_SQLVersion_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]     NVARCHAR (256) CONSTRAINT [DF_SQLVersion_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn] DATETIME       CONSTRAINT [DF_SQLVersion_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy] VARCHAR (50)   CONSTRAINT [DF_SQLVersion_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    CONSTRAINT [PK_SQLVersion] PRIMARY KEY CLUSTERED ([SQLVersionId] ASC)
);

