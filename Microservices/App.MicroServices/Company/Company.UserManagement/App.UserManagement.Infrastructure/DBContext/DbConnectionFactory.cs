using App.Common.Constants;
using App.UserManagement.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Npgsql;
using System.Data;

namespace App.UserManagement.Infrastructure.DBContext
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection CreateConnection(string connectionString, string provider)
        {
            if (provider.Contains(Constants.ProviderNameSqlServer))
                return new SqlConnection(connectionString);
            else if (provider.Contains(Constants.ProviderNamePostgreSQL))
                return new NpgsqlConnection(connectionString);
            else
                throw new NotSupportedException($"The provider '{provider}' is not supported.");
        }
    }
}
