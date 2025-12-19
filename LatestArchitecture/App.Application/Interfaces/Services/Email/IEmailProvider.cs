using System.Threading.Tasks;

namespace App.Application.Interfaces.Services.Email
{
    public interface IEmailProvider
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
