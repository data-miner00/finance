using Microsoft.Data.SqlClient;
using Provisioning.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Provisioning.Activities
{
    internal class ProvisionDatabaseActivity : IActivity
    {
        private readonly SqlConnection connection;
        private readonly DatabaseOption option;

        public ProvisionDatabaseActivity(SqlConnection connection, DatabaseOption option)
        {
            this.connection = connection;
            this.option = option;
        }

        public async Task ProvisionAsync(CancellationToken cancellationToken)
        {
            string sqlQuery = $@"USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'{option.DatabaseName}')
BEGIN
    CREATE DATABASE [{option.DatabaseName}];
END
GO";

            using var command = new SqlCommand(sqlQuery, this.connection);

            try
            {
                connection.Open();
                await command.ExecuteNonQueryAsync();
                Console.WriteLine("Successfully provisioned database.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Failed to provision database.");
            }
        }
    }
}
