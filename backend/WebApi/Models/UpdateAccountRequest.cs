namespace WebApi.Models
{
    public class UpdateAccountRequest
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public int AccountType { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public DateTime ActionedAt { get; set; }
    }
}
