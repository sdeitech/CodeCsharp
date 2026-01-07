using App.Application.Dto.AuditLog;
using App.Application.Dto.Common;

namespace App.Application.Interfaces.Repositories.AuditLogs
{
    public interface IAuditLogAgencyRepository
    {
        public void SaveChangesWithAuditLogs(string screenName, int action, int? userId, int? organizationId, string ipAddress, int portalId, double? latitude, double? longitude, int? PatientId);
        Task<List<AuditLogResponseDto>> GetAuditLogList(FilterDto filter);
        public void InsertAuditLogs(List<Domain.Entities.AuditLog.AuditLogs> auditLogs);
        public int GetTableId(string tableName);
        void PreloadMasterData();
    }
}
