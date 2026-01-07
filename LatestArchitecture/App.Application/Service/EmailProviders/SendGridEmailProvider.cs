using App.Application.Interfaces.Services.Email;
using App.Domain.Entities.EmailFactory;
using SendGrid;
using SendGrid.Helpers.Mail;


namespace App.Application.Service.EmailProviders
{
    public class SendGridEmailProvider : IEmailProvider
    {
        private readonly SendGridConfig _config;

        public SendGridEmailProvider(SendGridConfig config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var client = new SendGridClient(_config.ApiKey);
            var from = new SendGrid.Helpers.Mail.EmailAddress(_config.FromEmail, _config.FromEmail);
            var to = new SendGrid.Helpers.Mail.EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
