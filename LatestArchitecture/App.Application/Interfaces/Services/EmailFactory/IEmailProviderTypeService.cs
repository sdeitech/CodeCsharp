using App.Application.Dto.EmailFactory;
using App.Common.Models;

namespace App.Application.Interfaces.Services.EmailFactory
{
    public interface IEmailProviderTypeService
    {
        /// <summary>
        /// Gets all active email provider types
        /// </summary>
        /// <returns>A JsonModel containing the list of email provider types</returns>
        Task<JsonModel> GetAllActiveProviderTypesAsync();
    }
}
