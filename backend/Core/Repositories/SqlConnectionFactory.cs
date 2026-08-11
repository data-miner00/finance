using Microsoft.Data.SqlClient;
using System.Data;

namespace Core.Repositories
{
    public sealed class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection CreateConnection() => new SqlConnection(this.connectionString);
    }
}
