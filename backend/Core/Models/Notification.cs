using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Notification : Entity
    {
        public string Type { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        public string? EntityType { get; set; }

        public string? EntityId { get; set; }
    }
}
