using Core.Dtos;
using Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;

namespace Core.Repositories
{
    public sealed class SettingsRepository : ISettingsRepository
    {
        private readonly IDbConnectionFactory connectionFactory;

        public SettingsRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Setting>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT [Id], [Key], [Value], [CreatedAt], [UpdatedAt] FROM [dbo].[Settings];");

            using var connection = this.connectionFactory.CreateConnection();
            var dtos = await connection.QueryAsync<SettingDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<IEnumerable<Setting>> UpsertManyAsync(IDictionary<string, string> values, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = this.connectionFactory.CreateConnection())
            {
                foreach (var kvp in values)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("Key", kvp.Key);
                    parameters.Add("Value", kvp.Value);

                    await connection.ExecuteAsync(
                        SpNames.UpsertSetting,
                        parameters,
                        commandType: CommandType.StoredProcedure);
                }
            }

            return await this.GetAllAsync(cancellationToken);
        }
    }
}
