using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class ExpenseDto : Dto<Expense>
    {
        public string? CategoryName { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public string? Location { get; set; }

        public DateTime? ActionedAt { get; set; }

        public override Expense ToModel()
        {
            return new Expense
            {
                Id = Id.ToString(),
                Name = Name,
                Description = Description,
                CategoryName = CategoryName,
                Amount = Amount,
                Location = Location,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
                ActionedAt = ActionedAt,
            };
        }
    }
}
