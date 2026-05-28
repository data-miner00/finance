-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Deletes a piggy bank by Id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DeletePiggyBank]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[PiggyBanks]
    WHERE [Id] = @Id;
END
