-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Updates an income by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UpdateIncome]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(50),
    @Amount MONEY,
    @Currency NCHAR(3),
    @Description NVARCHAR(255) = NULL,
    @AccountId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    UPDATE [dbo].[Incomes]
    SET
        [Name] = @Name,
        [Amount] = @Amount,
        [Currency] = @Currency,
        [Description] = @Description,
        [AccountId] = @AccountId
    OUTPUT inserted.Id INTO @OutputTable
    WHERE [Id] = @Id;

    SELECT
        i.[Id],
        i.[Name],
        i.[Description],
        i.[Amount],
        i.[Currency],
        i.[ActionedAt],
        i.[CreatedAt],
        i.[UpdatedAt],
        i.[AccountId],
        a.[Name] AS AccountName
    FROM [dbo].[Incomes] i
    JOIN @OutputTable r ON i.Id = r.Id
    LEFT OUTER JOIN [dbo].[Accounts] a ON i.[AccountId] = a.[Id];
END
