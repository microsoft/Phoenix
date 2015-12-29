CREATE TABLE [dbo].[AzureAdminSubscriptionVnet](
	[Id] [int] IDENTITY (1, 1) NOT NULL,
	[SubId] [nvarchar](50) NULL,
	[Subnet] [nvarchar](50) NULL,
	[Gateway] [nvarchar](50) NULL,
	[CircuitName] [nvarchar](50) NULL,
	[VNetType] [nvarchar](50) NULL,
	[VNetName] [nvarchar](50) NULL,
	[IsActive] [Bit],
	[CreatedOn]        DATETIME       CONSTRAINT [DF_AzureAdminSubscriptionVnet_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]        NVARCHAR (256) CONSTRAINT [DF_AzureAdminSubscriptionVnet_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn]    DATETIME       CONSTRAINT [DF_AzureAdminSubscriptionVnet_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]    VARCHAR (50)   CONSTRAINT [DF_AzureAdminSubscriptionVnet_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    CONSTRAINT [PK_AzureAdminSubscriptionVnet] PRIMARY KEY CLUSTERED ([Id] ASC)
		
)  