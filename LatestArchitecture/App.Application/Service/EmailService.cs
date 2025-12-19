using App.Application.Dto;
using App.Application.Dto.AuthenticationModule;
using App.Application.Interfaces.Repositories.AuthenticationModule;
using App.Application.Interfaces.Services;
using App.Common.Constant;
using App.Common.Models;
using App.Common.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace App.Application.Service
{
    public class EmailService(IConfiguration config, IAuthenticationRepository authenticationRepository, IHttpContextAccessor httpContextAccessor) : IEmailService
    {
        private readonly IConfiguration _config = config;

        private readonly IAuthenticationRepository _authenticationRepository = authenticationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;


        public string GenerateOtp(int length = 6)
        {
            var random = new Random();
            return new string(Enumerable.Range(0, length)
                .Select(_ => random.Next(0, 10).ToString()[0])
                .ToArray());
        }

        private string LoadTemplate(string templateName, Dictionary<string, string> replacements)
        {
            var templateFolderPath = _config["EmailConfiguration:Templates"];
            var templatePath = Path.Combine(templateFolderPath, templateName);

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Email template not found: {templatePath}");

            string content = File.ReadAllText(templatePath);

            foreach (var kv in replacements)
            {
                content = content.Replace("{{" + kv.Key + "}}", kv.Value);
            }

            return content;
        }

        private async Task SendEmailInternalAsync(string toEmail, string subject, string templateName, Dictionary<string, string> replacements)
        {
            var body = LoadTemplate(templateName, replacements);

            var smtpHost = _config["EmailConfiguration:SmtpServer"];
            var smtpPort = int.Parse(_config["EmailConfiguration:Port"]);
            var senderEmail = _config["EmailConfiguration:From"];
            var senderPassword = _config["EmailConfiguration:Password"];
            var senderName = _config["EmailConfiguration:UserName"];

            var smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task<JsonModel> SendForgotPasswordEmailAsync(ForgotPasswordResponseModel request)
        {
            try
            {
                string otp = GenerateOtp();
                var resetUrlBase = _config["EmailConfiguration:ResetUrlBase"];

                var uriBuilder = new UriBuilder(resetUrlBase);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["token"] = request.ResetToken;
                uriBuilder.Query = query.ToString();

                string resetUrl = uriBuilder.ToString();


                var replacements = new Dictionary<string, string>
                {
                   { "name", request.UserName },
                   { "organizationName", GetSenderName() },
                   { "action_url", resetUrl}
                };

                await SendEmailInternalAsync(request.Email, "Reset Your Password", "forgot-password.html", replacements);

                return new JsonModel(true, StatusMessage.PasswordResetEmailSent, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel(false, StatusMessage.InternalServerError, (int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<JsonModel> SendPasswordResetConfirmationEmailAsync(PasswordResetEmailRequest request)
        {
            try
            {
                var replacements = new Dictionary<string, string>
                {
                    { "name", request.UserName },
                    { "organizationName", request.OrganizationName }
                };

                await SendEmailInternalAsync(request.Email, "Password Reset Confirmation", "reset-confirmation.html", replacements);

                return new JsonModel(true, "Password reset confirmation email sent.", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel(false, StatusMessage.InternalServerError, (int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        public async Task<JsonModel> SendOtpEmailAsync(EmailRequestDto request)
        {
            try
            {
                string otp = GenerateOtp();
                var clientIp = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);

                // Save OTP in DB
                await _authenticationRepository.SaveUserOtpAsync(
                    request.UserId,
                    otp,
                    "email",                    // MfaType
                    request.OrganizationId,
                    clientIp
                );


                var replacements = new Dictionary<string, string>
                {
                    { "name", request.Name },
                    { "organizationName", GetSenderName() },
                    { "otp", otp }
                };

                await SendEmailInternalAsync(request.Email, "Your OTP Code", "otp-verification.html", replacements);

                return new JsonModel(true, StatusMessage.OTPSentToEmail, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new JsonModel(false, StatusMessage.InternalServerError, (int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public string GetSenderName() => _config["EmailSettings:SenderName"] ?? "SmartPODS";
    }
}
