-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddIncome]
	@Name NVARCHAR(50),
	@Amount MONEY,
	@Description NVARCHAR(255) = NULL
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Incomes]
	(
		[Name],
		[Amount],
		[Description]
	)
	VALUES
	(
		@Name,
		@Amount,
		@Description
	);
END