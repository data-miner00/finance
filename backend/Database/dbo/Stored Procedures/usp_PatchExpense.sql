-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Patches an expense by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_PatchExpense]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(255) = NULL,
    @Amount MONEY = NULL,
    @CategoryId UNIQUEIDENTIFIER = NULL,
    @Location NVARCHAR(255) = NULL,
    @Description NVARCHAR(255) = NULL,
    @ActionedAt DATETIME2(7) = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    UPDATE [dbo].[Expenses]
    SET
        [Name] = COALESCE(@Name, [Name]),
        [CategoryId] = COALESCE(@CategoryId, [CategoryId]),
        [Amount] = COALESCE(@Amount, [Amount]),
        [Location] = COALESCE(@Location, [Location]),
        [Description] = COALESCE(@Description, [Description]),
        [ActionedAt] = COALESCE(@ActionedAt, [ActionedAt])
    OUTPUT inserted.Id INTO @OutputTable
    WHERE [Id] = @Id;

    SELECT
        e.[Id],
        e.[Name],
        e.[Description],
        e.[CategoryId],
        e.[Amount],
        e.[Location],
        e.[ActionedAt],
        e.[CreatedAt],
        e.[UpdatedAt]
    FROM [dbo].[Expenses] e
    JOIN @OutputTable r ON e.Id = r.Id;
END
