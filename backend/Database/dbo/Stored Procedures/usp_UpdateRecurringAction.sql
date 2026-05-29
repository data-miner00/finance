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
    @Amount MONEY,
    @StartAt DATETIME2(7),
    @RecurrenceType NVARCHAR(50) = 'Monthly',
    @IntervalValue INT = 1,
    @DayOfMonth INT = NULL
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
        [Amount] = @Amount,
        [StartAt] = @StartAt,
        [RecurrenceType] = @RecurrenceType,
        [IntervalValue] = @IntervalValue,
        [DayOfMonth] = @DayOfMonth
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
        r.[StartAt],
        r.[RecurrenceType],
        r.[IntervalValue],
        r.[DayOfMonth],
        r.[CreatedAt],
        r.[UpdatedAt]
    FROM [dbo].[Recurrings] r
    JOIN @OutputTable t ON r.Id = t.Id;
END
