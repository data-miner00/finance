CREATE TABLE [dbo].[Taxes] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Taxes_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]        NVARCHAR (50)    NOT NULL,
    [Description] NVARCHAR (255)   NULL,
    [Amount]      MONEY            NOT NULL,
    [CreatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Taxes_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Taxes_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Taxes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TRIGGER [dbo].[Trigger_Taxes_OnUpdate]
    ON [dbo].[Taxes]
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;

        UPDATE [dbo].[Taxes]
        SET [UpdatedAt] = GETDATE()
        FROM [dbo].[Taxes] T
        INNER JOIN inserted I ON T.Id = I.Id;
    END;
