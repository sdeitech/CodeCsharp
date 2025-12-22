using System.Data;

namespace App.UserManagement.Application.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(string connectionString, string provider);
    }
}
