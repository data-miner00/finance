namespace WebApi.Models
{
    public class UpdateRecurringActionRequest
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public bool IsActive { get; set; }

        public DateTime RecurringAt { get; set; }
    }
}
