using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class SaveProfileRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(100)]
        public string? CompanyName { get; set; }

        [StringLength(255)]
        [Url]
        public string? WebsiteUrl { get; set; }

        [StringLength(500)]
        [Url]
        public string? AvatarImage { get; set; }
    }
}
