using App.UserManagement.Application.Dto;
using App.UserManagement.Application.Interfaces;
using App.UserManagement.Application.Interfaces.Repositories;
using App.UserManagement.Application.Interfaces.Services;
using App.UserManagement.Domain.Entities;

namespace App.UserManagement.Application.Implementation.Service
{
    public class UserService(IUserRepository userRepository,
                    IUnitOfWork unitOfWork) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<int> CreateUserAsync(UserDto userDto)
        {
            var user = new Users(
                userDto.Email,
                userDto.FirstName,
                userDto.LastName,
                userDto.Password,
                (int)userDto.Role,
                (int)userDto.Status
            );

            _userRepository.Add(user);
            await _unitOfWork.CommitAsync();
            return user.Id;
        }

        public async Task<List<Users>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return [.. users];
        }

        public Task<UserDto?> GetUserByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
