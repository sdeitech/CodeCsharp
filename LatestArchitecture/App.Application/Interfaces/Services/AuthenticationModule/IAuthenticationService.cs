using App.Application.Dto.AuthenticationModule;
using App.Common.Models;
using App.Domain.Entities;

namespace App.Application.Interfaces.Services.AuthenticationModule
{
    public interface IAuthenticationService
    {
        Task<JsonModel> AuthenticateAsync(LoginDto loginDto);

        Task<JsonModel> ForgotPasswordAsync(ResetUserPasswordDto resetUserPasswordDto);
        Task<JsonModel> ResetPasswordAsync(ResetUserPasswordDto resetUserPasswordDto);

        Task<JsonModel> VerifyOtpAsync(VerifyOtpRequest request);
        Task<JsonModel> VerifyForgotPasswordOtpAsync(VerifyOtpRequest request);

        Task<JsonModel> GetUserByToken(string authToken, TokenModel tokenModel);

        Task<JsonModel> GoogleLoginAsync(Authenctication loginDto);
        //Task<JsonModel> MicrosoftLoginAsync(Authenctication loginDto);
        Task<JsonModel> FacebookLoginAsync(Authenctication loginDto);
        Task<JsonModel> GetOrgSettingsAsync(string organizationId);
    }

}
