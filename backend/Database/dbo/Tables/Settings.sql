CREATE TABLE [dbo].[Settings] (
    [Id]        UNIQUEIDENTIFIER CONSTRAINT [DF_Settings_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Key]       NVARCHAR (100)   NOT NULL,
    [Value]     NVARCHAR (MAX)   NOT NULL,
    [CreatedAt] DATETIME2 (7)    CONSTRAINT [DF_Settings_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt] DATETIME2 (7)    CONSTRAINT [DF_Settings_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Settings_Key] UNIQUE ([Key])
);

GO

CREATE TRIGGER [dbo].[Trigger_Settings_OnUpdate]
    ON [dbo].[Settings]
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;

        UPDATE [dbo].[Settings]
        SET [UpdatedAt] = GETDATE()
        FROM [dbo].[Settings] T
        INNER JOIN inserted I ON T.Id = I.Id;
    END;
