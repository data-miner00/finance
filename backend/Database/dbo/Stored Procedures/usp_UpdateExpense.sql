-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Updates an expense by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UpdateExpense]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(255),
    @Amount MONEY,
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
        [Name] = @Name,
        [CategoryId] = @CategoryId,
        [Amount] = @Amount,
        [Location] = @Location,
        [Description] = @Description,
        [ActionedAt] = @ActionedAt
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
