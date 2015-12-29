CREATE TABLE [dbo].[AzureAdminSubscriptionVmOsMapping](
	[Id] [int] IDENTITY (1, 1) NOT NULL,
	[VmOsId] [int] NOT NULL,
	[PlanId] varchar(50) NOT NULL,
	[IsActive] [bit] NOT NULL,
    CONSTRAINT [PK_AzureAdminSubscriptionVmOsMapping] PRIMARY KEY CLUSTERED ([Id] ASC));