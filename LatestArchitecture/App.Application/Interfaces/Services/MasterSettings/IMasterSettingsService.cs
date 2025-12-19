using App.Application.Dto.Common;
using App.Application.Dto.MasterSetting;
using App.Application.Dto.SubscriptionPlan;
using App.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces.Services.MasterSettings
{
    /// <summary>
    /// Interface for handling Master Settings service operations.
    /// Provides contract for CRUD operations on MasterSettings.
    /// </summary>
    public interface IMasterSettingsService
    {

        /// <summary>
        /// Save or update a master setting.
        /// </summary>
        /// <param name="masterSettingDto">The DTO object containing master setting details.</param>
        /// <returns>JsonModel containing success/failure response.</returns>
        Task<JsonModel> SaveAsync(MasterSettingsDto masterSettingDto);

        /// <summary>
        /// Get all master settings from the system.
        /// </summary>
        /// <returns>JsonModel containing the list of master settings.</returns>
        Task<JsonModel> GetAllAsync(FilterDto filter);
        Task<JsonModel> GetAllSettingNamesAsync(); 

        /// <summary>
        /// Delete a master setting by its ID.
        /// </summary>
        /// <param name="settingId">The unique identifier of the setting to delete.</param>
        /// <returns>JsonModel containing success/failure response.</returns>
        /// 


        Task<JsonModel> DeleteAsync(int settingId);

        /// <summary>
        /// Activate or deactivate a master setting by its ID.
        /// </summary>
        /// <param name="settingId">The unique identifier of the setting to toggle.</param>
        /// <param name="isActive">Flag indicating whether to activate (true) or deactivate (false).</param>
        /// <returns>JsonModel containing success/failure response.</returns>
        Task<JsonModel> ToggleStatusAsync(int settingId, bool isActive);

        /// <summary>
        /// Gets a specific master setting by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the setting.</param>
        /// <returns>A JsonModel containing the setting details.</returns>
        Task<JsonModel> GetByIdAsync(int id);
    }
}

