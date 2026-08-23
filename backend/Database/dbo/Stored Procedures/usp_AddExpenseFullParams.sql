-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddExpenseFullParams]
	@Id UNIQUEIDENTIFIER,
	@Name NVARCHAR(50),
	@Amount MONEY,
	@Currency NCHAR(3) = 'MYR',
	@ActionedAt DATETIME2(7),
	@CreatedAt DATETIME2(7),
	@UpdatedAt DATETIME2(7),
	@CategoryName NVARCHAR(50) = NULL,
	@Location NVARCHAR(255) = NULL,
	@Description NVARCHAR(255) = NULL,
	@AgentName NVARCHAR(255) = NULL,
	@AccountId UNIQUEIDENTIFIER = NULL,
	@ReceiptImage NVARCHAR(255) = NULL
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
		[Id],
		[Name],
		[CategoryId],
		[Amount],
		[Currency],
		[Location],
		[Description],
		[ActionedAt],
		[CreatedAt],
		[UpdatedAt],
		[AgentName],
		[AccountId],
		[ReceiptImage]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Id,
		@Name,
		@CategoryId,
		@Amount,
		@Currency,
		@Location,
		@Description,
		@ActionedAt,
		@CreatedAt,
		@UpdatedAt,
		@AgentName,
		@AccountId,
		@ReceiptImage
	);

	SELECT
		l.[Id],
		l.[Name],
		l.[Description],
		@CategoryName CategoryName,
		l.[Amount],
		l.[Currency],
		l.[Location],
		l.[ActionedAt],
		l.[CreatedAt],
		l.[UpdatedAt],
		l.[AgentName],
		l.[AccountId],
		a.[Name] AS AccountName,
		l.[ReceiptImage]
	FROM [dbo].[Expenses] l
	JOIN @OutputTable r ON l.Id = r.Id
	LEFT OUTER JOIN [dbo].[Accounts] a ON l.[AccountId] = a.[Id];
END
