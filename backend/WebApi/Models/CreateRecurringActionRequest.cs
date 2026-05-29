using Core.Models;

namespace WebApi.Models
{
    public class CreateRecurringActionRequest
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public decimal Amount { get; set; }

        public RecurringType Type { get; set; }

        public DateTime StartAt { get; set; }

        public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.Monthly;

        public int IntervalValue { get; set; } = 1;

        public int? DayOfMonth { get; set; }
    }
}
