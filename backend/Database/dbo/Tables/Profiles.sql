CREATE TABLE [dbo].[Profiles] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Profiles_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Username]    NVARCHAR (50)    NOT NULL,
    [FirstName]   NVARCHAR (50)    NULL,
    [LastName]    NVARCHAR (50)    NULL,
    [Email]       NVARCHAR (255)   NULL,
    [Bio]         NVARCHAR (500)   NULL,
    [CompanyName] NVARCHAR (100)   NULL,
    [WebsiteUrl]  NVARCHAR (255)   NULL,
    [AvatarImage] NVARCHAR (500)   NULL,
    [CreatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Profiles_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME2 (7)    CONSTRAINT [DF_Profiles_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Profiles] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE TRIGGER [dbo].[Trigger_Profiles_OnUpdate]
    ON [dbo].[Profiles]
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;

        UPDATE [dbo].[Profiles]
        SET [UpdatedAt] = GETDATE()
        FROM [dbo].[Profiles] T
        INNER JOIN inserted I ON T.Id = I.Id;
    END;
