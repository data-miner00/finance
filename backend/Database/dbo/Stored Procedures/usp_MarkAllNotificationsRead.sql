-- =============================================
-- Description:	Marks every unread notification as read.
-- =============================================
CREATE PROCEDURE [dbo].[usp_MarkAllNotificationsRead]
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[Notifications]
	SET [IsRead] = 1
	WHERE [IsRead] = 0;
END
