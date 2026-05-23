using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Expense : Entity
    {
        public string Category { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public string? Location { get; set; }

        public string? ReceiptImage { get; set; }

        public DateTime ActionedAt { get; set; }
    }
}
