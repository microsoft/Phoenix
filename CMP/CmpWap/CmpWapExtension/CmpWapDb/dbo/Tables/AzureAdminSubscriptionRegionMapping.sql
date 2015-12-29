CREATE TABLE [dbo].[AzureAdminSubscriptionRegionMapping](
	[Id] [int] IDENTITY (1, 1) NOT NULL,
	[PlanId] [nvarchar](50) NOT NULL,
	[AzureRegionId][int] NOT NULL,
	[IsActive][bit] NOT NULL
	CONSTRAINT [PK_AzureAdminSubscriptionRegionMapping] PRIMARY KEY CLUSTERED ([Id] ASC))