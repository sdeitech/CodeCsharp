using App.Application.Dto.AuditLog;
using App.Application.Dto.Common;
using App.Application.Interfaces.Repositories.AuditLogs;
using App.Application.Interfaces.Services.AuditLog;
using App.Common.Constant;
using App.Common.Models;
using System.Net;

namespace App.Application.Service.AuditLog
{
    public class AuditLogService(IAuditLogRepository auditLogRepository) : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;

        public async Task<JsonModel> GetAuditLogsAsync(FilterDto filter)
        {
            List<AuditLogResponseDto> data = await _auditLogRepository.GetAuditLogList(filter);

            return data.Any()
            ? new JsonModel { Data = data, StatusCode = (int)HttpStatusCode.OK }
            : new JsonModel { Message = StatusMessage.InternalServerError, StatusCode = (int)HttpStatusCode.InternalServerError };
        }
    }
}
