CREATE TABLE [dbo].[SequenceRequests]
(
	[Id]						INT IDENTITY (1, 1) NOT NULL,
    [WapSubscriptionID]			VARCHAR (100)  NULL,
    [CmpRequestID]				INT            NULL,
    [ServiceProviderName]		VARCHAR (256)  NULL,
    [ServiceProviderTypeCode]	VARCHAR (100)  NULL,
    [ServiceProviderJobId]		VARCHAR (100)  NULL,
    [TargetName]				VARCHAR (256)  NULL,
    [TargetTypeCode]			VARCHAR (100)  NULL,
    [StatusCode]        VARCHAR (50)   NULL,
    [Config]            VARCHAR (MAX)  NULL,
    [WhoRequested]      VARCHAR (50)   NULL,
    [WhenRequested]     DATETIME       NULL,
    [StatusMessage]     VARCHAR (4096) NULL,
    [ExceptionMessage]  VARCHAR (MAX)  NULL,
    [Warnings]          VARCHAR (MAX)  NULL,
    [LastStatusUpdate]  DATETIME       NULL,
    [Active]            BIT            NULL,
    [TagOpName]         VARCHAR (MAX)  NULL,
    [TagData]           VARCHAR (MAX)  NULL,
    [TagID]             INT            NULL,
    CONSTRAINT [PK_SequenceRequest] PRIMARY KEY CLUSTERED ([Id] ASC)
);


