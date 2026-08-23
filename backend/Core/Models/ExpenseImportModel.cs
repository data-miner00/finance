namespace Core.Models
{
    public class ExpenseImportModel
    {
        public string? CategoryName { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public string? Location { get; set; }

        public DateTime ActionedAt { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string? AgentName { get; set; }
    }
}
