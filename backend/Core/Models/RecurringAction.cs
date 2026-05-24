using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class RecurringAction : Entity
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public RecurringType Type { get; set; }

        public decimal Amount { get; set; }

        public DateTime RecurringAt { get; set; }
    }
}
