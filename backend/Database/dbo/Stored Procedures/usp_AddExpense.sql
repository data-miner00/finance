-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddExpense]
	@Name NVARCHAR(50),
	@Amount MONEY,
	@CategoryId UNIQUEIDENTIFIER = NULL,
	@Location NVARCHAR(255) = NULL,
	@Description NVARCHAR(255) = NULL
AS
BEGIN
	DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Expenses]
	(
		[Name],
		[CategoryId],
		[Amount],
		[Location],
		[Description]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Name,
		@CategoryId,
		@Amount,
		@Location,
		@Description
	);

	SELECT
		l.[Id],
		[Name],
		[Description],
		[CategoryId],
		[Amount],
		[Location],
		[Description],
		[CreatedAt],
		[UpdatedAt]
	FROM [dbo].[Expenses] l
	JOIN @OutputTable r
	ON l.Id = r.Id;
END