CREATE TABLE [dbo].[VmMigrationRequests] (
    [ID]                    INT            NOT NULL,
    [VmDeploymentRequestID] INT            NOT NULL,
    [VmSize]                VARCHAR (50)   NULL,
    [TagData]               VARCHAR (MAX)  NULL,
    [TagID]                 INT            NULL,
    [Config]                VARCHAR (MAX)  NULL,
    [TargetVmName]          VARCHAR (256)  NULL,
    [SourceServerName]      VARCHAR (256)  NULL,
    [SourceVhdFilesCSV]     VARCHAR (MAX)  NULL,
    [ExceptionMessage]      VARCHAR (MAX)  NULL,
    [LastStatusUpdate]      DATETIME       NULL,
    [StatusCode]            VARCHAR (50)   NULL,
    [StatusMessage]         VARCHAR (4096) NULL,
    [AgentRegion]           VARCHAR (50)   NULL,
    [AgentName]             VARCHAR (100)  NULL,
    [CurrentStateStartTime] DATETIME       NULL,
    [CurrentStateTryCount]  INT            NULL,
    [Warnings]              VARCHAR (MAX)  NULL,
    [Active]                BIT            NULL,
    CONSTRAINT [PK_VmMigrationRequests_ID] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_VmMigrationRequests_VmDeploymentRequestID] FOREIGN KEY ([VmDeploymentRequestID]) REFERENCES [dbo].[VmDeploymentRequests] ([ID])
);

