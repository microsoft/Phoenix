CREATE TABLE [dbo].[ServerRoleDriveMapping] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [ServerRoleId]  INT            NOT NULL,
    [Drive]         CHAR (1)       NOT NULL,
    [MemoryInGB]    INT            NOT NULL,
    [CreatedOn]     DATETIME       CONSTRAINT [DF_ServerRoleDataDiskMapping_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]     NVARCHAR (256) CONSTRAINT [DF_ServerRoleDataDiskMapping_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn] DATETIME       CONSTRAINT [DF_ServerRoleDataDiskMapping_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy] VARCHAR (50)   CONSTRAINT [DF_ServerRoleDataDiskMapping_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    CONSTRAINT [PK_ServerRoleDataDiskMapping] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ServerRoleDriveMapping_ServerRole] FOREIGN KEY ([ServerRoleId]) REFERENCES [dbo].[ServerRole] ([ServerRoleId])
);

