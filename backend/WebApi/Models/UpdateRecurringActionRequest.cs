using System.ComponentModel.DataAnnotations;
using Core.Models;

namespace WebApi.Models
{
    public class UpdateRecurringActionRequest
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        public bool IsActive { get; set; }

        public DateTime StartAt { get; set; }

        public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.Monthly;

        [Range(1, int.MaxValue, ErrorMessage = "IntervalValue must be at least 1.")]
        public int IntervalValue { get; set; } = 1;

        [Range(1, 31, ErrorMessage = "DayOfMonth must be between 1 and 31.")]
        public int? DayOfMonth { get; set; }
    }
}
