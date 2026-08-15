using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class CategoryMonthlySpend
    {
        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        public decimal? BudgetAmount { get; set; }

        public decimal SpentThisMonth { get; set; }
    }
}
