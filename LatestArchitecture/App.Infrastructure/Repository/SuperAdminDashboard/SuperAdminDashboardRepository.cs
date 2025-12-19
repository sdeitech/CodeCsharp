using App.Application.Dto.Common;
using App.Application.Dto.MasterDatabase;
using App.Application.Dto.MasterSetting;
using App.Application.Dto.SuperAdminDashboard;
using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.SuperAdminDashboard;
using App.Common.Constant;
using App.Domain.Entities.MasterSettings;
using App.SharedConfigs.DBContext;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Repository.SuperAdminDashboard
{
    /// <summary>
    /// Repository to fetch dashboard data from database using stored procedures.
    /// </summary>
    public class SuperAdminDashboardRepository(MasterDbContext context, IDbConnectionFactory dbConnectionFactory)
    : BaseRepository<MasterSetting>(context, dbConnectionFactory), ISuperAdminDashboardRepository
    {
        public async Task<IEnumerable<PagedAuditLogResponseDto>> GetAuditLogsPagedAsync(AuditLogFilterDto filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SearchTerm", filter.SearchTerm);
            parameters.Add("@SortColumn", filter.SortColumn);
            parameters.Add("@SortOrder", filter.SortOrder ?? "DESC");
            parameters.Add("@PageNumber", filter.PageNumber <= 0 ? 1 : filter.PageNumber);
            parameters.Add("@PageSize", filter.PageSize <= 0 ? 10 : filter.PageSize);
            parameters.Add("@FromDate", filter.FromDate);
            parameters.Add("@ToDate", filter.ToDate);

            return await _dbConnection.QueryAsync<PagedAuditLogResponseDto>(
                SqlMethod.MST_GetAuditLogsPaged,
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<SuperAdminDashboardTileCountsDto> GetSuperadminDashboardTileCountsAsync()
        {
            var dto = await _dbConnection.QueryFirstOrDefaultAsync<SuperAdminDashboardTileCountsDto>(
        App.Common.Constant.SqlMethod.sp_GetSuperadminDashboardTileCounts,
        commandType: CommandType.StoredProcedure,
        commandTimeout: 120
    );

            return dto ?? new SuperAdminDashboardTileCountsDto();
        }

    }
}
