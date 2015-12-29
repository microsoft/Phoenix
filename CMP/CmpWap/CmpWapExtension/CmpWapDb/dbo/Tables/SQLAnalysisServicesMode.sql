CREATE TABLE [dbo].[SQLAnalysisServicesMode] (
    [SQLAnalysisServicesModeId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]                      NVARCHAR (100) NOT NULL,
    [Description]               NVARCHAR (500) NOT NULL,
    [IsActive]                  BIT            NOT NULL,
    [CreatedOn]                 DATETIME       CONSTRAINT [DF_SQLAnalysisServicesMode_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]                 NVARCHAR (256) CONSTRAINT [DF_SQLAnalysisServicesMode_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn]             DATETIME       CONSTRAINT [DF_SQLAnalysisServicesMode_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]             VARCHAR (50)   CONSTRAINT [DF_SQLAnalysisServicesMode_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    CONSTRAINT [PK_SQLAnalysisServicesMode] PRIMARY KEY CLUSTERED ([SQLAnalysisServicesModeId] ASC)
);

