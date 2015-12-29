CREATE TABLE [dbo].[AzureRegion] (
    [AzureRegionId]    INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (100) NOT NULL,
    [Description]      NVARCHAR (500) NOT NULL,
    [OsImageContainer] VARCHAR (MAX)  NULL,
    [IsActive]         BIT            NOT NULL,
    [CreatedOn]        DATETIME       CONSTRAINT [DF_AzureRegion_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]        NVARCHAR (256) CONSTRAINT [DF_AzureRegion_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn]    DATETIME       CONSTRAINT [DF_AzureRegion_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]    VARCHAR (50)   CONSTRAINT [DF_AzureRegion_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    CONSTRAINT [PK_AzureRegion] PRIMARY KEY CLUSTERED ([AzureRegionId] ASC)
);

