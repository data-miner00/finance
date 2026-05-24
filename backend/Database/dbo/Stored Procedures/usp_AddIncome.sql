-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddIncome]
	@Name NVARCHAR(50),
	@Amount MONEY,
	@Description NVARCHAR(255) = NULL
AS
BEGIN
	DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Incomes]
	(
		[Name],
		[Amount],
		[Description]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Name,
		@Amount,
		@Description
	);

	SELECT
		l.[Id],
		[Name],
		[Description],
		[Amount],
		[CreatedAt],
		[UpdatedAt]
	FROM [dbo].[Incomes] l
	JOIN @OutputTable r
	ON l.Id = r.Id;
END