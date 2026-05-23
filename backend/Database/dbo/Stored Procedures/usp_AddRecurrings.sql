-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddRecurrings]
	@Name NVARCHAR(50),
	@Amount MONEY,
	@Type NVARCHAR(50),
	@RecurringAt DATETIME2(7),
	@Description NVARCHAR(255) = NULL
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO [dbo].[Recurrings]
	(
		[Name],
		[Amount],
		[Type],
		[Description],
		[RecurringAt]
	)
	VALUES
	(
		@Name,
		@Amount,
		@Type,
		@Description,
		@RecurringAt
	);
END