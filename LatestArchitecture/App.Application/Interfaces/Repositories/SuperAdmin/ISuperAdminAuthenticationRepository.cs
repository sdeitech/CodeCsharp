using App.Application.Dto.AuthenticationModule;
using App.Application.Dto.SuperAdminAuthenticationModule.App.Application.Dto.AuthenticationModule;

namespace App.Application.Interfaces.Repositories.SuperAdmin
{
    public interface ISuperAdminAuthenticationRepository
    {
        Task<UserResponseModel> AuthenticateSuperAdminAsync(SALoginDto loginDto);
    }
}
