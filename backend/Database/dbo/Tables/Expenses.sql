CREATE TABLE [dbo].[Expenses] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Expenses_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CategoryId]  UNIQUEIDENTIFIER NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Description] NVARCHAR (255)   NOT NULL,
    [Amount]      MONEY            NOT NULL,
    [Location]    NVARCHAR (255)   NULL,
    [ActionedAt]  DATETIME2 (7)    CONSTRAINT [DF_Expenses_ActionedAt] DEFAULT (getdate()) NOT NULL,
    [CreatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Expenses_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Expenses_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Expenses] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Expenses_Categories1] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Hello World', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Expenses', @level2type = N'CONSTRAINT', @level2name = N'FK_Expenses_Categories1';

GO

CREATE TRIGGER [dbo].[Trigger_Expenses_OnUpdate]
    ON [dbo].[Expenses]
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;

        UPDATE [dbo].[Expenses]
        SET [UpdatedAt] = GETDATE()
        FROM [dbo].[Expenses] T
        INNER JOIN inserted I ON T.Id = I.Id;
    END;
