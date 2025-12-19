using App.Application.Dto.Common;
using App.Common.Models;

namespace App.Application.Interfaces.Services.AuditLog
{
    public interface IAuditLogService
    {
        public Task<JsonModel> GetAuditLogsAsync(FilterDto filter);
    }
}
