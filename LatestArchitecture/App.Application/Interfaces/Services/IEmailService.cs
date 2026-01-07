using App.Application.Dto;
using App.Application.Dto.AuthenticationModule;
using App.Common.Models;

namespace App.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<JsonModel> SendForgotPasswordEmailAsync(ForgotPasswordResponseModel request);
        Task<JsonModel> SendPasswordResetConfirmationEmailAsync(PasswordResetEmailRequest request);
        Task<JsonModel> SendOtpEmailAsync(EmailRequestDto request);
        string GenerateOtp(int length = 6);
        string GetSenderName();

    }
}
