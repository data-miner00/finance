-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddCategory]
	@Name NVARCHAR(50),
	@IsSystemDefault BIT = 0
AS
BEGIN
	DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Categories]
	(
		[Name],
		[IsSystemDefault]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Name,
		@IsSystemDefault
	);

	SELECT
		l.[Id],
		[Name],
		[IsSystemDefault],
		[CreatedAt],
		[UpdatedAt]
	FROM [dbo].[Categories] l
	JOIN @OutputTable r
	ON l.Id = r.Id;
END