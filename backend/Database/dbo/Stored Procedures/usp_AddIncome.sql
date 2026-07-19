-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddIncome]
	@Name NVARCHAR(50),
	@Amount MONEY,
	@Description NVARCHAR(255) = NULL,
	@AccountId UNIQUEIDENTIFIER = NULL
AS
BEGIN
	DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Incomes]
	(
		[Name],
		[Amount],
		[Description],
		[AccountId]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Name,
		@Amount,
		@Description,
		@AccountId
	);

	SELECT
		l.[Id],
		l.[Name],
		l.[Description],
		l.[Amount],
		l.[ActionedAt],
		l.[CreatedAt],
		l.[UpdatedAt],
		l.[AccountId],
		a.[Name] AS AccountName
	FROM [dbo].[Incomes] l
	JOIN @OutputTable r
	ON l.Id = r.Id
	LEFT OUTER JOIN [dbo].[Accounts] a ON l.[AccountId] = a.[Id];
END