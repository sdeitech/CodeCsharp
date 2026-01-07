using App.Domain.Entities.EmailFactory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Application.Interfaces.Repositories.EmailFactory
{
    public interface IEmailProviderTypeRepository
    {
        Task<IEnumerable<EmailProviderType>> GetAllAsync();
        Task<EmailProviderType?> GetByIdAsync(int id);
        Task<EmailProviderType?> GetByNameAsync(string name);
        Task AddAsync(EmailProviderType providerType);
        Task UpdateAsync(EmailProviderType providerType);
        Task DeleteAsync(int id);
    }
}
