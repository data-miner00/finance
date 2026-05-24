using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class PiggyBankDto : Dto<PiggyBank>
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public decimal Target { get; set; }

        public DateTime? Deadline { get; set; }

        public override PiggyBank ToModel()
        {
            return new PiggyBank
            {
                Id = Id.ToString(),
                Name = Name,
                Description = Description,
                Amount = Amount,
                Target = Target,
                Deadline = Deadline,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
            };
        }
    }
}
