using App.Application.Dto.AuthenticationModule;
using App.Application.Dto.SuperAdminAuthenticationModule.App.Application.Dto.AuthenticationModule;
using App.Common.Models;

namespace App.Application.Interfaces.Services.SuperAdmin
{
    public interface ISuperAdminAuthenticationService
    {
        Task<JsonModel> LoginAsync(SALoginDto loginDto);
    }
}
