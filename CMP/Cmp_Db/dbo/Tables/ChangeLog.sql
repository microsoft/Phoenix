CREATE TABLE [dbo].[ChangeLog] (
    [ID]         INT            NOT NULL,
    [RequestID]  INT            NULL,
    [When]       DATETIME       NULL,
    [StatusCode] VARCHAR (50)   NULL,
    [Message]    VARCHAR (MAX)  NULL,
    [TagData]    VARCHAR (MAX)  NULL,
    [ConfigFrom] VARCHAR (MAX)  NULL,
    [ConfigTo]   VARCHAR (MAX)  NULL,
    [Who]        VARCHAR (1024) NULL
);


GO
CREATE CLUSTERED INDEX [ChangeLog_Index]
    ON [dbo].[ChangeLog]([ID] ASC);

