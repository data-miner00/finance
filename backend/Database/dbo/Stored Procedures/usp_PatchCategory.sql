-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Patches a category by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_PatchCategory]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(50) = NULL,
    @IsSystemDefault BIT = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    UPDATE [dbo].[Categories]
    SET
        [Name] = COALESCE(@Name, [Name]),
        [IsSystemDefault] = COALESCE(@IsSystemDefault, [IsSystemDefault])
    OUTPUT inserted.Id INTO @OutputTable
    WHERE [Id] = @Id;

    SELECT
        c.[Id],
        c.[Name],
        c.[CreatedAt],
        c.[UpdatedAt],
        c.[IsSystemDefault]
    FROM [dbo].[Categories] c
    JOIN @OutputTable r ON c.Id = r.Id;
END
