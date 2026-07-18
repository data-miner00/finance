using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Profile : Entity
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
