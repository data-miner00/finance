using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class PersonDto : Dto<Person>
    {
        public string Name { get; set; }

        public string? Alias { get; set; }

        public string? Description { get; set; }

        public override Person ToModel()
        {
            return new Person
            {
                Id = Id.ToString(),
                Name = Name,
                Alias = Alias,
                Description = Description,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
            };
        }
    }
}
