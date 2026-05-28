-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Deletes an expense by Id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DeleteExpense]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Expenses]
    WHERE [Id] = @Id;
END
