namespace WebApi.Models
{
    public class UpdateCategoryRequest
    {
        public string Name { get; set; }

        public string? Color { get; set; }

        public string? Icon { get; set; }
    }
}
