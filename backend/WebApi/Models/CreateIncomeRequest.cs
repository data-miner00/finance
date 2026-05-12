namespace WebApi.Models
{
    public class CreateIncomeRequest
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }
    }
}
