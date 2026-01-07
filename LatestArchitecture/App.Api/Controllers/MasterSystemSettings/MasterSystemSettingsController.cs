
using App.Application.Dto.Common;
using App.Application.Dto.MasterSystemSettings;
using App.Application.Interfaces.Services.MasterSystemSettingsService;
using App.Application.Service.MasterTimeZones;
using App.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.MasterSystemSettings
{
    /// <summary>
    /// API controller for managing Master System Settings.
    /// Provides CRUD operations, status toggling, and fetching settings.
    /// </summary>
   [ServiceFilter(typeof(DateTimeConversionFilterService))]
    [Route("api/[controller]")]
    [ApiController]
    public class MasterSystemSettingsController : BaseController
    {
        private readonly IMasterSystemSettingsService _masterSystemSettingsService;

        /// <summary>
        /// Initializes a new instance of <see cref="MasterSystemSettingsController"/>.
        /// </summary>
        /// <param name="masterSystemSettingsService">Service layer for master system settings operations.</param>
        public MasterSystemSettingsController(IMasterSystemSettingsService masterSystemSettingsService)
        {
            _masterSystemSettingsService = masterSystemSettingsService;
        }


        #region Save / Update
        /// <summary>
        /// Creates a new System Setting or updates an existing one based on the provided data.
        /// </summary>
        /// <param name="dto">
        /// The <see cref="MasterSystemSettingDto"/> object containing the system setting details.
        /// If <c>SystemSettingId</c> is null or zero, a new record will be created.  
        /// Otherwise, the existing record will be updated.
        /// </param>
        /// <returns>
        /// Returns 200 OK if operation succeeds,  
        /// 404 Not Found if update target does not exist,  
        /// or 500 Internal Server Error for unexpected errors.
        /// </returns>
        [HttpPost("save")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveAsync([FromBody] MasterSystemSettingsDto dto)
        {
            var result = await _masterSystemSettingsService.SaveAsync(dto);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
        #endregion
        #region Get All
        /// <summary>
        /// Retrieves all active system settings.
        /// </summary>
        /// <returns>Returns list of settings wrapped in <see cref="JsonModel"/>.</returns>
        [HttpPost("getAll")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllAsync([FromBody] FilterDto filter)
        {
            var result = await _masterSystemSettingsService.GetAllAsync(filter);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result)

            };
        }
        #endregion

        #region Get By Id
        /// <summary>
        /// Gets a specific system setting by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the system setting.</param>
        /// <returns>Returns setting details if found, otherwise NotFound.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _masterSystemSettingsService.GetByIdAsync(id);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
        #endregion
        #region Delete (Soft Delete)
        /// <summary>
        /// Soft deletes a system setting by marking IsDeleted flag.
        /// </summary>
        /// <param name="id">The unique identifier of the system setting.</param>
        /// <returns>Returns success/failure response.</returns>
        [HttpDelete("delete/{id:int}")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _masterSystemSettingsService.DeleteAsync(id);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
        #endregion

        #region Toggle Active / Inactive
        /// <summary>
        /// Updates the active status (true = active, false = inactive) of a setting.
        /// </summary>
        /// <param name="id">The unique identifier of the system setting.</param>
        /// <param name="isActive">Boolean value to activate/deactivate.</param>
        /// <returns>Returns updated status response.</returns>
        [HttpPatch("updateStatus/{id:int}")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromQuery] bool isActive)
        {
            var result = await _masterSystemSettingsService.ToggleStatusAsync(id, isActive);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
        #endregion

        #region Toggle Setting Value
        /// <summary>
        /// Toggles the <c>SystemSettingValue</c> (boolean).  
        /// This will be used by frontend toggle buttons.
        /// </summary>
        /// <param name="id">The unique identifier of the system setting.</param>
        /// <param name="value">Boolean value (true = ON, false = OFF).</param>
        /// <returns>Returns updated toggle state.</returns>
        [HttpPatch("toggleValue/{id:int}")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ToggleValueAsync(int id, [FromQuery] bool value)
        {
            var result = await _masterSystemSettingsService.ToggleValueAsync(id, value);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                StatusCodes.Status400BadRequest => BadRequest(result),

                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
        #endregion

    }

}
