CREATE TABLE [dbo].[BadAssets] (
    [Id]                 INT           NOT NULL,
    [AssetName]          VARCHAR (100) NOT NULL,
    [AssetTypeCode]      VARCHAR (50)  NOT NULL,
    [ProblemDescription] VARCHAR (MAX) NULL,
    [WhoReported]        VARCHAR (100) NULL,
    [WhenReported]       DATETIME      NULL,
    [Config]             VARCHAR (MAX) NULL,
    [TagData]            VARCHAR (MAX) NULL,
    [Active]             BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

