using App.Application.Dto.Common;
using App.Application.Dto.SuperAdminDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Interfaces.Repositories.SuperAdminDashboard
{
    /// <summary>
    /// Defines contract for fetching dashboard statistics.
    /// </summary>
    public interface ISuperAdminDashboardRepository
    {
        Task<SuperAdminDashboardTileCountsDto> GetSuperadminDashboardTileCountsAsync();
        Task<IEnumerable<PagedAuditLogResponseDto>>GetAuditLogsPagedAsync(AuditLogFilterDto filter);
    }
}
