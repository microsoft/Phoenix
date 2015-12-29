CREATE TABLE [dbo].[OpRequests] (
    [Id]                            INT            NOT NULL,
    [RequestName]                   VARCHAR (512)  NULL,
    [RequestDescription]            VARCHAR (1024) NULL,
    [RequestType]                   VARCHAR (50)   NULL,
    [Config]                        VARCHAR (MAX)  NULL,
    [TargetTypeCode]                VARCHAR (50)   NULL,
    [TargetName]                    VARCHAR (256)  NULL,
    [WhoRequested]                  VARCHAR (100)  NULL,
    [WhenRequested]                 DATETIME       NULL,
    [ExceptionMessage]              VARCHAR (MAX)  NULL,
    [LastStatusUpdate]              DATETIME       NULL,
    [StatusCode]                    VARCHAR (50)   NULL,
    [StatusMessage]                 VARCHAR (4096) NULL,
    [Warnings]                      VARCHAR (MAX)  NULL,
    [ServiceProviderStatusCheckTag] VARCHAR (MAX)  NULL,
    [CurrentStateStartTime]         DATETIME       NULL,
    [CurrentStateTryCount]          INT            NULL,
    [TagData]                       VARCHAR (MAX)  NULL,
    [TagID]                         INT            NULL,
    [Active]                        BIT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

