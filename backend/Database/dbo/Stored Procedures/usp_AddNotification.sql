-- =============================================
-- Description:	Creates a notification and returns the created row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddNotification]
	@Type NVARCHAR(50),
	@Title NVARCHAR(200),
	@Message NVARCHAR(500),
	@EntityType NVARCHAR(50) = NULL,
	@EntityId NVARCHAR(50) = NULL
AS
BEGIN
	DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Notifications]
	(
		[Type],
		[Title],
		[Message],
		[EntityType],
		[EntityId]
	)
	OUTPUT inserted.Id INTO @OutputTable
	VALUES
	(
		@Type,
		@Title,
		@Message,
		@EntityType,
		@EntityId
	);

	SELECT
		n.[Id],
		n.[Type],
		n.[Title],
		n.[Message],
		n.[IsRead],
		n.[EntityType],
		n.[EntityId],
		n.[CreatedAt],
		n.[UpdatedAt]
	FROM [dbo].[Notifications] n
	JOIN @OutputTable r
	ON n.Id = r.Id;
END
