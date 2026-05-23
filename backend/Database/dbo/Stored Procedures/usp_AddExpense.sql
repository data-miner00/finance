-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddExpense]
	@Name NVARCHAR(50),
	@Amount MONEY,
	@CategoryId UNIQUEIDENTIFIER = NULL,
	@Location NVARCHAR(255) = NULL,
	@Description NVARCHAR(255) = NULL
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Expenses]
	(
		[Name],
		[CategoryId],
		[Amount],
		[Location],
		[Description]
	)
	VALUES
	(
		@Name,
		@CategoryId,
		@Amount,
		@Location,
		@Description
	);
END