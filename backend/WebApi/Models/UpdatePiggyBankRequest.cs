namespace WebApi.Models
{
    public class UpdatePiggyBankRequest
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public decimal Target { get; set; }

        public string? Currency { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
