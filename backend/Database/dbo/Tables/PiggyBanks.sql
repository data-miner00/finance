CREATE TABLE [dbo].[PiggyBanks] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_PiggyBanks_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]        NVARCHAR (50)    NOT NULL,
    [Description] NVARCHAR (255)   NULL,
    [Amount]      MONEY            CONSTRAINT [DF_PiggyBanks_Amount] DEFAULT ((0.00)) NOT NULL,
    [Target]      MONEY            NOT NULL,
    [Deadline]    DATETIME2 (7)    NULL,
    [CreatedAt]   DATETIME2 (7)    CONSTRAINT [DF_PiggyBanks_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME2 (7)    CONSTRAINT [DF_PiggyBanks_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_PiggyBanks] PRIMARY KEY CLUSTERED ([Id] ASC)
);

