using App.Application.Dto.Common;
using App.Application.Dto.MasterSetting;
using App.Common.Models;
using App.Domain.Entities;
using App.Domain.Entities.MasterSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces.Repositories.MasterSettings
{
    public interface IMasterSettingsRepository : IRepository<MasterSetting>
    {
        /// <summary>
        /// Saves the master setting asynchronously.
        /// </summary>
        /// <param name="masterSettingDto">The master setting data transfer object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<MasterSetting> SaveAsync(MasterSetting masterSetting);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<MasterSettingsDto>> GetAllAsync(FilterDto filter); 
        Task<List<MasterSettingNames>> GetAllSettingNamesAsync(); 
        /// <summary>
        /// Retrieves a MasterSetting entity by its name (case-insensitive).
        /// Returns null if not found.
        /// </summary>
        /// <param name="name">The setting name to search for.</param>
        Task<MasterSetting> GetByNameAsync(string name);



    }
} 
