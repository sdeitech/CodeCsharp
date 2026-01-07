
using App.Application.Dto.EmailFactory;
using App.Application.Interfaces.Services.EmailFactory;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.Api.Controllers.MasterEmailConfiguration
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterEmailConfigurationController : ControllerBase
    {
        private readonly IEmailConfigurationService _emailConfigurationService;
        private readonly ILogger<MasterEmailConfigurationController> _logger;

        public MasterEmailConfigurationController(
            IEmailConfigurationService emailConfigurationService,
            ILogger<MasterEmailConfigurationController> logger)
        {
            _emailConfigurationService = emailConfigurationService ?? throw new ArgumentNullException(nameof(emailConfigurationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Sets or updates email configuration for an organization
        /// </summary>
        /// <param name="emailConfig">The email configuration DTO</param>
        /// <returns>A JsonModel containing the operation result</returns>
        [HttpPost("set-email-configuration")]
        [ProducesResponseType(typeof(JsonModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(JsonModel), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(JsonModel), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SetEmailConfiguration([FromBody] MasterEmailConfigurationDto emailConfig)
        {
            if (emailConfig == null)
            {
                _logger.LogWarning("SetEmailConfiguration called with null emailConfig");
                return BadRequest(new JsonModel
                {
                    Data = false,
                    Message = StatusMessage.InvalidData,
                    StatusCode = (int)HttpStatusCode.BadRequest
                });
            }

            try
            {
                _logger.LogInformation("Setting email configuration for organization {OrganizationId}",
                    emailConfig.OrganizationId);

                var result = await _emailConfigurationService.SetEmailConfigurationAsync(emailConfig);

                if (result.StatusCode == (int)HttpStatusCode.OK)
                {
                    _logger.LogInformation("Successfully set email configuration for organization {OrganizationId}",
                        emailConfig.OrganizationId);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Failed to set email configuration for organization {OrganizationId}: {Message}",
                        emailConfig.OrganizationId, result.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while setting email configuration for organization {OrganizationId}",
                    emailConfig.OrganizationId);

                return StatusCode((int)HttpStatusCode.InternalServerError, new JsonModel
                {
                    Data = false,
                    Message = StatusMessage.InternalServerError,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        /// <summary>
        /// Gets email configurations for an organization
        /// </summary>
        /// <param name="organizationId">The organization ID</param>
        /// <returns>A JsonModel containing the list of email configuration response DTOs</returns>
        [HttpGet("get-email-configuration/{organizationId}")]
        [ProducesResponseType(typeof(JsonModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(JsonModel), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(JsonModel), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetEmailConfiguration(int organizationId)
        {
            if (organizationId <= 0)
            {
                _logger.LogWarning("GetEmailConfiguration called with invalid organizationId: {OrganizationId}", organizationId);
                return BadRequest(new JsonModel
                {
                    Data = false,
                    Message = StatusMessage.InvalidData,
                    StatusCode = (int)HttpStatusCode.BadRequest
                });
            }

            try
            {
                _logger.LogInformation("Getting email configurations for organization {OrganizationId}", organizationId);

                var result = await _emailConfigurationService.GetEmailConfigurationAsync(organizationId);

                if (result.StatusCode == (int)HttpStatusCode.OK)
                {
                    _logger.LogInformation("Successfully retrieved email configurations for organization {OrganizationId}", organizationId);
                    return Ok(result);
                }
                else if (result.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("No email configuration found for organization {OrganizationId}", organizationId);
                    return NotFound(result);
                }
                else
                {
                    _logger.LogWarning("Failed to get email configuration for organization {OrganizationId}: {Message}",
                        organizationId, result.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting email configuration for organization {OrganizationId}", organizationId);

                return StatusCode((int)HttpStatusCode.InternalServerError, new JsonModel
                {
                    Data = false,
                    Message = StatusMessage.InternalServerError,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        /// <summary>
        /// Gets all email configurations
        /// </summary>
        /// <returns>A JsonModel containing the list of email configuration response DTOs</returns>
        [HttpGet("get-all-email-configurations")]
        [ProducesResponseType(typeof(JsonModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(JsonModel), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllEmailConfigurations()
        {
            try
            {
                _logger.LogInformation("Getting all email configurations");

                var result = await _emailConfigurationService.GetAllEmailConfigurationsAsync();

                if (result.StatusCode == (int)HttpStatusCode.OK)
                {
                    _logger.LogInformation("Successfully retrieved all email configurations");
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Failed to get all email configurations: {Message}", result.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all email configurations");

                return StatusCode((int)HttpStatusCode.InternalServerError, new JsonModel
                {
                    Data = false,
                    Message = StatusMessage.InternalServerError,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }
        /// <summary>
        /// Gets email configurations with pagination, search, and sorting
        /// </summary>
        /// <param name="query">The query parameters for pagination, search, and sorting</param>
        /// <returns>A JsonModel containing paginated email configuration results</returns>
        [HttpPost("get-email-configurations")]
        [ProducesResponseType(typeof(JsonModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(JsonModel), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(JsonModel), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetEmailConfigurationsWithPagination([FromBody] Models.EmailConfigurationQueryModel query)
        {
            if (query == null)
            {
                _logger.LogWarning("GetEmailConfigurationsWithPagination called with null query");
                return BadRequest(new JsonModel
                {
                    Data = false,
                    Message = StatusMessage.InvalidData,
                    StatusCode = (int)HttpStatusCode.BadRequest
                });
            }

            try
            {
                _logger.LogInformation("Getting email configurations with pagination. Page: {PageNumber}, Size: {PageSize}, Search: {Search}, Sort: {SortColumn} {SortDirection}",
                    query.PageNumber, query.PageSize, query.Search, query.SortColumn, query.SortDirection);

                // Validate parameters
                if (query.PageNumber < 1)
                {
                    _logger.LogWarning("Invalid page number: {PageNumber}, setting to 1", query.PageNumber);
                    query.PageNumber = 1;
                }

                if (query.PageSize < 1 || query.PageSize > 100)
                {
                    _logger.LogWarning("Invalid page size: {PageSize}, setting to 10", query.PageSize);
                    query.PageSize = 10;
                }

                // Validate sort column
                var validSortColumns = new[] { "OrganizationName", "ProviderName", "ConfigId" };
                if (string.IsNullOrEmpty(query.SortColumn) || !validSortColumns.Contains(query.SortColumn))
                {
                    _logger.LogWarning("Invalid sort column: {SortColumn}, setting to OrganizationName", query.SortColumn);
                    query.SortColumn = "OrganizationName";
                }

                // Validate sort direction
                if (string.IsNullOrEmpty(query.SortDirection) || (query.SortDirection != "ASC" && query.SortDirection != "DESC"))
                {
                    _logger.LogWarning("Invalid sort direction: {SortDirection}, setting to ASC", query.SortDirection);
                    query.SortDirection = "ASC";
                }

                var result = await _emailConfigurationService.GetEmailConfigurationsWithPaginationAsync(
                    pageNumber: query.PageNumber,
                    pageSize: query.PageSize,
                    search: query.Search,
                    sortColumn: query.SortColumn,
                    sortDirection: query.SortDirection
                );

                if (result.StatusCode == (int)HttpStatusCode.OK)
                {
                    _logger.LogInformation("Successfully retrieved email configurations with pagination");
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Failed to get email configurations with pagination: {Message}", result.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting email configurations with pagination");

                return StatusCode((int)HttpStatusCode.InternalServerError, new JsonModel
                {
                    Data = false,
                    Message = StatusMessage.InternalServerError,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }
    }
}
