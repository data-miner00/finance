-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Updates a piggy bank by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UpdatePiggyBank]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(50),
    @Amount MONEY,
    @Target MONEY,
    @Deadline DATETIME2(7) = NULL,
    @Description NVARCHAR(255) = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    UPDATE [dbo].[PiggyBanks]
    SET
        [Name] = @Name,
        [Amount] = @Amount,
        [Target] = @Target,
        [Deadline] = @Deadline,
        [Description] = @Description
    OUTPUT inserted.Id INTO @OutputTable
    WHERE [Id] = @Id;

    SELECT
        p.[Id],
        p.[Name],
        p.[Description],
        p.[Amount],
        p.[Target],
        p.[Deadline],
        p.[CreatedAt],
        p.[UpdatedAt]
    FROM [dbo].[PiggyBanks] p
    JOIN @OutputTable r ON p.Id = r.Id;
END
