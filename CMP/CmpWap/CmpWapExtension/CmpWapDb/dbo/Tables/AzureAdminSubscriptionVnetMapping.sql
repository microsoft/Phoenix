CREATE TABLE [dbo].[AzureAdminSubscriptionVnetMapping](
	[Id] [int] IDENTITY (1, 1) NOT NULL,
	[PlanId] [nvarchar](50) NULL,
	[VnetId][int] NOT NULL
    CONSTRAINT [PK_AzureAdminSubscriptionVnetMapping] PRIMARY KEY CLUSTERED ([Id] ASC)	
)  