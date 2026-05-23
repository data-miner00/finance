CREATE TABLE [dbo].[Categories] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_Categories_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]            NVARCHAR (50)    NOT NULL,
    [CreatedAt]       DATETIME2 (7)    CONSTRAINT [DF_Categories_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]       DATETIME2 (7)    CONSTRAINT [DF_Categories_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    [IsSystemDefault] BIT              CONSTRAINT [DF_Categories_IsSystemDefault] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TRIGGER [dbo].[Trigger_Categories_OnUpdate]
    ON [dbo].[Categories]
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;

        UPDATE [dbo].[Categories]
        SET [UpdatedAt] = GETDATE()
        FROM [dbo].[Categories] T
        INNER JOIN inserted I ON T.Id = I.Id;
    END;
