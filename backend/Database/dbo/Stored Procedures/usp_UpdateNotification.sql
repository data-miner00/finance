-- =============================================
-- Description:	Sets the read state of a notification and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UpdateNotification]
	@Id UNIQUEIDENTIFIER,
	@IsRead BIT
AS
BEGIN
	DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
	SET NOCOUNT ON;

	UPDATE [dbo].[Notifications]
	SET [IsRead] = @IsRead
	OUTPUT inserted.Id INTO @OutputTable
	WHERE [Id] = @Id;

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
