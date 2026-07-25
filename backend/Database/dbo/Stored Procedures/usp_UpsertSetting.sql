-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Inserts a setting if its key doesn't exist yet, otherwise updates its value.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UpsertSetting]
    @Key NVARCHAR(100),
    @Value NVARCHAR(MAX)
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    MERGE INTO [dbo].[Settings] AS target
    USING (SELECT @Key AS [Key], @Value AS [Value]) AS source
    ON target.[Key] = source.[Key]
    WHEN MATCHED THEN
        UPDATE SET [Value] = source.[Value]
    WHEN NOT MATCHED THEN
        INSERT ([Key], [Value]) VALUES (source.[Key], source.[Value])
    OUTPUT inserted.Id INTO @OutputTable;

    SELECT
        s.[Id],
        s.[Key],
        s.[Value],
        s.[CreatedAt],
        s.[UpdatedAt]
    FROM [dbo].[Settings] s
    JOIN @OutputTable r ON s.Id = r.Id;
END
