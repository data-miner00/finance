using System.ComponentModel.DataAnnotations;

namespace WebApi.Validation
{
    public class GuidStringAttribute : ValidationAttribute
    {
        public GuidStringAttribute()
        {
            ErrorMessage = "{0} must be a valid GUID.";
        }

        public override bool IsValid(object? value)
        {
            if (value is null)
            {
                return true;
            }

            return value is string s && Guid.TryParse(s, out _);
        }
    }
}
