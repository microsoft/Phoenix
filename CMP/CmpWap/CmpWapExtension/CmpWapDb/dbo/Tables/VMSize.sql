CREATE TABLE [dbo].[VmSize] (
    [VmSizeId]         INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (100) NOT NULL,
    [Description]      NVARCHAR (500) NOT NULL,
    [Cores]            INT            NOT NULL,
    [Memory]           INT            NOT NULL,
    [MaxDataDiskCount] INT            NOT NULL,
    [IsActive]         BIT            NOT NULL,
    [CreatedOn]        DATETIME       CONSTRAINT [DF_VmSize_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]        NVARCHAR (256) CONSTRAINT [DF_VmSize_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [LastUpdatedOn]    DATETIME       CONSTRAINT [DF_VmSize_LastUpdatedOn] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]    VARCHAR (50)   CONSTRAINT [DF_VmSize_LastUpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    CONSTRAINT [PK_VmSize] PRIMARY KEY CLUSTERED ([VmSizeId] ASC)
);

