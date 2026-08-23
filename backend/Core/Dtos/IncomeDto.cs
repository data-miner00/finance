using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class IncomeDto : Dto<Income>
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public DateTime ActionedAt { get; set; }

        public Guid? AccountId { get; set; }

        public string? AccountName { get; set; }

        public override Income ToModel()
        {
            return new Income
            {
                Id = Id.ToString(),
                Name = Name,
                Description = Description,
                Amount = Amount,
                Currency = Currency,
                ActionedAt = ActionedAt,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
                AccountId = AccountId?.ToString(),
                AccountName = AccountName,
            };
        }
    }
}
