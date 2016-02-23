CREATE TABLE [dbo].[AzureRegionVmOsMapping](
	[Id] [int] IDENTITY (1, 1) NOT NULL,
	[AzureRegionId] [int] NOT NULL,
	[VmOsId] [int] NOT NULL,
	[AzureSubscriptionId] NVARCHAR(100) NULL,
	[IsActive] [bit] NOT NULL,
    CONSTRAINT [PK_AzureRegionVmOsMapping] PRIMARY KEY CLUSTERED ([Id] ASC));