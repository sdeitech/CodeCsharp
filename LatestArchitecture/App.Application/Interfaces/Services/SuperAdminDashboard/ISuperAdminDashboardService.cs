using App.Application.Dto.Common;
using App.Application.Dto.SuperAdminDashboard;
using App.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces.Services.SuperAdminDashboard
{
    /// <summary>
    /// Service contract to provide dashboard tile counts.
    /// </summary>
    public interface ISuperAdminDashboardService
    {
        Task<JsonModel> GetTileCountsAsync();
        Task<JsonModel> GetAuditLogsAsync(AuditLogFilterDto filter);
    }
}
