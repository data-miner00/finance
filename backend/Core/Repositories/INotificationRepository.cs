using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<int> GetUnreadCountAsync(CancellationToken cancellationToken);

        Task MarkAllAsReadAsync(CancellationToken cancellationToken);

        Task<bool> ExistsForEntityThisMonthAsync(string entityType, string entityId, string type, CancellationToken cancellationToken);
    }
}
