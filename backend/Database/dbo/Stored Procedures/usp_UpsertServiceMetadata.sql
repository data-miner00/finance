-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Upserts the service metadata record.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UpsertServiceMetadata]
    @Name NVARCHAR(50),
    @Description NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    MERGE INTO [dbo].[ServiceMetadata] AS target
    USING (SELECT @Name AS [ServiceName], @Description AS [Description]) AS source
    ON target.[ServiceName] = source.[ServiceName]
    WHEN MATCHED THEN
        UPDATE SET [Description] = source.[Description]
    WHEN NOT MATCHED THEN
        INSERT ([ServiceName], [Description]) VALUES (source.[ServiceName], source.[Description]);

    SELECT
        [Id],
        [ServiceName],
        [Description],
        [CreatedAt],
        [UpdatedAt]
    FROM [dbo].[ServiceMetadata]
    WHERE [ServiceName] = @Name;
END
