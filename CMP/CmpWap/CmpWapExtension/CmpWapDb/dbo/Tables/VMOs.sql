CREATE TABLE [dbo].[VmOs] (
    [VmOsId]         INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (250) NOT NULL,
    [Description]    NVARCHAR (500) NULL,
    [OsFamily]       NVARCHAR (100)  NULL,
    [AzureImageName] VARCHAR (MAX)  NULL,
    [IsCustomImage]  BIT            NOT NULL,
    [IsActive]       BIT            NOT NULL,
    [CreatedOn]      DATETIME       CONSTRAINT [DF_VmOs_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]      NVARCHAR (256) CONSTRAINT [DF_VmOs_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn]  DATETIME       CONSTRAINT [DF_VmOs_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]  VARCHAR (50)   CONSTRAINT [DF_VmOs_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
	[AzureImagePublisher] [varchar](100) NULL,
	[AzureImageOffer] [varchar](100) NULL,
	[AzureWindowsOSVersion] [varchar](100) NULL
    CONSTRAINT [PK_VmOs] PRIMARY KEY CLUSTERED ([VmOsId] ASC)
);

