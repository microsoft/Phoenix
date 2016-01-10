CREATE TABLE [dbo].[Application] (
    [ApplicationId] INT IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (300) NOT NULL,
    [Code]          NVARCHAR (300) NOT NULL,
    [HasService]    BIT            NOT NULL,
    [CIOwner]       VARCHAR (150)  NULL,
    [IsActive]      BIT            NOT NULL,
	[SubscriptionId] NVARCHAR (100) NOT NULL,
    [CreatedOn]     DATETIME       CONSTRAINT [DF_Application_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]     NVARCHAR (256) CONSTRAINT [DF_Application_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn] DATETIME       CONSTRAINT [DF_Application_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy] VARCHAR (50)   CONSTRAINT [DF_Application_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
	[Region]        NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED ([ApplicationId] ASC)
);

