CREATE TABLE [dbo].[FluRequests] (
    [ID]                 INT            NOT NULL,
    [RequestName]        VARCHAR (100)  NULL,
    [RequestDescription] VARCHAR (1024) NULL,
    [ParentAppName]      VARCHAR (50)   NULL,
    [TargetVmName]       VARCHAR (100)  NULL,
    [SourceServerName]   VARCHAR (100)  NULL,
    [SourceVhdFilesCSV]  VARCHAR (1024) NULL,
    [TargetLocation]     VARCHAR (50)   NULL,
    [WhoRequested]       VARCHAR (100)  NULL,
    [WhenRequested]      DATETIME       NULL,
    [ExceptionMessage]   VARCHAR (1024) NULL,
    [LastStatusUpdate]   DATETIME       NULL,
    [TagData]            VARCHAR (4096) NULL,
    [Status]             VARCHAR (50)   NULL,
    [VmSize]             VARCHAR (50)   NULL,
    [TargetLocationType] VARCHAR (50)   NULL,
    [Active]             BIT            NULL
);


GO
CREATE CLUSTERED INDEX [ci_azure_fixup_dbo_FluRequests]
    ON [dbo].[FluRequests]([ID] ASC);

