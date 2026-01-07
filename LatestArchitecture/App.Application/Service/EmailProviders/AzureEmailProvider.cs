using App.Application.Interfaces.Services.Email;
using App.Domain.Entities.EmailFactory;
using Aspose.Email;
using Aspose.Email.Clients.Smtp;
using Aspose.Email.Mime;
using System;
using System.Threading.Tasks;

namespace App.Application.Service.EmailProviders
{
    public class AzureEmailProvider : IEmailProvider
    {
        private readonly AzureConfig _config;

        public AzureEmailProvider(AzureConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            
            if (string.IsNullOrEmpty(_config.ConnectionString))
                throw new ArgumentException("Azure connection string is required");
                
            if (string.IsNullOrEmpty(_config.SenderAddress))
                throw new ArgumentException("Azure sender address is required");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(toEmail))
                throw new ArgumentException("Recipient email address is required");

            if (string.IsNullOrEmpty(subject))
                throw new ArgumentException("Email subject is required");

            if (string.IsNullOrEmpty(body))
                throw new ArgumentException("Email body is required");

            try
            {
                var message = new MailMessage
                {
                    From = _config.SenderAddress,
                    To = toEmail,
                    Subject = subject,
                    HtmlBody = body
                };

                using var client = new SmtpClient(_config.ConnectionString);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to send email via Azure: {ex.Message}", ex);
            }
        }
    }
}
