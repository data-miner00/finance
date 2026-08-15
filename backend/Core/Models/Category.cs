using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Category : Entity
    {
        public string Name { get; set; }

        public string? Color { get; set; }

        public string? Icon { get; set; }

        public decimal? BudgetAmount { get; set; }
    }
}
