-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Updates a tax record by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_UpdateTax]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(50),
    @Amount MONEY,
    @Description NVARCHAR(255) = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    UPDATE [dbo].[Taxes]
    SET
        [Name] = @Name,
        [Amount] = @Amount,
        [Description] = @Description
    OUTPUT inserted.Id INTO @OutputTable
    WHERE [Id] = @Id;

    SELECT
        i.[Id],
        i.[Name],
        i.[Description],
        i.[Amount],
        i.[CreatedAt],
        i.[UpdatedAt]
    FROM [dbo].[Taxes] i
    JOIN @OutputTable r ON i.Id = r.Id;
END
