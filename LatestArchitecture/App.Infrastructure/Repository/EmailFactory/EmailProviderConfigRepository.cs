using App.Application.Dto.EmailFactory;
using App.Application.Interfaces.Repositories.EmailFactory;
using App.Domain.Entities.EmailFactory;
using App.Infrastructure.DBContext;
using App.SharedConfigs.DBContext;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace App.Infrastructure.Repository.EmailFactory
{
    public class EmailProviderConfigRepository : IEmailProviderConfigRepository
    {
        private readonly MasterDbContext _context;

        public EmailProviderConfigRepository(MasterDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmailProviderConfigs>> GetAllAsync()
        {
            try
            {
                return await _context.EmailProviderConfigs
                    .Include(x => x.EmailProviderType)
                    .Include(x => x.SmtpConfig)
                    .Include(x => x.AwsSesConfig)
                    .Include(x => x.SendGridConfig)
                    .Include(x => x.AzureConfig)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // TODO: Replace with actual logger (Serilog, NLog, ILogger<T>, etc.)
                Console.Error.WriteLine($"[GetAllAsync] Error: {ex.Message}");

                // Rethrow if you want higher layer handling
                throw;
            }
        }

        public async Task<EmailProviderConfigs?> GetActiveProviderByOrganizationIdAsync(int organizationId)
        {
            try
            {
                return await _context.EmailProviderConfigs
                    .Include(x => x.EmailProviderType)
                    .Include(x => x.SmtpConfig)
                    .Include(x => x.AwsSesConfig)
                    .Include(x => x.SendGridConfig)
                    .Include(x => x.AzureConfig)
                    .FirstOrDefaultAsync(x => x.OrganizationId == organizationId
                                              && x.IsActive); // ✅ Ensure only active provider
            }
            catch (Exception ex)
            {
                // TODO: Replace with actual logger (Serilog, NLog, ILogger<T>, etc.)
                Console.Error.WriteLine($"[GetActiveProviderByOrganizationIdAsync] Error: {ex.Message}");

                // Rethrow if you want higher layer handling
                throw;
            }
        }

        public async Task<EmailProviderConfigs?> GetActiveProviderByOrganizationIdAndTypeAsync(int organizationId, int emailProviderTypeId)
        {
            try
            {
                return await _context.EmailProviderConfigs
                    .Include(x => x.EmailProviderType)
                    .Include(x => x.SmtpConfig)
                    .Include(x => x.AwsSesConfig)
                    .Include(x => x.SendGridConfig)
                    .Include(x => x.AzureConfig)
                    .FirstOrDefaultAsync(x => x.OrganizationId == organizationId
                                              && x.EmailProviderTypeId == emailProviderTypeId
                                              && x.IsActive);
            }
            catch (Exception ex)
            {
                // TODO: Replace with actual logger (Serilog, NLog, ILogger<T>, etc.)
                Console.Error.WriteLine($"[GetActiveProviderByOrganizationIdAndTypeAsync] Error: {ex.Message}");

                // Rethrow if you want higher layer handling
                throw;
            }
        }

        public async Task<List<EmailProviderConfigs>> GetActiveProvidersByOrganizationIdAsync(int organizationId)
        {
            try
            {
                return await _context.EmailProviderConfigs
                    .Include(x => x.EmailProviderType)
                    .Include(x => x.SmtpConfig)
                    .Include(x => x.AwsSesConfig)
                    .Include(x => x.SendGridConfig)
                    .Include(x => x.AzureConfig)
                    .Where(x => x.OrganizationId == organizationId && x.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // TODO: Replace with actual logger (Serilog, NLog, ILogger<T>, etc.)
                Console.Error.WriteLine($"[GetActiveProvidersByOrganizationIdAsync] Error: {ex.Message}");

                // Rethrow if you want higher layer handling
                throw;
            }
        }


        public async Task<EmailProviderConfigs?> GetByIdAsync(int id)
        {
            return await _context.EmailProviderConfigs.FindAsync(id);
        }

        public async Task AddAsync(EmailProviderConfigs config)
        {
            await _context.EmailProviderConfigs.AddAsync(config);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmailProviderConfigs config)
        {
            _context.EmailProviderConfigs.Update(config);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var config = await GetByIdAsync(id);
            if (config != null)
            {
                _context.EmailProviderConfigs.Remove(config);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<EmailConfigurationResult> SetEmailConfigurationAsync(
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
    string? performedBy = null)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@OrganizationId", organizationId);
                parameters.Add("@EmailProviderTypeId", emailProviderTypeId);
                parameters.Add("@IsActive", isActive);
                parameters.Add("@EnableSsl", enableSsl);
                parameters.Add("@TemplatesPath", templatesPath);

                // SMTP Parameters
                parameters.Add("@SmtpServer", smtpServer);
                parameters.Add("@Port", port);
                parameters.Add("@UserName", userName);
                parameters.Add("@Password", password);
                parameters.Add("@FromEmail", fromEmail);
                parameters.Add("@FromName", fromName);

                // AWS SES Parameters
                parameters.Add("@AwsAccessKey", awsAccessKey);
                parameters.Add("@AwsSecretKey", awsSecretKey);
                parameters.Add("@AwsRegion", awsRegion);

                // SendGrid Parameters
                parameters.Add("@ApiKey", apiKey);

                // Azure Parameters
                parameters.Add("@ConnectionString", connectionString);
                parameters.Add("@SenderAddress", senderAddress);

                parameters.Add("@PerformedBy", performedBy);

                // Execute SP and map structured response
                var result = await _context.Database.GetDbConnection().QueryFirstOrDefaultAsync<EmailConfigurationResult>(
                    "SUP_SetEmailConfiguration",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result ?? new EmailConfigurationResult
                {
                    ConfigId = 0,
                    Status = "ERROR",
                    Message = "No response from stored procedure"
                };
            }
            catch (SqlException sqlEx)
            {
                return new EmailConfigurationResult
                {
                    ConfigId = 0,
                    Status = "SQL_ERROR",
                    Message = $"Database error: {sqlEx.Message}"
                };
            }
            catch (Exception ex)
            {
                return new EmailConfigurationResult
                {
                    ConfigId = 0,
                    Status = "EXCEPTION",
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }

        public async Task<EmailConfigurationListDto> GetEmailConfigurationsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? sortColumn = "OrganizationName",
            string? sortDirection = "ASC")
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@PageNumber", pageNumber);
                parameters.Add("@PageSize", pageSize);
                parameters.Add("@Search", search);
                parameters.Add("@SortColumn", sortColumn);
                parameters.Add("@SortDirection", sortDirection);

                // Execute SP and get results
                var results = await _context.Database.GetDbConnection().QueryAsync<EmailConfigurationItemDto>(
                    "SUP_GetEmailConfigurations",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var items = results.ToList();
                var totalCount = items.Any() ? items.First().TotalCount : 0;

                return new EmailConfigurationListDto
                {
                    Items = items,
                    TotalCount = (int)totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (SqlException sqlEx)
            {
                // Log sqlEx here using ILogger or your logging mechanism
                throw new Exception($"Database error: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                // Log ex here using ILogger or your logging mechanism
                throw new Exception($"Unexpected error: {ex.Message}", ex);
            }
        }



    }
}
