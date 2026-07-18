namespace WebApi.Models
{
    public class SaveProfileRequest
    {
        public string Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Bio { get; set; }

        public string? CompanyName { get; set; }

        public string? WebsiteUrl { get; set; }

        public string? AvatarImage { get; set; }
    }
}
