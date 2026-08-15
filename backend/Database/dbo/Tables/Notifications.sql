CREATE TABLE [dbo].[Notifications] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Notifications_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Type]        NVARCHAR (50)    NOT NULL,
    [Title]       NVARCHAR (200)   NOT NULL,
    [Message]     NVARCHAR (500)   NOT NULL,
    [IsRead]      BIT              CONSTRAINT [DF_Notifications_IsRead] DEFAULT ((0)) NOT NULL,
    [EntityType]  NVARCHAR (50)    NULL,
    [EntityId]    NVARCHAR (50)    NULL,
    [CreatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Notifications_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Notifications_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TRIGGER [dbo].[Trigger_Notifications_OnUpdate]
    ON [dbo].[Notifications]
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;

        UPDATE [dbo].[Notifications]
        SET [UpdatedAt] = GETDATE()
        FROM [dbo].[Notifications] T
        INNER JOIN inserted I ON T.Id = I.Id;
    END;

GO

CREATE NONCLUSTERED INDEX [IX_Notifications_IsRead] ON [dbo].[Notifications]([IsRead]);

GO

CREATE NONCLUSTERED INDEX [IX_Notifications_Dedup] ON [dbo].[Notifications]([EntityType], [EntityId], [Type], [CreatedAt]);
