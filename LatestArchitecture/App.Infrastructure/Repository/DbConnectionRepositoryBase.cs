
using App.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace App.Infrastructure.Repository
{
    public abstract class DbConnectionRepositoryBase
    {
        protected readonly DbContext _dbContext;
        protected readonly string _connectionString;
        protected readonly string _provider;
        protected readonly IDbConnection _dbConnection;

        protected DbConnectionRepositoryBase(DbContext context, IDbConnectionFactory dbConnectionFactory)
        {
            _dbContext = context;
            _connectionString = context.Database.GetDbConnection().ConnectionString;
            _provider = context.Database.ProviderName ?? throw new InvalidOperationException("Database provider name cannot be null.");
            _dbConnection = dbConnectionFactory.CreateConnection(_connectionString, _provider);
        }
    }
}
