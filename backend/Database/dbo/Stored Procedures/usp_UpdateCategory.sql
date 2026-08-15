-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Updates a category by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UpdateCategory]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(50),
    @Color NVARCHAR(20) = NULL,
    @Icon NVARCHAR(50) = NULL,
    @IsSystemDefault BIT = NULL,
    @BudgetAmount MONEY = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    UPDATE [dbo].[Categories]
    SET
        [Name] = @Name,
        [Color] = @Color,
        [Icon] = @Icon,
        [IsSystemDefault] = COALESCE(@IsSystemDefault, [IsSystemDefault]),
        [BudgetAmount] = @BudgetAmount
    OUTPUT inserted.Id INTO @OutputTable
    WHERE [Id] = @Id;

    SELECT
        c.[Id],
        c.[Name],
        c.[Color],
        c.[Icon],
        c.[CreatedAt],
        c.[UpdatedAt],
        c.[IsSystemDefault],
        c.[BudgetAmount]
    FROM [dbo].[Categories] c
    JOIN @OutputTable r ON c.Id = r.Id;
END
