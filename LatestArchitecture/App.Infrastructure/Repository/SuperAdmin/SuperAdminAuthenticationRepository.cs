using App.Application.Dto.AuthenticationModule;
using App.Application.Dto.SuperAdminAuthenticationModule.App.Application.Dto.AuthenticationModule;
using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.SuperAdmin;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Common.Constant;
using App.Common.Utility;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Infrastructure.DBContext;
using App.SharedConfigs.DBContext;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace App.Infrastructure.Repository.SuperAdmin
{
    public class SuperAdminAuthenticationRepository : ISuperAdminAuthenticationRepository
    {
        private readonly MasterDbContext _context;
        private readonly IBruteForceProtectionService _bruteForceProtectionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbConnection _masterdbConnection;

        public SuperAdminAuthenticationRepository(
            MasterDbContext context,
            IBruteForceProtectionService bruteForceProtectionService,
            IHttpContextAccessor httpContextAccessor,
            IDbConnectionFactory dbConnectionFactory)
        {
            _context = context;
            _bruteForceProtectionService = bruteForceProtectionService;
            _httpContextAccessor = httpContextAccessor;

            var masterConnString = _context.Database.GetDbConnection().ConnectionString;
            _masterdbConnection = dbConnectionFactory.CreateConnection(masterConnString, _context.Database.ProviderName);
        }

        public async Task<UserResponseModel> AuthenticateSuperAdminAsync(SALoginDto loginDto)
        {
            var clientIp = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);

            // Get user with super admin role
            var parameters = new DynamicParameters();
            parameters.Add("@IPAddress", loginDto.OrgnizationId);
            parameters.Add("@UserName", loginDto.UserName);
            parameters.Add("@Password", loginDto.Password);

            var user = await _masterdbConnection.QueryFirstOrDefaultAsync<SAUserResponseModel>(
                SqlMethod.AuthenticateSAUser,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (user == null)
            {
                return null; // User not found or not super admin
            }

            var decryptedPassword = CommonMethods.DecryptUserPassword(loginDto.Password);

            // Check brute force protection
            if (user.BlockUntil.HasValue &&
                CommonMethods.TruncateToMinute(DateTime.UtcNow) < CommonMethods.TruncateToMinute(user.BlockUntil.Value))
            {
                return null; // User is blocked
            }

            // Verify password
            if (!CommonMethods.VerifyPassword(decryptedPassword, user.Password))
            {
                
                return null;
            }

                        // Return user response model
            return new UserResponseModel
            {
                UserId = user.UserId,
                Email = user.Email,
                Username = user.Username,
                RoleId = user.RoleId,
                RoleName = "SuperAdmin",
                Password = user.Password,
            };
        }
    }
}
