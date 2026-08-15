using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository repository;

        public NotificationController(INotificationRepository repository)
        {
            this.repository = repository;
        }

        private CancellationToken CancellationToken => this.HttpContext.RequestAborted;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAll()
        {
            var notifications = await this.repository.GetAllAsync(this.CancellationToken);
            return this.Ok(notifications);
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var count = await this.repository.GetUnreadCountAsync(this.CancellationToken);
            return this.Ok(count);
        }

        [HttpPut("{id}/read")]
        public async Task<ActionResult<Notification>> MarkRead(string id)
        {
            Notification notification;
            try
            {
                notification = await this.repository.GetByIdAsync(id, this.CancellationToken);
            }
            catch
            {
                return this.NotFound();
            }

            notification.IsRead = true;
            var updated = await this.repository.UpdateAsync(notification, this.CancellationToken);
            return this.Ok(updated);
        }

        [HttpPut("read-all")]
        public async Task<ActionResult> MarkAllRead()
        {
            await this.repository.MarkAllAsReadAsync(this.CancellationToken);
            return this.NoContent();
        }
    }
}
