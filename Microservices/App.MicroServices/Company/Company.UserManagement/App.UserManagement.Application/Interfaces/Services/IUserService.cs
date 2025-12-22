using App.UserManagement.Application.Dto;
using App.UserManagement.Domain.Entities;

namespace App.UserManagement.Application.Interfaces.Services
{
    public interface IUserService
    {
        public Task<UserDto?> GetUserByUserIdAsync(int userId);
        public Task<int> CreateUserAsync(UserDto userDto);
        public Task<List<Users>> GetAllAsync();
    }
}
