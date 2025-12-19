using App.Application.Dto.Common;
using App.Application.Dto.MasterSystemSettings;
using App.Domain.Entities.MasterSettings;
using App.Domain.Entities.MasterSystemSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces.Repositories.MasterSystemSettings
{
    public interface IMasterSystemSettingsRepository : IRepository<MasterSystemSetting>
    {
        /// <summary>
        /// Saves the master system setting asynchronously.
        /// </summary>
        /// <param name="masterSystemSetting">The master system setting entity.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<MasterSystemSetting> SaveAsync(MasterSystemSetting masterSystemSetting);

        /// <summary>
        /// Gets all master system settings asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a list of master system settings.</returns>
        Task<IEnumerable<MasterSystemSettingsDto>> GetAllAsync(FilterDto filter);
        
        Task<int> SaveChangesAsync();
        Task<MasterSystemSetting> GetByNameAsync(string name);
    }
}
