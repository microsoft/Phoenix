CREATE TABLE [dbo].[AzureAdminSubscriptionMapping](
	[Id] [int] IDENTITY (1, 1) NOT NULL,
	[SubId] [int] NOT NULL,
	[PlanId] [nvarchar](50) NULL,	
	[IsActive][Bit], 
    CONSTRAINT [PK_AzureAdminSubscriptionMapping] PRIMARY KEY CLUSTERED ([Id] ASC)
)  