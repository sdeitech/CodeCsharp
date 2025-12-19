using App.Application.Dto.EmailFactory;
using App.Domain.Entities.EmailFactory;
using System.Threading.Tasks;

namespace App.Application.Interfaces.Repositories.EmailFactory
{
    public interface IEmailProviderConfigRepository
    {
        Task<List<EmailProviderConfigs>> GetAllAsync();
        Task<EmailProviderConfigs?> GetActiveProviderByOrganizationIdAsync(int organizationId);
        Task<EmailProviderConfigs?> GetActiveProviderByOrganizationIdAndTypeAsync(int organizationId, int emailProviderTypeId);
        Task<List<EmailProviderConfigs>> GetActiveProvidersByOrganizationIdAsync(int organizationId);
        Task<EmailProviderConfigs?> GetByIdAsync(int id);
        Task AddAsync(EmailProviderConfigs config);
        Task UpdateAsync(EmailProviderConfigs config);
        Task DeleteAsync(int id);
        Task<EmailConfigurationResult> SetEmailConfigurationAsync(
            int organizationId,
            int emailProviderTypeId,
            bool isActive = true,
            bool enableSsl = true,
            string? templatesPath = null,
            // SMTP Parameters
            string? smtpServer = null,
            int? port = null,
            string? userName = null,
            string? password = null,
            string? fromEmail = null,
            string? fromName = null,
            // AWS SES Parameters
            string? awsAccessKey = null,
            string? awsSecretKey = null,
            string? awsRegion = null,
            // SendGrid Parameters
            string? apiKey = null,
            // Azure Parameters
            string? connectionString = null,
            string? senderAddress = null,
            string? performedBy = null);
        Task<EmailConfigurationListDto> GetEmailConfigurationsAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? sortColumn,
            string? sortDirection);
    }

  
}
