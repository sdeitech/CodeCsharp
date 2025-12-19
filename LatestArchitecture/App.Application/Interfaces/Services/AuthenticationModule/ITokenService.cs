using App.Application.Dto.AuthenticationModule;
using System.Security.Claims;

namespace App.Application.Interfaces.Services.AuthenticationModule
{
    public interface ITokenService
    {
        UserResponseModel GenerateClaims(UserResponseModel userResponseModel);
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        dynamic GetDataFromToken(string token);

    }
}
