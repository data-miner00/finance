-- =============================================
-- Author:      <Author,,Name>
-- Create date: <Create Date,,>
-- Description: Reassigns all expenses from the source category to the target category, then deletes the source category.
-- =============================================
CREATE PROCEDURE [dbo].[usp_MergeCategories]
    @SourceCategoryId UNIQUEIDENTIFIER,
    @TargetCategoryId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Expenses]
    SET [CategoryId] = @TargetCategoryId
    WHERE [CategoryId] = @SourceCategoryId;

    DELETE FROM [dbo].[Categories]
    WHERE [Id] = @SourceCategoryId;
END
