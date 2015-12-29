CREATE TABLE [dbo].[IISRoleService] (
    [IISRoleServiceId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (100) NOT NULL,
    [Description]      NVARCHAR (500) NOT NULL,
    [IsActive]         BIT            NOT NULL,
    [CreatedOn]        DATETIME       CONSTRAINT [DF_IISRoleService_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]        NVARCHAR (256) CONSTRAINT [DF_IISRoleService_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn]    DATETIME       CONSTRAINT [DF_IISRoleService_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]    VARCHAR (50)   CONSTRAINT [DF_IISRoleService_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    CONSTRAINT [PK_IISRoleService] PRIMARY KEY CLUSTERED ([IISRoleServiceId] ASC)
);

