using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class ServiceMetadata : Entity
    {
        public string ServiceName { get; set; }

        public string? Description { get; set; }
    }
}
