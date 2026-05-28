-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Deletes an account by Id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DeleteAccount]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Accounts]
    WHERE [Id] = @Id;
END
