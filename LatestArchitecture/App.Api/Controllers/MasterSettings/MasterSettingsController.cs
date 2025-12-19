using App.Application.Dto.Common;
using App.Application.Dto.MasterSetting;
using App.Application.Interfaces.Services.MasterSettings;
using App.Application.Service.MasterSettings;
using App.Application.Service.MasterTimeZones;
using App.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.MasterSettings
{
    /// <summary>
    /// API controller for managing <see cref="MasterSettings"/> records.
    /// Provides endpoints for CRUD operations on master settings.
    /// </summary>
   [ServiceFilter(typeof(DateTimeConversionFilterService))]
    [Route("api/[controller]")]
    [ApiController]
    public class MasterSettingsController : BaseController
    {
        private readonly IMasterSettingsService _masterSettingsService;

        /// <summary>
        /// Initializes a new instance of <see cref="MasterSettingsController"/>.
        /// </summary>
        /// <param name="masterSettingservice">Service layer for master setting operations.</param>
        public MasterSettingsController(IMasterSettingsService masterSettingsService)
            => _masterSettingsService = masterSettingsService;

        /// <summary>
        /// Creates a new Master Setting or updates an existing one based on the provided data.
        /// </summary>
        /// <param name="masterSettingDto">
        /// The <see cref="MasterSettingDto"/> object containing the master setting details.
        /// If <c>MasterSettingId</c> is null or zero, a new record will be created.  
        /// Otherwise, the existing record will be updated.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the result of the operation.  
        /// Returns:
        /// - 200 (OK): When the operation is successful.  
        /// - 404 (NotFound): If the requested record for update does not exist.  
        /// - 500 (InternalServerError): For unexpected errors.  
        /// </returns>
        [HttpPost("save")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> SaveAsync([FromBody] MasterSettingsDto masterSettingDto)
        {

            var result = await _masterSettingsService.SaveAsync(masterSettingDto);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Get all master settings list.
        /// </summary>
        /// <returns>Returns list of settings in a JsonModel</returns>
        /// 
        [HttpPost("getAll")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllAsync(FilterDto filter)
        {
            var result = await _masterSettingsService.GetAllAsync(filter);

            return Ok(result);


        }

        /// <summary>
        /// Delete a specific setting by Id.
        /// </summary>
        /// <param name="id">Setting Id</param>
        /// <returns>Success/Failure response</returns>
        [HttpDelete("delete/{id:int}")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _masterSettingsService.DeleteAsync(id);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Activate or Deactivate a setting.
        /// </summary>
        /// <param name="id">Setting Id</param>
        /// <param name="isActive">true = activate, false = deactivate</param>
        /// <returns>Updated status response</returns>
        [HttpPatch("updateStatus/{id:int}")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromQuery] bool isActive)
        {
            var result = await _masterSettingsService.ToggleStatusAsync(id, isActive);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Gets a master setting by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the master setting.</param>
        /// <returns>Returns the master setting details if found, otherwise NotFound.</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _masterSettingsService.GetByIdAsync(id);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Get all active setting names.
        /// </summary>
        /// <returns>Returns list of setting names in a JsonModel</returns>
        [HttpGet("getAllSettingNames")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllSettingNames()
        {
            var result = await _masterSettingsService.GetAllSettingNamesAsync();

            return result.StatusCode == StatusCodes.Status200OK
                ? Ok(result)
                : NotFound(result);
        }

    }
}
