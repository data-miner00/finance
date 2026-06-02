-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Updates an expense by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UpdateExpenseWithCategoryName]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(255),
    @Amount MONEY,
    @CategoryName NVARCHAR(50) = NULL,
    @Location NVARCHAR(255) = NULL,
    @Description NVARCHAR(255) = NULL,
    @ActionedAt DATETIME2(7) = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    DECLARE @CategoryId UNIQUEIDENTIFIER = NULL;
    SET NOCOUNT ON;

    IF @CategoryName IS NOT NULL
    BEGIN
        SELECT @CategoryId = [Id]
        FROM [dbo].[Categories]
        WHERE [Name] = @CategoryName;

        IF @CategoryId IS NULL
        BEGIN
            SET @CategoryId = NEWID();
            INSERT INTO [dbo].[Categories] ([Id], [Name], [IsSystemDefault])
            VALUES (@CategoryId, @CategoryName, 0);
        END
    END

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
        @CategoryName,
        e.[Amount],
        e.[Location],
        e.[ActionedAt],
        e.[CreatedAt],
        e.[UpdatedAt]
    FROM [dbo].[Expenses] e
    JOIN @OutputTable r ON e.Id = r.Id;
END
