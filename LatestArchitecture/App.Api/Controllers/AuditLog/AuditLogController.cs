using App.Application.Dto.Common;
using App.Application.Interfaces.Services.AuditLog;
using App.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.AuditLog
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController(IAuditLogService auditLogService) : ControllerBase
    {
        private readonly IAuditLogService _auditLogService = auditLogService;

        /// <summary>
        /// Get Audit Logs based on filter criteria
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("get")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(JsonModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuditLogAsync(FilterDto filter)
        {
            var response = await _auditLogService.GetAuditLogsAsync(filter);
            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                //StatusCodes.Status401Unauthorized => Unauthorized(response),
                _ => StatusCode(StatusCodes.Status500InternalServerError, response)
            };
        }
    }
}
