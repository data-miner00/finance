using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class RecurringExpense : Entity
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime ExecutedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
