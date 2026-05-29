-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Patches an account by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_PatchAccount]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(50) = NULL,
    @Type NVARCHAR(50) = NULL,
    @Balance MONEY = NULL,
    @Description NVARCHAR(255) = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    UPDATE [dbo].[Accounts]
    SET
        [Name] = COALESCE(@Name, [Name]),
        [Type] = COALESCE(@Type, [Type]),
        [Balance] = COALESCE(@Balance, [Balance]),
        [Description] = COALESCE(@Description, [Description])
    OUTPUT inserted.Id INTO @OutputTable
    WHERE [Id] = @Id;

    SELECT
        a.[Id],
        a.[Name],
        a.[Description],
        a.[Type],
        a.[Balance],
        a.[CreatedAt],
        a.[UpdatedAt]
    FROM [dbo].[Accounts] a
    JOIN @OutputTable r ON a.Id = r.Id;
END
