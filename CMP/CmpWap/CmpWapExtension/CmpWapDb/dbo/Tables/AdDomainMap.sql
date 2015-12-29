CREATE TABLE [dbo].[AdDomainMap] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [DomainShortName]      VARCHAR (MAX) NULL,
    [DomainFullName]       VARCHAR (MAX) NULL,
    [JoinCredsUserName]    VARCHAR (MAX) NULL,
    [JoinCredsPasword]     VARCHAR (MAX) NULL,
    [IsActive]             BIT           NULL,
    [Config]               VARCHAR (MAX) NULL,
    [ServerOU]             VARCHAR (200) NULL,
    [WorkstationOU]        VARCHAR (200) NULL,
    [DefaultVmAdminMember] VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

