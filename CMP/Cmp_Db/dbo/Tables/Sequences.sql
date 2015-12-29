CREATE TABLE [dbo].[Sequences] (
    [Id]            INT           NOT NULL,
    [SequenceName]  VARCHAR (100) NULL,
    [SequenceOrder] INT           NULL,
    [Config]        VARCHAR (MAX) NULL,
    [TagData]       VARCHAR (MAX) NULL
);


GO
CREATE CLUSTERED INDEX [ci_azure_fixup_dbo_Sequences]
    ON [dbo].[Sequences]([Id] ASC);

