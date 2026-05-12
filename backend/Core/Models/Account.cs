using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Account : Entity
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public AccountType AccountType { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public DateTime ActionedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public enum AccountType
    {
        Bank,
        EWallet,
        Cash,
        App,
        Card,
    }
}
