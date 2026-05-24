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
	@Description NVARCHAR(255) = NULL
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
		[RecurringAt]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Name,
		@Amount,
		@Type,
		@Description,
		@RecurringAt
	);

	SELECT
		l.[Id],
		[Name],
		[Description],
		[Type],
		[Amount],
		[RecurringAt],
		[CreatedAt],
		[UpdatedAt]
	FROM [dbo].[Recurrings] l
	JOIN @OutputTable r
	ON l.Id = r.Id;
END