using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class ExpenseDto : Dto<Expense>
    {
        public string? CategoryName { get; set; }

        public Guid? AccountId { get; set; }

        public string? AccountName { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string? Location { get; set; }

        public DateTime ActionedAt { get; set; }

        public string? AgentName { get; set; }

        public override Expense ToModel()
        {
            return new Expense
            {
                Id = Id.ToString(),
                Name = Name,
                Description = Description,
                CategoryName = CategoryName,
                AccountId = AccountId?.ToString(),
                AccountName = AccountName,
                Amount = Amount,
                Currency = Currency,
                Location = Location,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
                ActionedAt = ActionedAt,
                AgentName = AgentName,
            };
        }
    }
}
