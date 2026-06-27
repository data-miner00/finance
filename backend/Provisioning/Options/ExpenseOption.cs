using System;
using System.Collections.Generic;
using System.Text;

namespace Provisioning.Options
{
    internal class ExpenseOption
    {
        public const string SectionName = "Expense";

        public List<string> DefaultCategories { get; set; } = [];
    }
}
