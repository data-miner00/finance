CREATE TABLE [dbo].[Recurrings] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_Recurrings_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]            NVARCHAR (50)    NOT NULL,
    [Description]     NVARCHAR (255)   NULL,
    [IsActive]        BIT              CONSTRAINT [DF_Recurrings_IsActive] DEFAULT ((1)) NOT NULL,
    [RecurringAt]     DATETIME2 (7)    NOT NULL,
    [CreatedAt]       DATETIME2 (7)    CONSTRAINT [DF_Recurrings_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]       DATETIME2 (7)    CONSTRAINT [DF_Recurrings_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    [Type]            NVARCHAR (50)    NOT NULL,
    [Amount]          MONEY            NOT NULL DEFAULT (0.00),
    [StartAt]         DATETIME2 (7)    NOT NULL,
    [RecurrenceType]  NVARCHAR (50)    CONSTRAINT [DF_Recurrings_RecurrenceType] DEFAULT ('Monthly') NOT NULL,
    [IntervalValue]   INT              CONSTRAINT [DF_Recurrings_IntervalValue] DEFAULT (1) NOT NULL,
    [DayOfMonth]      INT              NULL,
    CONSTRAINT [PK_Recurrings] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TRIGGER [dbo].[Trigger_Recurrings_OnUpdate]
    ON [dbo].[Recurrings]
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;

        UPDATE [dbo].[Recurrings]
        SET [UpdatedAt] = GETDATE()
        FROM [dbo].[Recurrings] T
        INNER JOIN inserted I ON T.Id = I.Id;
    END;
