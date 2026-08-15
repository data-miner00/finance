-- =============================================
-- Description:	Deletes a notification by Id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DeleteNotification]
	@Id UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM [dbo].[Notifications]
	WHERE [Id] = @Id;
END
