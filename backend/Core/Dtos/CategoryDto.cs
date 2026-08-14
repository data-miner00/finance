using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class CategoryDto : Dto<Category>
    {
        public string Name { get; set; }

        public string? Color { get; set; }

        public string? Icon { get; set; }

        public override Category ToModel()
        {
            return new Category
            {
                Id = Id.ToString(),
                Name = Name,
                Color = Color,
                Icon = Icon,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
            };
        }
    }
}
