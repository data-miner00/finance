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

        public DateTime StartAt { get; set; }

        public string RecurrenceType { get; set; }

        public int IntervalValue { get; set; }

        public int? DayOfMonth { get; set; }

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
                StartAt = StartAt,
                RecurrenceType = Enum.Parse<Core.Models.RecurrenceType>(RecurrenceType),
                IntervalValue = IntervalValue,
                DayOfMonth = DayOfMonth,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
            };
        }
    }
}
