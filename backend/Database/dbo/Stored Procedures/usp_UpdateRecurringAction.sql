-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Updates a recurring action by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UpdateRecurringAction]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(50),
    @Description NVARCHAR(255) = NULL,
    @IsActive BIT,
    @RecurringAt DATETIME2(7),
    @Type NVARCHAR(50),
    @Amount MONEY
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    UPDATE [dbo].[Recurrings]
    SET
        [Name] = @Name,
        [Description] = @Description,
        [IsActive] = @IsActive,
        [RecurringAt] = @RecurringAt,
        [Type] = @Type,
        [Amount] = @Amount
    OUTPUT inserted.Id INTO @OutputTable
    WHERE [Id] = @Id;

    SELECT
        r.[Id],
        r.[Name],
        r.[Description],
        r.[IsActive],
        r.[RecurringAt],
        r.[Type],
        r.[Amount],
        r.[CreatedAt],
        r.[UpdatedAt]
    FROM [dbo].[Recurrings] r
    JOIN @OutputTable t ON r.Id = t.Id;
END
