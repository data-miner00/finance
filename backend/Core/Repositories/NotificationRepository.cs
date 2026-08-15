using Core.Models;
using Core.Dtos;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Core.Repositories
{
    public sealed class NotificationRepository : INotificationRepository
    {
        private readonly IDbConnectionFactory connectionFactory;

        public NotificationRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<Notification> CreateAsync(Notification entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Type", entity.Type);
            parameters.Add("Title", entity.Title);
            parameters.Add("Message", entity.Message);
            parameters.Add("EntityType", entity.EntityType);
            parameters.Add("EntityId", entity.EntityId);

            using var connection = this.connectionFactory.CreateConnection();
            var created = await connection.QuerySingleOrDefaultAsync<NotificationDto>(
                SpNames.AddNotification,
                parameters,
                commandType: CommandType.StoredProcedure);

            return created.ToModel();
        }

        public async Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(id), DbType.Guid);

            using var connection = this.connectionFactory.CreateConnection();
            await connection.ExecuteAsync(SpNames.DeleteNotification, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Notification>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT [Id], [Type], [Title], [Message], [IsRead], [EntityType], [EntityId], [CreatedAt], [UpdatedAt] " +
                "FROM Notifications ORDER BY CreatedAt DESC;");

            using var connection = this.connectionFactory.CreateConnection();
            var dtos = await connection.QueryAsync<NotificationDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<Notification> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = "SELECT [Id], [Type], [Title], [Message], [IsRead], [EntityType], [EntityId], [CreatedAt], [UpdatedAt] " +
                "FROM [dbo].[Notifications] WHERE [Id] = @Id;";

            using var connection = this.connectionFactory.CreateConnection();
            var dto = await connection.QueryFirstAsync<NotificationDto>(query, new { Id = Guid.Parse(id) });

            return dto.ToModel();
        }

        public async Task<Notification> UpdateAsync(Notification entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parameters = new DynamicParameters();
            parameters.Add("Id", Guid.Parse(entity.Id), DbType.Guid);
            parameters.Add("IsRead", entity.IsRead);

            using var connection = this.connectionFactory.CreateConnection();
            var updated = await connection.QuerySingleOrDefaultAsync<NotificationDto>(
                SpNames.UpdateNotification,
                parameters,
                commandType: CommandType.StoredProcedure);

            return updated.ToModel();
        }

        public async Task<int> GetUnreadCountAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = "SELECT COUNT(1) FROM [dbo].[Notifications] WHERE [IsRead] = 0;";

            using var connection = this.connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query);
        }

        public async Task MarkAllAsReadAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var connection = this.connectionFactory.CreateConnection();
            await connection.ExecuteAsync(SpNames.MarkAllNotificationsRead, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> ExistsForEntityThisMonthAsync(string entityType, string entityId, string type, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var query = @"SELECT COUNT(1) FROM [dbo].[Notifications]
                WHERE [EntityType] = @EntityType
                AND [EntityId] = @EntityId
                AND [Type] = @Type
                AND YEAR([CreatedAt]) = YEAR(GETDATE())
                AND MONTH([CreatedAt]) = MONTH(GETDATE());";

            using var connection = this.connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(query, new { EntityType = entityType, EntityId = entityId, Type = type });
            return count > 0;
        }
    }
}
