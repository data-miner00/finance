-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddExpenseWithCategoryName]
	@Name NVARCHAR(50),
	@Amount MONEY,
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

    IF @CategoryName IS NOT NULL
	BEGIN
		SELECT @CategoryId = [Id]
		FROM [dbo].[Categories]
		WHERE [Name] = @CategoryName;

		IF @CategoryId IS NULL
		BEGIN
			SET @CategoryId = NEWID();
			INSERT INTO [dbo].[Categories] ([Id], [Name], [IsSystemDefault])
			VALUES (@CategoryId, @CategoryName, 0);
		END
	END

	INSERT INTO [dbo].[Expenses]
	(
		[Name],
		[CategoryId],
		[Amount],
		[Location],
		[Description],
		[AgentName],
		[AccountId]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Name,
		@CategoryId,
		@Amount,
		@Location,
		@Description,
		@AgentName,
		@AccountId
	);

	SELECT
		l.[Id],
		l.[Name],
		l.[Description],
		@CategoryName AS CategoryName,
		l.[Amount],
		l.[Location],
		l.[ActionedAt],
		l.[CreatedAt],
		l.[UpdatedAt],
		l.[AgentName],
		l.[AccountId],
		a.[Name] AS AccountName
	FROM [dbo].[Expenses] l
	JOIN @OutputTable r ON l.Id = r.Id
	LEFT OUTER JOIN [dbo].[Accounts] a ON l.[AccountId] = a.[Id];
END