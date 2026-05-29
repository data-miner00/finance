-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Patches a piggy bank by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_PatchPiggyBank]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(50) = NULL,
    @Amount MONEY = NULL,
    @Target MONEY = NULL,
    @Deadline DATETIME2(7) = NULL,
    @Description NVARCHAR(255) = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    UPDATE [dbo].[PiggyBanks]
    SET
        [Name] = COALESCE(@Name, [Name]),
        [Amount] = COALESCE(@Amount, [Amount]),
        [Target] = COALESCE(@Target, [Target]),
        [Deadline] = COALESCE(@Deadline, [Deadline]),
        [Description] = COALESCE(@Description, [Description])
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
