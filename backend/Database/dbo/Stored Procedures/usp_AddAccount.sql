-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddAccount]
	@Name NVARCHAR(50),
	@Type NVARCHAR(50),
	@Description NVARCHAR(255) = NULL,
	@Balance MONEY = 0.00
AS
BEGIN
	DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Accounts]
	(
		[Name],
		[Type],
		[Description],
		[Balance]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Name,
		@Type,
		@Description,
		@Balance
	);

	SELECT
		l.[Id],
		[Name],
		[Description],
		[Type],
		[Balance],
		[CreatedAt],
		[UpdatedAt]
	FROM [dbo].[Accounts] l
	JOIN @OutputTable r
	ON l.Id = r.Id;
END