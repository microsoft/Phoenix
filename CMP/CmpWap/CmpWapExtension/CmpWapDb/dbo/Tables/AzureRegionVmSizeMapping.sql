CREATE TABLE [dbo].[AzureRegionVmSizeMapping](
	[Id] [int] IDENTITY (1, 1) NOT NULL,
	[VmSizeId] [int] NOT NULL,
	[AzureRegionId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
    CONSTRAINT [PK_AzureRegionVmSizeMapping] PRIMARY KEY CLUSTERED ([Id] ASC));