-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Deletes a recurring action by Id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DeleteRecurringAction]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Recurrings]
    WHERE [Id] = @Id;
END
