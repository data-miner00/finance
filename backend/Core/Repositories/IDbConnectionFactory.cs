using System.Data;

namespace Core.Repositories
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
