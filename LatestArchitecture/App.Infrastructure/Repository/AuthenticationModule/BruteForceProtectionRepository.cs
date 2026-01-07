using App.Application.Dto.AuthenticationModule;
using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.AuthenticationModule;
using App.Infrastructure.DBContext;
using Dapper;
using System.Data;

namespace App.Infrastructure.Repository.AuthenticationModule
{
    public class BruteForceProtectionRepository : DbConnectionRepositoryBase, IBruteForceProtectionRepository
    {
        public BruteForceProtectionRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory)
        : base(context, dbConnectionFactory)
        {
        }


        public async Task<FailedLoginAttemptResponseModel> LoginAttemptAsync(BruteForceProtectionDto bruteForceProtectionDto)
        {


            const string storedProcedure = "sp_FailedLoginAttempt";

            var parameters = new DynamicParameters();
            parameters.Add("@IpAddress", bruteForceProtectionDto.IpAddress);
            parameters.Add("@UserName", bruteForceProtectionDto.UserName);
            parameters.Add("@ClickEvent", bruteForceProtectionDto.ClickEvent);


            return await _dbConnection.QueryFirstOrDefaultAsync<FailedLoginAttemptResponseModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> ResetAttemptsAsync(BruteForceProtectionDto bruteForceProtectionDto)
        {


            const string storedProcedure = "sp_ResetLoginAttempts";

            var parameters = new DynamicParameters();
            parameters.Add("@IpAddress", bruteForceProtectionDto.IpAddress);
            parameters.Add("@UserName", bruteForceProtectionDto.UserName);


            return await _dbConnection.QueryFirstOrDefaultAsync<bool>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
