using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class TaxDto : Dto<Tax>
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public override Tax ToModel()
        {
            return new Tax
            {
                Id = Id.ToString(),
                Name = Name,
                Description = Description,
                Amount = Amount,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
            };
        }
    }
}
