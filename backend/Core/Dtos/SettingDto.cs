using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class SettingDto : Dto<Setting>
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public override Setting ToModel()
        {
            return new Setting
            {
                Id = Id.ToString(),
                Key = Key,
                Value = Value,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
            };
        }
    }
}
