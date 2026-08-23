namespace WebApi.Models
{
    public class UpdateIncomeRequest
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string? AccountId { get; set; }
    }
}
