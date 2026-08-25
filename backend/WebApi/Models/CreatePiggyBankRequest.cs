using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class CreatePiggyBankRequest
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Amount cannot be negative.")]
        public decimal Amount { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Target must be greater than 0.")]
        public decimal Target { get; set; }

        public string? Currency { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
