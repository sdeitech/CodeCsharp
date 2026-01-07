using App.Application.Dto.Common;
using App.Application.Dto.Organization;
using App.Application.Dto.StorageConfiguration;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Interfaces.Services.Organization;
using App.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Organization
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController(IOrganizationService organizationService, ICurrentUserClaimService currentUserClaimService) : ControllerBase
    {
        private readonly IOrganizationService _organizationService = organizationService;
        private readonly ICurrentUserClaimService _currentUserClaimService = currentUserClaimService;

        /// <summary>
        /// Create a new organization
        /// </summary>
        /// <param name="organizationDTO"></param>
        /// <returns></returns>
        [HttpPost("create-organization")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrganizationAsync([FromBody] OrganizationDto organizationDTO)
        {
            var result = await _organizationService.CreateOrganizationAsync(organizationDTO, Convert.ToInt32(_currentUserClaimService.UserId));

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                //StatusCodes.Status401Unauthorized => Unauthorized(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Update organization details
        /// </summary>
        /// <param name="organizationDTO"></param>
        /// <returns></returns>
        [HttpPost("update-organization")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateOrganizationAsync([FromBody] OrganizationDto organizationDTO)
        {
            var result = await _organizationService.UpdateOrganizationAsync(organizationDTO, Convert.ToInt32(_currentUserClaimService.UserId));

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                //StatusCodes.Status401Unauthorized => Unauthorized(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Update organization status (Active/Inactive)
        /// </summary>
        /// <param name="organizationDTO"></param>
        /// <returns></returns>
        [HttpPost("update-status")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> OrganizationStatusUpdateAsync([FromBody] OrganizationStatusUpdateDto organizationDTO)
        {
            var result = await _organizationService.OrganizationStatusUpdateAsync(organizationDTO, Convert.ToInt32(_currentUserClaimService.UserId));

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                //StatusCodes.Status401Unauthorized => Unauthorized(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Get all organizations with optional filtering
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("get-all")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllOrganizationsAsync([FromBody] FilterDto filter)
        {
            var response = await _organizationService.GetAllOrganizationsAsync(filter);
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                //StatusCodes.Status401Unauthorized => Unauthorized(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response)
            };
        }

        /// <summary>
        /// Get organization by ID
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [HttpGet("get-by-id/{organizationId:int}")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrganizationByIdAsync(int organizationId)
        {
            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            httpContextAccessor.HttpContext = HttpContext;

            var response = await _organizationService.GetOrganizationByIdAsync(organizationId, httpContextAccessor);
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                //StatusCodes.Status401Unauthorized => Unauthorized(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response)
            };
        }

        /// <summary>
        /// Get card statistics for organization dashboard
        /// </summary>
        /// <returns>Card statistics including organization and subscription counts</returns>
        [HttpGet("card-statistics")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCardStatisticsAsync()
        {
            var response = await _organizationService.GetCardStatisticsAsync();
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                //StatusCodes.Status401Unauthorized => Unauthorized(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response)
            };
        }

        /// <summary>
        /// Get storage configuration for an organization
        /// </summary>
        /// <param name="organizationId">Organization ID</param>
        /// <returns>Storage configuration details</returns>
        [HttpGet("storage-configuration/{organizationId:int}")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStorageConfigurationAsync(int organizationId)
        {
            var response = await _organizationService.GetStorageConfigurationAsync(organizationId);
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                StatusCodes.Status404NotFound => NotFound(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response)
            };
        }

        /// <summary>
        /// Save or update storage configuration for an organization
        /// </summary>
        /// <param name="storageConfigurationDto">Storage configuration data</param>
        /// <returns>Success/failure response</returns>
        [HttpPost("storage-configuration")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveStorageConfigurationAsync([FromBody] StorageConfigurationDto storageConfigurationDto)
        {
            var response = await _organizationService.SaveStorageConfigurationAsync(storageConfigurationDto, Convert.ToInt32(_currentUserClaimService.UserId));
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response)
            };
        }

        /// <summary>
        /// Delete storage configuration for an organization
        /// </summary>
        /// <param name="organizationId">Organization ID</param>
        /// <returns>Success/failure response</returns>
        [HttpDelete("storage-configuration/{organizationId:int}")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStorageConfigurationAsync(int organizationId)
        {
            var response = await _organizationService.DeleteStorageConfigurationAsync(organizationId, Convert.ToInt32(_currentUserClaimService.UserId));
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response)
            };
        }

        /// <summary>
        /// Get all storage configurations
        /// </summary>
        /// <returns>List of all storage configurations</returns>
        [HttpGet("storage-configurations")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllStorageConfigurationsAsync()
        {
            var response = await _organizationService.GetAllStorageConfigurationsAsync();
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response)
            };
        }




    }
}
