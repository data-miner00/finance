-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddPiggyBank]
	@Name NVARCHAR(50),
	@Amount MONEY,
	@Target MONEY,
	@Description NVARCHAR(255) = NULL,
	@Deadline DATETIME2(7) = NULL
AS
BEGIN
	DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
	SET NOCOUNT ON;

    INSERT INTO [dbo].[PiggyBanks]
	(
		[Name],
		[Amount],
		[Target],
		[Description],
		[Deadline]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Name,
		@Amount,
		@Target,
		@Description,
		@Deadline
	);

	SELECT
		l.[Id],
		[Name],
		[Description],
		[Target],
		[Amount],
		[Deadline],
		[CreatedAt],
		[UpdatedAt]
	FROM [dbo].[PiggyBanks] l
	JOIN @OutputTable r
	ON l.Id = r.Id;
END