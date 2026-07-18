using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class ProfileDto : Dto<Profile>
    {
        public string Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Bio { get; set; }

        public string? CompanyName { get; set; }

        public string? WebsiteUrl { get; set; }

        public string? AvatarImage { get; set; }

        public override Profile ToModel()
        {
            return new Profile
            {
                Id = Id.ToString(),
                Username = Username,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Bio = Bio,
                CompanyName = CompanyName,
                WebsiteUrl = WebsiteUrl,
                AvatarImage = AvatarImage,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
            };
        }
    }
}
