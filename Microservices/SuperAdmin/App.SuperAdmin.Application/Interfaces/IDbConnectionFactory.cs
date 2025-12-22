using System.Data;

namespace App.SuperAdmin.Infrastructure.DBContext
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(string connectionString, string provider);
    }
}
