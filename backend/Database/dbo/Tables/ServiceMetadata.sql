CREATE TABLE [dbo].[ServiceMetadata] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_ServiceMetadata_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [ServiceName] NVARCHAR (50)    NOT NULL,
    [Description] NVARCHAR (255)   NULL,
    [CreatedAt]   DATETIME2 (7)    CONSTRAINT [DF_ServiceMetadata_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME2 (7)    CONSTRAINT [DF_ServiceMetadata_UpdatedAt] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_ServiceMetadata] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_ServiceMetadata_ServiceName] UNIQUE ([ServiceName])
);

GO

CREATE TRIGGER [dbo].[Trigger_ServiceMetadata_OnUpdate]
    ON [dbo].[ServiceMetadata]
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;

        UPDATE [dbo].[ServiceMetadata]
        SET [UpdatedAt] = GETDATE()
        FROM [dbo].[ServiceMetadata] T
        INNER JOIN inserted I ON T.Id = I.Id;
    END;
