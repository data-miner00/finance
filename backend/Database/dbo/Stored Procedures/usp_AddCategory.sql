-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddCategory]
	@Name NVARCHAR(50),
	@Color NVARCHAR(20) = NULL,
	@Icon NVARCHAR(50) = NULL,
	@IsSystemDefault BIT = 0
AS
BEGIN
	DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Categories]
	(
		[Name],
		[Color],
		[Icon],
		[IsSystemDefault]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Name,
		@Color,
		@Icon,
		@IsSystemDefault
	);

	SELECT
		l.[Id],
		[Name],
		[Color],
		[Icon],
		[IsSystemDefault],
		[CreatedAt],
		[UpdatedAt]
	FROM [dbo].[Categories] l
	JOIN @OutputTable r
	ON l.Id = r.Id;
END