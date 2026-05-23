CREATE TABLE [dbo].[Recurrings] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Recurrings_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]        NVARCHAR (50)    NOT NULL,
    [Description] NVARCHAR (255)   NULL,
    [IsActive]    BIT              CONSTRAINT [DF_Recurrings_IsActive] DEFAULT ((1)) NOT NULL,
    [RecurringAt] DATETIME2 (7)    NOT NULL,
    [CreatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Recurrings_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Recurrings_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    [Type]        NVARCHAR (50)    NOT NULL,
    CONSTRAINT [PK_Recurrings] PRIMARY KEY CLUSTERED ([Id] ASC)
);

