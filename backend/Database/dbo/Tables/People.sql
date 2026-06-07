CREATE TABLE [dbo].[People] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_People_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]        NVARCHAR (50)    NOT NULL,
    [Alias]       NVARCHAR (50)    NULL,
    [Description] NVARCHAR (255)   NULL,
    [CreatedAt]   DATETIME2 (7)    CONSTRAINT [DF_People_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME2 (7)    CONSTRAINT [DF_People_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_People] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TRIGGER [dbo].[Trigger_People_OnUpdate]
    ON [dbo].[People]
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;

        UPDATE [dbo].[People]
        SET [UpdatedAt] = GETDATE()
        FROM [dbo].[People] T
        INNER JOIN inserted I ON T.Id = I.Id;
    END;
