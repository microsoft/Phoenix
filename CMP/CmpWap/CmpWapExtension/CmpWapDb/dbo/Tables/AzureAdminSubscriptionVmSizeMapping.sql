CREATE TABLE [dbo].[AzureAdminSubscriptionVmSizeMapping](
	[Id] [int] IDENTITY (1, 1) NOT NULL,
	[VmSizeId] [int] NOT NULL,
	[PlanId] varchar(50) NOT NULL,
	[IsActive] [bit] NOT NULL,
    CONSTRAINT [PK_AzureAdminSubscriptionVmSizeMapping] PRIMARY KEY CLUSTERED ([Id] ASC));