using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class AccountDto : Dto
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public string Type { get; set; }

        public decimal Balance { get; set; }

        public Account ToModel()
        {
            return new Account
            {
                Id = Id.ToString(),
                Name = Name,
                Description = Description,
                Type = Enum.Parse<AccountType>(Type),
                Balance = Balance,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
            };
        }
    }
}
