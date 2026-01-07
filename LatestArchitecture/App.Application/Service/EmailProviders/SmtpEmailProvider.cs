using App.Application.Interfaces.Services.Email;
using App.Domain.Entities.EmailFactory;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace App.Application.Service.EmailProviders
{
    public class SmtpEmailProvider : IEmailProvider
    {
        private readonly SmtpConfig _config;

        public SmtpEmailProvider(SmtpConfig config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_config.SmtpServer)
            {
                Port = _config.Port ?? 587,
                Credentials = new NetworkCredential(_config.UserName, _config.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config.FromEmail ?? _config.UserName, _config.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
