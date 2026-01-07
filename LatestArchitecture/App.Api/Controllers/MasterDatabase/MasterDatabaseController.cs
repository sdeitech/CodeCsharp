using App.Application.Dto.Common;
using App.Application.Dto.MasterDatabase;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Interfaces.Services.MasterDatabase;
using App.Application.Service.MasterTimeZones;
using App.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.MasterDatabase
{
    [ServiceFilter(typeof(DateTimeConversionFilterService))]
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDatabaseController(IMasterDatabaseService masterDatabaseService, ICurrentUserClaimService currentUserClaimService) : ControllerBase
    {
        private readonly IMasterDatabaseService _masterDatabaseService = masterDatabaseService;
        private readonly ICurrentUserClaimService _currentUserClaimService = currentUserClaimService;

        /// <summary>
        /// Create a new master database
        /// </summary>
        /// <param name="masterDatabaseDto"></param>
        /// <returns></returns>
        [HttpPost("create-database")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        //[ProducesResponseType(typeof(JsonModel), 401)]
        [ProducesResponseType(typeof(JsonModel), 500)]
        public async Task<IActionResult> CreateMasterDatabaseAsync([FromBody] MasterDatabaseDto masterDatabaseDto)
        {
            var result = await _masterDatabaseService.CreateMasterDatabaseAsync(masterDatabaseDto, Convert.ToInt32(_currentUserClaimService.UserId));

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                400 => BadRequest(result),
                //401 => Unauthorized(result),
                _ => StatusCode(500, result)
            };
        }

        /// <summary>
        /// Update master database details
        /// </summary>
        /// <param name="masterDatabaseDto"></param>
        /// <returns></returns>
        [HttpPost("update-database")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), 400)]
        //[ProducesResponseType(typeof(JsonModel), 401)]
        [ProducesResponseType(typeof(JsonModel), 500)]
        public async Task<IActionResult> UpdateMasterDatabaseAsync([FromBody] MasterDatabaseDto masterDatabaseDto)
        {
            var result = await _masterDatabaseService.UpdateMasterDatabaseAsync(masterDatabaseDto, Convert.ToInt32(_currentUserClaimService.UserId));

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                400 => BadRequest(result),
                //401 => Unauthorized(result),
                _ => StatusCode(500, result)
            };
        }

        /// <summary>
        /// Get all master databases with optional filters
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("get-all")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllMasterDatabaseAsync([FromBody] MasterDatabaseFilterDto filter)
        {
            var response = await _masterDatabaseService.GetAllMasterDatabaseAsync(filter);
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                400 => BadRequest(response),
                //401 => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        /// <summary>
        /// Get all master databases for dropdown selection
        /// </summary>
        /// <returns></returns>
        [HttpPost("get-all-master-databases-dropdown")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllMasterDatabaseDropdownAsync()
        {
            var response = await _masterDatabaseService.GetAllMasterDatabaseDropdownAsync();
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                400 => BadRequest(response),
                //401 => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        /// <summary>
        /// Get master database by ID
        /// </summary>
        /// <param name="databaseId"></param>
        /// <returns></returns>
        [HttpGet("get-by-id/{databaseId:int}")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMasterDatabaseByIdAsync(int databaseId)
        {
            var response = await _masterDatabaseService.GetMasterDatabaseByIdAsync(databaseId);
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                400 => BadRequest(response),
                //401 => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

        /// <summary>
        /// Update the status of a master database (Active/Inactive)
        /// </summary>
        /// <param name="databaseDTO"></param>
        /// <returns></returns>
        [HttpPost("update-status")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MasterDatabaseStatusUpdateAsync([FromBody] MasterDatabaseStatusUpdateDto databaseDTO)
        {
            var result = await _masterDatabaseService.MasterDatabaseStatusUpdateAsync(databaseDTO, Convert.ToInt32(_currentUserClaimService.UserId));

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                //StatusCodes.Status401Unauthorized => Unauthorized(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Get master database counts
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-counts")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMasterDatabaseCountsAsync()
        {
            var response = await _masterDatabaseService.GetMasterDatabaseCountsAsync();
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                400 => BadRequest(response),
                //401 => Unauthorized(response),
                _ => StatusCode(500, response)
            };
        }

    }
}
