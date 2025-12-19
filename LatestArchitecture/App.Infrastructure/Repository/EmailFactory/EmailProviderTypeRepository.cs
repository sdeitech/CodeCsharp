using App.Application.Interfaces.Repositories.EmailFactory;
using App.Domain.Entities.EmailFactory;
using App.Infrastructure.DBContext;
using App.SharedConfigs.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Infrastructure.Repository.EmailFactory
{
    public class EmailProviderTypeRepository : IEmailProviderTypeRepository
    {

        private readonly MasterDbContext _context;
        private readonly ILogger<EmailProviderTypeRepository> _logger;

        public EmailProviderTypeRepository(MasterDbContext context, ILogger<EmailProviderTypeRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<EmailProviderType>> GetAllAsync()
        {
            try
            {
                // Validate database context
                if (_context == null)
                {
                    throw new InvalidOperationException("Database context is not available");
                }

                // Check if EmailProviderTypes DbSet exists
                if (_context.EmailProviderTypes == null)
                {
                    throw new InvalidOperationException("EmailProviderTypes DbSet is not configured");
                }

                var result = await _context.EmailProviderTypes
                    .Where(x => x.IsActive)
                    .ToListAsync();

                return result ?? new List<EmailProviderType>();
            }
            catch (Exception ex)
            {
                // Log the exception (assuming logger is available, or we can add one)
                throw new Exception($"Error retrieving email provider types from database: {ex.Message}", ex);
            }
        }

        public async Task<EmailProviderType?> GetByIdAsync(int id)
        {
            return await _context.EmailProviderTypes.FindAsync(id);
        }

        public async Task<EmailProviderType?> GetByNameAsync(string name)
        {
            return await _context.EmailProviderTypes
                .FirstOrDefaultAsync(x => x.Name.ToUpper() == name.ToUpper() && x.IsActive);
        }

        public async Task AddAsync(EmailProviderType providerType)
        {
            await _context.EmailProviderTypes.AddAsync(providerType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmailProviderType providerType)
        {
            _context.EmailProviderTypes.Update(providerType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var providerType = await GetByIdAsync(id);
            if (providerType != null)
            {
                _context.EmailProviderTypes.Remove(providerType);
                await _context.SaveChangesAsync();
            }
        }
    }
}
