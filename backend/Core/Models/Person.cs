using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Person : Entity
    {
        public string Name { get; set; }

        public string? Alias { get; set; }

        public string? Description { get; set; }
    }
}
