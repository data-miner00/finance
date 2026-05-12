namespace WebApi.Models
{
    public class UpdateRecurringExpenseRequest
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime ExecutedAt { get; set; }
    }
}
