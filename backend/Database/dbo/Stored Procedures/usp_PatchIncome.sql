-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Patches an income by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_PatchIncome]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(50) = NULL,
    @Amount MONEY = NULL,
    @Description NVARCHAR(255) = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    UPDATE [dbo].[Incomes]
    SET
        [Name] = COALESCE(@Name, [Name]),
        [Amount] = COALESCE(@Amount, [Amount]),
        [Description] = COALESCE(@Description, [Description])
    OUTPUT inserted.Id INTO @OutputTable
    WHERE [Id] = @Id;

    SELECT
        i.[Id],
        i.[Name],
        i.[Description],
        i.[Amount],
        i.[CreatedAt],
        i.[UpdatedAt]
    FROM [dbo].[Incomes] i
    JOIN @OutputTable r ON i.Id = r.Id;
END
