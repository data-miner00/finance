CREATE TABLE [dbo].[Accounts] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Accounts_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]        NVARCHAR (50)    NOT NULL,
    [Description] NVARCHAR (255)   NULL,
    [Type]        NVARCHAR (50)    NOT NULL,
    [Balance]     MONEY            CONSTRAINT [DF_Accounts_Balance] DEFAULT ((0.00)) NOT NULL,
    [CreatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Accounts_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Accounts_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED ([Id] ASC)
);

