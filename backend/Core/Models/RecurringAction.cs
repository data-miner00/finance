using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class RecurringAction : Entity
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public RecurringType Type { get; set; }

        public decimal Amount { get; set; }

        public DateTime RecurringAt { get; set; }

        public DateTime StartAt { get; set; }

        public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.Monthly;

        public int IntervalValue { get; set; } = 1;

        public int? DayOfMonth { get; set; }
    }
}
