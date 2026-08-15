namespace WebApi.Models
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }

        public string? Color { get; set; }

        public string? Icon { get; set; }

        public decimal? BudgetAmount { get; set; }
    }
}
