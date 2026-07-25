using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Setting : Entity
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
