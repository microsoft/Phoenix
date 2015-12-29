CREATE TABLE [dbo].[AzureRoleSize] (
    [Id]                 INT			IDENTITY (1,1) NOT NULL,
    [Name]				 VARCHAR (MAX)	NOT NULL,
    [CoreCount]			 INT			CONSTRAINT [DF_AzureRoleSize_CoreCount]			DEFAULT (0) NOT NULL,
    [DiskCount]			 INT			CONSTRAINT [DF_AzureRoleSize_DiskCount]			DEFAULT (0) NOT NULL,
    [RamMb]				 FLOAT			CONSTRAINT [DF_AzureRoleSize_RamMb]				DEFAULT (0.0) NOT NULL,
    [DiskSizeRoleOs]	 INT			CONSTRAINT [DF_AzureRoleSize_DiskSizeRoleOs]	DEFAULT (0) NOT NULL,
    [DiskSizeRoleApps]   FLOAT			CONSTRAINT [DF_AzureRoleSize_DiskSizeRoleApps]	DEFAULT (0.0) NOT NULL,
    [DiskSizeVmOs]       INT			CONSTRAINT [DF_AzureRoleSize_DiskSizeVmOs]		DEFAULT (0) NOT NULL,
    [DiskSizeVmTemp]     INT			CONSTRAINT [DF_AzureRoleSize_DiskSizeVmTemp]	DEFAULT (0) NOT NULL,
	[CanBeService]		 BIT			CONSTRAINT [DF_AzureRoleSize_CanBeService]		DEFAULT (1) NOT NULL,
	[CanBeVm]			 BIT			CONSTRAINT [DF_AzureRoleSize_CanBeVm]			DEFAULT (0) NOT NULL,
    CONSTRAINT [PK_AzureRoleSize] PRIMARY KEY CLUSTERED ([Id] ASC)
);

