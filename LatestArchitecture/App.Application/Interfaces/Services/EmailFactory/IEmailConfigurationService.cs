using App.Application.Dto.EmailFactory;
using App.Common.Models;
using App.Domain.Entities.EmailFactory;
using System.Threading.Tasks;

namespace App.Application.Interfaces.Services.EmailFactory
{
    public interface IEmailConfigurationService
    {
        /// <summary>
        /// Sets or updates email configuration for an organization
        /// </summary>
        /// <param name="emailConfig">The email configuration DTO</param>
        /// <param name="performedBy">The user who performed the operation</param>
        /// <returns>A JsonModel containing the operation result</returns>
        Task<JsonModel> SetEmailConfigurationAsync(MasterEmailConfigurationDto emailConfig, string? performedBy = null);

        /// <summary>
        /// Gets email configurations for an organization
        /// </summary>
        /// <param name="organizationId">The organization ID</param>
        /// <returns>A JsonModel containing the list of email configuration response DTOs</returns>
        Task<JsonModel> GetEmailConfigurationAsync(int organizationId);

        /// <summary>
        /// Gets all email configurations
        /// </summary>
        /// <returns>A JsonModel containing the list of email configuration response DTOs</returns>
        Task<JsonModel> GetAllEmailConfigurationsAsync();

        /// <summary>
        /// Gets email configurations with pagination, search, and sorting using stored procedure
        /// </summary>
        /// <param name="pageNumber">Page number for pagination</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="search">Search term for filtering</param>
        /// <param name="sortColumn">Column to sort by</param>
        /// <param name="sortDirection">Sort direction (ASC/DESC)</param>
        /// <returns>A JsonModel containing paginated email configuration results</returns>
        Task<JsonModel> GetEmailConfigurationsWithPaginationAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? sortColumn,
            string? sortDirection);
    }
}
