using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class UpdateCategoryRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }

        [StringLength(20)]
        public string? Color { get; set; }

        [StringLength(50)]
        public string? Icon { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "BudgetAmount must be greater than 0.")]
        public decimal? BudgetAmount { get; set; }
    }
}
