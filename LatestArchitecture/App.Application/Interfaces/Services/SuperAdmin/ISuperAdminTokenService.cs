using App.Application.Dto.AuthenticationModule;
using System.Security.Claims;

namespace App.Application.Interfaces.Services.SuperAdmin
{
    public interface ISuperAdminTokenService
    {
        UserResponseModel GenerateClaims(UserResponseModel userResponseModel);
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        dynamic GetDataFromToken(string token);
    }
}
