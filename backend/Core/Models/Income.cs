using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Income : Entity
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
