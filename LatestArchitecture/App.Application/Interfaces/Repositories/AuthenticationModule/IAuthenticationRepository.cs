using App.Application.Dto.AuthenticationModule;
using App.Domain.Entities;

namespace App.Application.Interfaces.Repositories.AuthenticationModule
{
    public interface IAuthenticationRepository
    {
        Task<UserResponseModel> AuthenticateAsync(LoginDto loginDto);
        Task<ForgotPasswordResponseModel> ForgotPasswordAsync(ResetUserPasswordDto resetUserPasswordDto);

        Task<ResetPasswordResponseModel> ResetPasswordAsync(ResetUserPasswordDto dto);

        Task<UserResponseModel> VerifyOtpAsync(int userId, string otp);
        Task SaveUserOtpAsync(int userId, string otp, string mfaType, int? organizationId, string ipAddress);
        Task UpdateThirdPartyLoginAsync(int userId, int thirdPartyLogin);
        Task<Users?> GetUserByEmailAsync(string email);


        Task<UserResponseModel> GetUserForSSOAsync(string email, int thirdPartyLogin);
        Task<IEnumerable<AgencySettingDto>> GetOrgSettingsAsync(string organizationId);

    }

}
