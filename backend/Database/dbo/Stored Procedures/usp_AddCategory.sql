-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddCategory]
	@Name NVARCHAR(50),
	@IsSystemDefault BIT = 0
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Categories]
	(
		[Name],
		[IsSystemDefault]
	)
	VALUES
	(
		@Name,
		@IsSystemDefault
	);
END