namespace WebApi.Models
{
    public class CreateExpenseRequest
    {
        public string? CategoryName { get; set; }

        public string? AccountId { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public string? Location { get; set; }

        public string? ReceiptImage { get; set; }

        public string? AgentName { get; set; }
    }
}
