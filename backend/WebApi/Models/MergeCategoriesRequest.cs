using System.ComponentModel.DataAnnotations;
using WebApi.Validation;

namespace WebApi.Models
{
    public class MergeCategoriesRequest : IValidatableObject
    {
        [Required]
        [GuidString]
        public string SourceCategoryId { get; set; }

        [Required]
        [GuidString]
        public string TargetCategoryId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(this.SourceCategoryId) &&
                string.Equals(this.SourceCategoryId, this.TargetCategoryId, StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult(
                    "SourceCategoryId and TargetCategoryId must be different.",
                    [nameof(this.SourceCategoryId), nameof(this.TargetCategoryId)]);
            }
        }
    }
}
