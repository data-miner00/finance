-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddRecurringAction]
	@Name NVARCHAR(50),
	@Amount MONEY,
	@Type NVARCHAR(50),
	@RecurringAt DATETIME2(7),
	@StartAt DATETIME2(7),
	@Description NVARCHAR(255) = NULL,
	@RecurrenceType NVARCHAR(50) = 'Monthly',
	@IntervalValue INT = 1,
	@DayOfMonth INT = NULL
AS
BEGIN
	DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Recurrings]
	(
		[Name],
		[Amount],
		[Type],
		[Description],
		[RecurringAt],
		[StartAt],
		[RecurrenceType],
		[IntervalValue],
		[DayOfMonth]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Name,
		@Amount,
		@Type,
		@Description,
		@RecurringAt,
		@StartAt,
		@RecurrenceType,
		@IntervalValue,
		@DayOfMonth
	);

	SELECT
		l.[Id],
		[Name],
		[Description],
		[Type],
		[Amount],
		[RecurringAt],
		[StartAt],
		[RecurrenceType],
		[IntervalValue],
		[DayOfMonth],
		[CreatedAt],
		[UpdatedAt]
	FROM [dbo].[Recurrings] l
	JOIN @OutputTable r
	ON l.Id = r.Id;
END