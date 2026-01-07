using App.Application.Dto.Common;
using App.Application.Dto.SuperAdminDashboard;
using App.Application.Interfaces.Repositories.MasterSettings;
using App.Application.Interfaces.Repositories.SuperAdminDashboard;
using App.Application.Interfaces.Services.SuperAdminDashboard;
using App.Common.Constant;
using App.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Service.SuperAdminDashboard
{
    /// <summary>
    /// Service layer for business logic related to dashboard statistics.
    /// </summary>
    public class SuperAdminDashboardService : ISuperAdminDashboardService
    {
        private readonly ISuperAdminDashboardRepository _repository;
        public SuperAdminDashboardService(ISuperAdminDashboardRepository repository)
        {
            _repository = repository;
        }

        public async  Task<JsonModel> GetAuditLogsAsync(AuditLogFilterDto filter)
        {
            var auditLogs = await _repository.GetAuditLogsPagedAsync(filter);
            if (auditLogs == null || !auditLogs.Any())
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.NoContent,
                    Message = StatusMessage.NoContent,
                    Data = false
                };
            }
            int totalRecords = Convert.ToInt32(auditLogs.FirstOrDefault()?.TotalRecords ?? 0);

            return new JsonModel
            {
                Meta = new Meta
                {
                    TotalRecords = totalRecords,
                    CurrentPage = filter.PageNumber,
                    PageSize = filter.PageSize,
                    DefaultPageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling((decimal)totalRecords / filter.PageSize)
                },
                StatusCode = (int)HttpStatusCode.OK,
                Message = StatusMessage.SettingsRetrieved,
                Data = auditLogs
            };
        }

        public async Task<JsonModel> GetTileCountsAsync()
        {
            
            var dashboardTileCount = await _repository.GetSuperadminDashboardTileCountsAsync();

            if (dashboardTileCount == null)
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.NoContent,
                    Message = StatusMessage.NoContent,
                    Data = false
                };
            }

            return new JsonModel
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = StatusMessage.Retrieved,
                Data = dashboardTileCount
            };
        }
    }
}
