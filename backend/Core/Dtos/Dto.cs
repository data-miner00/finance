using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class Dto
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
