-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Updates an expense by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UpdateExpenseWithCategoryName]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(255),
    @Amount MONEY,
    @ActionedAt DATETIME2(7),
    @CategoryName NVARCHAR(50) = NULL,
    @Location NVARCHAR(255) = NULL,
    @Description NVARCHAR(255) = NULL,
    @AgentName NVARCHAR(255) = NULL,
    @AccountId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    DECLARE @CategoryId UNIQUEIDENTIFIER = NULL;
    SET NOCOUNT ON;

	SELECT @CategoryId = [Id]
	FROM [dbo].[Categories]
	WHERE [Name] = @CategoryName;

    UPDATE [dbo].[Expenses]
    SET
        [Name] = @Name,
        [CategoryId] = @CategoryId,
        [Amount] = @Amount,
        [Location] = @Location,
        [Description] = @Description,
        [ActionedAt] = @ActionedAt,
        [AgentName] = @AgentName,
        [AccountId] = @AccountId
    OUTPUT inserted.Id INTO @OutputTable
    WHERE [Id] = @Id;

    SELECT
        e.[Id],
        e.[Name],
        e.[Description],
        @CategoryName AS CategoryName,
        e.[Amount],
        e.[Location],
        e.[ActionedAt],
        e.[CreatedAt],
        e.[UpdatedAt],
        e.[AgentName],
        e.[AccountId],
        a.[Name] AS AccountName
    FROM [dbo].[Expenses] e
    JOIN @OutputTable r ON e.Id = r.Id
    LEFT OUTER JOIN [dbo].[Accounts] a ON e.[AccountId] = a.[Id];
END
