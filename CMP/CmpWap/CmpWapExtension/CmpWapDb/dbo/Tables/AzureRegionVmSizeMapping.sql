CREATE TABLE [dbo].[AzureRegionVmSizeMapping](
	[Id] [int] IDENTITY (1, 1) NOT NULL,
	[AzureRegionId] [int] NOT NULL,
	[VmSizeId] [int] NOT NULL,
	[AzureSubscriptionId] NVARCHAR(100) NULL,
	[IsActive] [bit] NOT NULL,
    CONSTRAINT [PK_AzureRegionVmSizeMapping] PRIMARY KEY CLUSTERED ([Id] ASC));