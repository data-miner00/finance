-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Patches a person by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_PatchPerson]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(50) = NULL,
    @Alias NVARCHAR(50) = NULL,
    @Description NVARCHAR(255) = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    UPDATE [dbo].[People]
    SET
        [Name] = COALESCE(@Name, [Name]),
        [Alias] = COALESCE(@Alias, [Alias]),
        [Description] = COALESCE(@Description, [Description])
    OUTPUT inserted.Id INTO @OutputTable
    WHERE [Id] = @Id;

    SELECT
        i.[Id],
        i.[Name],
        i.[Description],
        i.[Alias],
        i.[CreatedAt],
        i.[UpdatedAt]
    FROM [dbo].[People] i
    JOIN @OutputTable r ON i.Id = r.Id;
END
