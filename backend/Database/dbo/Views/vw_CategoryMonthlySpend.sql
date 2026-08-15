CREATE VIEW [dbo].[vw_CategoryMonthlySpend]
AS
SELECT
    c.[Id]           AS [CategoryId],
    c.[Name]         AS [CategoryName],
    c.[BudgetAmount] AS [BudgetAmount],
    ISNULL(SUM(CASE WHEN YEAR(e.[ActionedAt]) = YEAR(GETDATE()) AND MONTH(e.[ActionedAt]) = MONTH(GETDATE())
                     THEN e.[Amount] ELSE 0 END), 0) AS [SpentThisMonth]
FROM [dbo].[Categories] c
LEFT JOIN [dbo].[Expenses] e ON e.[CategoryId] = c.[Id]
WHERE c.[BudgetAmount] IS NOT NULL
GROUP BY c.[Id], c.[Name], c.[BudgetAmount];
