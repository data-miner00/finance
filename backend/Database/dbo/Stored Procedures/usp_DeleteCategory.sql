-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Deletes a category by Id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DeleteCategory]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Categories]
    WHERE [Id] = @Id;
END
