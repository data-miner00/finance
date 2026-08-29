using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Account : Entity
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public AccountType Type { get; set; }

        public decimal Balance { get; set; }

        public string Currency { get; set; }

        /// <summary>
        /// Optional target for total spend on this account within the current calendar year
        /// (e.g. spend needed on a credit card to waive its annual fee). Progress against it
        /// is computed client-side from this account's expenses, same as <see cref="Category.BudgetAmount"/>.
        /// </summary>
        public decimal? AnnualSpendTarget { get; set; }
    }
}
