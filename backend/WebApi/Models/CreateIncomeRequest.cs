using System.ComponentModel.DataAnnotations;
using WebApi.Validation;

namespace WebApi.Models
{
    public class CreateIncomeRequest
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter uppercase code (e.g. MYR).")]
        public string Currency { get; set; }

        [GuidString]
        public string? AccountId { get; set; }
    }
}
