-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddPerson]
	@Name NVARCHAR(50),
	@Alias NVARCHAR(50) = NULL,
	@Description NVARCHAR(255) = NULL
AS
BEGIN
	DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
	SET NOCOUNT ON;

    INSERT INTO [dbo].[People]
	(
		[Name],
		[Alias],
		[Description]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Name,
		@Alias,
		@Description
	);

	SELECT
		l.[Id],
		[Name],
		[Description],
		[Alias],
		[CreatedAt],
		[UpdatedAt]
	FROM [dbo].[People] l
	JOIN @OutputTable r
	ON l.Id = r.Id;
END
