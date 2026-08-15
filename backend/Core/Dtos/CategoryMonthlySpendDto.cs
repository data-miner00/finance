using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class CategoryMonthlySpendDto
    {
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }

        public decimal? BudgetAmount { get; set; }

        public decimal SpentThisMonth { get; set; }

        public CategoryMonthlySpend ToModel()
        {
            return new CategoryMonthlySpend
            {
                CategoryId = CategoryId.ToString(),
                CategoryName = CategoryName,
                BudgetAmount = BudgetAmount,
                SpentThisMonth = SpentThisMonth,
            };
        }
    }
}
