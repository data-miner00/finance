using Core.Models;

namespace WebApi.Models
{
    public class CreateRecurringActionRequest
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public RecurringType Type { get; set; }

        public DateTime ExecutedAt { get; set; }
    }
}
