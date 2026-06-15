namespace WebApi.Models
{
    public class UpdateTaxRequest
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }
    }
}
