CREATE TABLE [dbo].[Incomes] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Incomes_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]        NVARCHAR (50)    NOT NULL,
    [Description] NVARCHAR (255)   NULL,
    [Amount]      MONEY            CONSTRAINT [DF_Incomes_Amount] DEFAULT ((0.00)) NOT NULL,
    [CreatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Incomes_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Incomes_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Incomes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

