-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Deletes an income entry by Id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DeleteIncome]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Incomes]
    WHERE [Id] = @Id;
END
