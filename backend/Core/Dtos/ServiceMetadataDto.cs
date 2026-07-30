using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class ServiceMetadataDto : Dto<ServiceMetadata>
    {
        public string ServiceName { get; set; }

        public string? Description { get; set; }

        public override ServiceMetadata ToModel()
        {
            return new ServiceMetadata
            {
                Id = Id.ToString(),
                ServiceName = ServiceName,
                Description = Description,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
            };
        }
    }
}
