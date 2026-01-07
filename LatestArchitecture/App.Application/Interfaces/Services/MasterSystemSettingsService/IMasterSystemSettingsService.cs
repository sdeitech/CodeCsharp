using App.Application.Dto.Common;
using App.Application.Dto.MasterSetting;
using App.Application.Dto.MasterSystemSettings;
using App.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces.Services.MasterSystemSettingsService
{
    /// <summary>
    /// Service interface for managing Master System Settings.
    /// Provides methods for CRUD operations, status toggling, and value toggling.
    /// </summary>
    public interface IMasterSystemSettingsService
    {
       
      
        /// <summary>
        /// Creates a new system setting or updates an existing one.
        /// </summary>
        /// <param name="dto">The DTO object containing system setting details.</param>
        /// <returns>
        /// A <see cref="JsonModel"/> containing the result of the operation
        /// (success/failure, status code, and message).
        /// </returns>
        Task<JsonModel> SaveAsync(MasterSystemSettingsDto masterSettingDto);
        /// <summary>
        /// Retrieves all system settings from the database.
        /// </summary>
        /// <returns>
        /// A <see cref="JsonModel"/> containing the list of all system settings.
        /// </returns>
        Task<JsonModel> GetAllAsync(FilterDto filter);
        /// <summary>
        /// Retrieves a specific system setting by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the system setting.</param>
        /// <returns>
        /// A <see cref="JsonModel"/> containing the system setting details if found,
        /// otherwise a not found response.
        /// </returns>
        Task<JsonModel> GetByIdAsync(int id);
        /// <summary>
        /// Soft deletes a system setting by marking it as deleted.
        /// </summary>
        /// <param name="settingId">The unique identifier of the system setting.</param>
        /// <returns>
        /// A <see cref="JsonModel"/> containing success/failure response.
        /// </returns>
        Task<JsonModel> DeleteAsync(int settingId);

        /// <summary>
        /// Activates or deactivates a system setting by updating the IsActive flag.
        /// </summary>
        /// <param name="settingId">The unique identifier of the system setting.</param>
        /// <param name="isActive">Boolean flag: true = activate, false = deactivate.</param>
        /// <returns>
        /// A <see cref="JsonModel"/> containing success/failure response.
        /// </returns>
        Task<JsonModel> ToggleStatusAsync(int settingId, bool isActive);

        /// <summary>
        /// Toggles the boolean value of SystemSettingValue.
        /// This is primarily used for frontend toggle switches.
        /// </summary>
        /// <param name="settingId">The unique identifier of the system setting.</param>
        /// <param name="value">Boolean value: true = enabled, false = disabled.</param>
        /// <returns>
        /// A <see cref="JsonModel"/> containing success/failure response.
        /// </returns>
        Task<JsonModel> ToggleValueAsync(int settingId, bool value);

    }
}
