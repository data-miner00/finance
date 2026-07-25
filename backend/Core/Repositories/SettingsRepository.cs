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
        private readonly IDbConnection connection;

        public SettingsRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<IEnumerable<Setting>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var command = new CommandDefinition(
                "SELECT * FROM [dbo].[Settings];");

            var dtos = await this.connection.QueryAsync<SettingDto>(command);
            return dtos.Select(x => x.ToModel());
        }

        public async Task<IEnumerable<Setting>> UpsertManyAsync(IDictionary<string, string> values, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var kvp in values)
            {
                var parameters = new DynamicParameters();
                parameters.Add("Key", kvp.Key);
                parameters.Add("Value", kvp.Value);

                await this.connection.ExecuteAsync(
                    SpNames.UpsertSetting,
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }

            return await this.GetAllAsync(cancellationToken);
        }
    }
}
