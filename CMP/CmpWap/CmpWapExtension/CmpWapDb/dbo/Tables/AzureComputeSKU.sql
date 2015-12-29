CREATE TABLE [dbo].[AzureComputeSKU] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (100) NOT NULL,
    [Description]      NVARCHAR (500) NOT NULL,
    [Cores]            INT            NOT NULL,
    [Memory]           INT            NOT NULL,
    [MaxDataDiskCount] INT            NOT NULL,
    [IsActive]         BIT            NOT NULL,
    [CreatedOn]        DATETIME       CONSTRAINT [DF_AzureComputeSKU_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]        NVARCHAR (256) CONSTRAINT [DF_AzureComputeSKU_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn]    DATETIME       CONSTRAINT [DF_AzureComputeSKU_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]    VARCHAR (50)   CONSTRAINT [DF_AzureComputeSKU_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    CONSTRAINT [PK_AzureComputeSKU] PRIMARY KEY CLUSTERED ([Id] ASC)
);

