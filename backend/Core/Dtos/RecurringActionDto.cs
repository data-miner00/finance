using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class RecurringActionDto : Dto<RecurringAction>
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public string Type { get; set; }

        public decimal Amount { get; set; }

        public DateTime RecurringAt { get; set; }

        public override RecurringAction ToModel()
        {
            return new RecurringAction
            {
                Id = Id.ToString(),
                Name = Name,
                Description = Description,
                IsActive = IsActive,
                Type = Enum.Parse<RecurringType>(Type),
                Amount = Amount,
                RecurringAt = RecurringAt,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
            };
        }
    }
}
