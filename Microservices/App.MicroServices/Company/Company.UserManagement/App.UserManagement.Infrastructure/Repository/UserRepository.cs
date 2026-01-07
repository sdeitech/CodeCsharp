using App.UserManagement.Application.Interfaces;
using App.UserManagement.Application.Interfaces.Repositories;
using App.UserManagement.Domain.Entities;
using App.UserManagement.Infrastructure.DBContext;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace App.UserManagement.Infrastructure.Repository;

public class UserRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory) : BaseRepository<Users>(context, dbConnectionFactory), IUserRepository
{
    public async Task<IEnumerable<Users>> GetAllAsync()
    {
        using var connection = _dbConnection;
        string sql = "SELECT * FROM Users";
        return await connection.QueryAsync<Users>(sql);
    }

    public async Task<Users?> GetByEmailAsync(string email)
    {
        return await DbSet.SingleOrDefaultAsync(user => user.Email == email);
    }
}