using App.UserManagement.Domain.Entities;

namespace App.UserManagement.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<Users>
{
    Task<Users?> GetByEmailAsync(string email);

    //dapper
    Task<IEnumerable<Users>> GetAllAsync();
}