-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Patches a recurring action by Id and returns the updated row.
-- =============================================
CREATE PROCEDURE [dbo].[usp_PatchRecurringAction]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(50) = NULL,
    @Description NVARCHAR(255) = NULL,
    @IsActive BIT = NULL,
    @RecurringAt DATETIME2(7) = NULL,
    @Type NVARCHAR(50) = NULL,
    @Amount MONEY = NULL
AS
BEGIN
    DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER);
    SET NOCOUNT ON;

    UPDATE [dbo].[Recurrings]
    SET
        [Name] = COALESCE(@Name, [Name]),
        [Description] = COALESCE(@Description, [Description]),
        [IsActive] = COALESCE(@IsActive, [IsActive]),
        [RecurringAt] = COALESCE(@RecurringAt, [RecurringAt]),
        [Type] = COALESCE(@Type, [Type]),
        [Amount] = COALESCE(@Amount, [Amount])
    OUTPUT inserted.Id INTO @OutputTable
    WHERE [Id] = @Id;

    SELECT
        r.[Id],
        r.[Name],
        r.[Description],
        r.[IsActive],
        r.[RecurringAt],
        r.[Type],
        r.[Amount],
        r.[CreatedAt],
        r.[UpdatedAt]
    FROM [dbo].[Recurrings] r
    JOIN @OutputTable t ON r.Id = t.Id;
END
