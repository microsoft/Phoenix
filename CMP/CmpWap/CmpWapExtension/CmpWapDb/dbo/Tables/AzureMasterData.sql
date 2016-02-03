CREATE TABLE [dbo].[AzureMasterData](
	[Id] [int] IDENTITY (1, 1) NOT NULL,
	[OsName] [nvarchar](100) NULL,
	[RegionName] [nvarchar](100) NULL,
	[VnetName] [nvarchar](100) NULL,
	[VmSize] [nvarchar](100) NULL,
	[StorageType] [nvarchar](100) NULL,
	[Description] [nvarchar](500) NULL,
    [CreatedOn]        DATETIME       CONSTRAINT [DF_AzureMasterData_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]        NVARCHAR (256) CONSTRAINT [DF_AzureMasterData_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn]    DATETIME       CONSTRAINT [DF_AzureMasterData_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]    VARCHAR (50)   CONSTRAINT [DF_AzureMasterData_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
 CONSTRAINT [PK_AzureMasterData] PRIMARY KEY CLUSTERED ([Id] ASC))