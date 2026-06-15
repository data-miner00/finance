-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Deletes a tax record by Id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DeleteTax]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Taxes]
    WHERE [Id] = @Id;
END
