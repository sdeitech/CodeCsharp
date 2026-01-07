using App.Application.Dto.AuthenticationModule;
using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.AuthenticationModule;
using App.Application.Profiles.UserMapper;
using App.Common.Constant;
using App.Domain.Entities;
using App.Infrastructure.DBContext;
using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace App.Infrastructure.Repository.AuthenticationModule
{
    public class AuthenticationRepository : DbConnectionRepositoryBase, IAuthenticationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AuthenticationRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory, IMapper mapper)
        : base(context, dbConnectionFactory)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<IEnumerable<AgencySettingDto>> GetOrgSettingsAsync(string organizationId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OrganizationId", organizationId, DbType.String);

            return await _dbConnection.QueryAsync<AgencySettingDto>(SqlMethod.GetOrgSocialSettings, parameters, commandType: CommandType.StoredProcedure);
        }


        public async Task<UserResponseModel> AuthenticateAsync(LoginDto loginDto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OrganizationId", loginDto.OrgnizationId);
            parameters.Add("@IPAddress", loginDto.OrgnizationId);
            parameters.Add("@UserName", loginDto.UserName);
            parameters.Add("@Password", loginDto.Password);

            return await _dbConnection.QueryFirstOrDefaultAsync<UserResponseModel>(SqlMethod.AuthenticateUser, parameters, commandType: CommandType.StoredProcedure);

        }


        //  Common method for SSO login
        public async Task<UserResponseModel> GetUserForSSOAsync(string email, int thirdPartyLogin)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email, DbType.String);
            parameters.Add("@ThirdPartyLogin", thirdPartyLogin, DbType.Int32);

            return await _dbConnection.QueryFirstOrDefaultAsync<UserResponseModel>(SqlMethod.GetUserFromEmail, parameters, commandType: CommandType.StoredProcedure);
        }



        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            var entity = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive && !u.IsDeleted);


            return entity != null ? _mapper.Map<Users>(entity) : null;
        }

        public async Task UpdateThirdPartyLoginAsync(int Id, int thirdPartyLogin)
        {
            var entity = await _context.Users.FindAsync(Id);
            if (entity != null)
            {
                entity.ThirdPartyLogin = thirdPartyLogin;
                await _context.SaveChangesAsync();
            }
        }




        public async Task<ForgotPasswordResponseModel> ForgotPasswordAsync(ResetUserPasswordDto resetUserPasswordDto)
        {
            ForgotPasswordResponseModel forgotPasswordResponseModel = null;

            var parameters = new DynamicParameters();
            parameters.Add("@Email", resetUserPasswordDto.Email);

            parameters.Add("@IpAddress", resetUserPasswordDto.IpAddress);
            parameters.Add("@Source", resetUserPasswordDto.Email);
            return await _dbConnection.QueryFirstOrDefaultAsync<ForgotPasswordResponseModel>(SqlMethod.ForgotPassword1, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<ResetPasswordResponseModel> ResetPasswordAsync(ResetUserPasswordDto dto)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ResetToken", dto.Token);
            parameters.Add("@NewPasswordHash", dto.Password);
            parameters.Add("@NewSalt", dto.SaltName);
            parameters.Add("@IpAddress", dto.IpAddress);
            parameters.Add("@Source", dto.Source ?? "Web");
            parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@Message", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("@Email", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
            parameters.Add("@UserName", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            await _dbConnection.ExecuteAsync(SqlMethod.ResetUserPassword1, parameters, commandType: CommandType.StoredProcedure);

            return new ResetPasswordResponseModel
            {
                Success = parameters.Get<bool>("@Success"),
                Message = parameters.Get<string>("@Message"),
                Email = parameters.Get<string>("@Email"),
                UserName = parameters.Get<string>("@UserName")
            };
        }

        //public async Task<bool> ResetPasswordAsync(ResetUserPasswordDto dto)
        //{

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@ResetToken", dto.Token);
        //    parameters.Add("@NewPasswordHash", dto.Password);
        //    parameters.Add("@NewSalt", dto.SaltName);
        //    parameters.Add("@IpAddress", dto.IpAddress);
        //    parameters.Add("@Source", dto.Source ?? "Web");
        //    parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);
        //    parameters.Add("@Message", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

        //    await _dbConnection.ExecuteAsync(SqlMethod.ResetUserPassword, parameters, commandType: CommandType.StoredProcedure);

        //    bool isSuccess = parameters.Get<bool>("@Success");
        //    string message = parameters.Get<string>("@Message");


        //    return isSuccess;
        //}

        public async Task<UserResponseModel> VerifyOtpAsync(int userId, string otp)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserID", userId);
            parameters.Add("@Otp", otp);
            return await _dbConnection.QueryFirstOrDefaultAsync<UserResponseModel>(SqlMethod.VerifyUserMfaOtp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task SaveUserOtpAsync(int userId, string otp, string mfaType, int? organizationId, string ipAddress)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserID", userId);
            parameters.Add("@Otp", otp);
            parameters.Add("@MfaType", mfaType);
            parameters.Add("@OrganizationID", organizationId);
            parameters.Add("@IpAddress", ipAddress);
            await _dbConnection.ExecuteAsync(SqlMethod.SaveUserOtp, parameters, commandType: CommandType.StoredProcedure);
        }

    }
}
