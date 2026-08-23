CREATE VIEW dbo.vw_GetAllIncomes
AS
SELECT        dbo.Incomes.Id, dbo.Incomes.Name, dbo.Incomes.Description, dbo.Incomes.Amount, dbo.Incomes.Currency, dbo.Incomes.ActionedAt, dbo.Incomes.CreatedAt, dbo.Incomes.UpdatedAt,
                         dbo.Incomes.AccountId, dbo.Accounts.Name AS AccountName
FROM            dbo.Incomes LEFT OUTER JOIN
                         dbo.Accounts ON dbo.Incomes.AccountId = dbo.Accounts.Id
