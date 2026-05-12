using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class PiggyBank : Entity
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public decimal Target { get; set; }

        public string? Currency { get; set; }

        public DateTime? Deadline { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
