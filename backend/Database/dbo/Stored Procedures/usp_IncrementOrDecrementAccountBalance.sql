-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_IncrementOrDecrementAccountBalance] 
	@Id UNIQUEIDENTIFIER,
	@Amount MONEY,
	@Type BIT -- One means increment, zero means decrement
AS
BEGIN
	SET NOCOUNT ON;

	IF @Type = 0
		MERGE INTO [dbo].[Accounts] T
		USING
		(
			SELECT @Id AS Id, @Amount AS Amount
		) S
		ON (T.Id = S.Id AND T.Balance - S.Amount >= 0)
		WHEN MATCHED THEN
			UPDATE SET [Balance] = T.[Balance] - S.[Amount];
	ELSE
		MERGE INTO [dbo].[Accounts] T
		USING
		(
			SELECT @Id AS Id, @Amount AS Amount
		) S
		ON (T.Id = S.Id)
		WHEN MATCHED THEN
			UPDATE SET [Balance] = T.[Balance] + S.[Amount];
END