using App.Application.Dto.AuthenticationModule;
using App.Application.Interfaces.Services.SuperAdmin;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace App.Application.Service.SuperAdmin
{
    public class SuperAdminTokenService : ISuperAdminTokenService
    {
        private readonly IConfiguration _config;

        public SuperAdminTokenService(IConfiguration config)
        {
            _config = config;
        }

        public UserResponseModel GenerateClaims(UserResponseModel userResponseModel)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userResponseModel.UserId.ToString()),
                new Claim(ClaimTypes.Name, userResponseModel.Username),
                new Claim(ClaimTypes.Role, userResponseModel.RoleName),
                new Claim("RoleId", userResponseModel.RoleId.ToString()),
                new Claim("RoleIName", userResponseModel.RoleName.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            userResponseModel.AccessToken = GenerateAccessToken(claims);
            userResponseModel.RefreshToken = GenerateRefreshToken();

            return userResponseModel;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SuperAdminJwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["SuperAdminJwtSettings:Issuer"],
                audience: _config["SuperAdminJwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["SuperAdminJwtSettings:AccessTokenExpirationMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public dynamic GetDataFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadToken(token);
        }
    }
}
