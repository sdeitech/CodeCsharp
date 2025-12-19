using App.Application.Dto.Common;
using App.Application.Dto.SuperAdminDashboard;
using App.Application.Interfaces.Services.SuperAdminDashboard;
using App.Application.Service.SuperAdminDashboard;
using App.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ClientModel.Primitives;

namespace App.Api.Controllers.SuperAdminDashboard
{
    /// <summary>
    /// Exposes endpoints for fetching SuperAdmin dashboard data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SuperAdminDashboardController  :BaseController
    {
        private readonly ISuperAdminDashboardService _dashboardService;

        public SuperAdminDashboardController(ISuperAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Gets the tile counts for SuperAdmin dashboard.
        /// </summary>
        [HttpGet("dashboard/tile-counts")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDashboardTileCountsAsync()
        {
            var result = await _dashboardService.GetTileCountsAsync();

            return result.StatusCode == StatusCodes.Status200OK
         ? Ok(result)
         : NoContent();
        }

        [HttpPost("auditlogs/search")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SearchAuditLogsAsync([FromBody] AuditLogFilterDto filter)
        {
            var result = await _dashboardService.GetAuditLogsAsync(filter);

            if (result == null)
                return NoContent();

            return Ok(result);
        }






    }
}
