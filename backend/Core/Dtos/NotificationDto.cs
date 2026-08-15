using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dtos
{
    internal class NotificationDto : Dto<Notification>
    {
        public string Type { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        public string? EntityType { get; set; }

        public string? EntityId { get; set; }

        public override Notification ToModel()
        {
            return new Notification
            {
                Id = Id.ToString(),
                Type = Type,
                Title = Title,
                Message = Message,
                IsRead = IsRead,
                EntityType = EntityType,
                EntityId = EntityId,
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
            };
        }
    }
}
