CREATE TABLE [dbo].[Categories] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_Categories_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]            NVARCHAR (50)    NOT NULL,
    [CreatedAt]       DATETIME2 (7)    CONSTRAINT [DF_Categories_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]       DATETIME2 (7)    CONSTRAINT [DF_Categories_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    [IsSystemDefault] BIT              CONSTRAINT [DF_Categories_IsSystemDefault] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
);

