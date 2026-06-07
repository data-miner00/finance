-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Deletes a person entry by Id.
-- =============================================
CREATE PROCEDURE [dbo].[usp_DeletePerson]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[People]
    WHERE [Id] = @Id;
END
