namespace Core.Models
{
    public class RecurringIncome : Entity
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime RecurringAt { get; set; }
    }
}
