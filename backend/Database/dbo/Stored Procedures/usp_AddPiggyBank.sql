-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddPiggyBank]
	@Name NVARCHAR(50),
	@Amount MONEY,
	@Target MONEY,
	@Description NVARCHAR(255) = NULL,
	@Deadline DATETIME2(7) = NULL
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO [dbo].[PiggyBanks]
	(
		[Name],
		[Amount],
		[Target],
		[Description],
		[Deadline]
	)
	VALUES
	(
		@Name,
		@Amount,
		@Target,
		@Description,
		@Deadline
	);
END