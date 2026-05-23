-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddAccount]
	@Name NVARCHAR(50),
	@Type NVARCHAR(50),
	@Description NVARCHAR(255) = NULL
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Accounts]
	(
		[Name],
		[Type],
		[Description]
	)
	VALUES
	(
		@Name,
		@Type,
		@Description
	);
END