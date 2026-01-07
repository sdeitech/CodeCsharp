using App.Application.Dto.AuditLog;
using App.Application.Dto.Common;

namespace App.Application.Interfaces.Repositories.AuditLogs
{
    public interface IAuditLogRepository // : IBaseRepository<AuditLogs>
    {
        public void SaveChangesWithAuditLogs(string screenName, int action, int? userId, int? organizationId, string ipAddress, int portalId, double? latitude, double? longitude, int? PatientId);

        Task<List<AuditLogResponseDto>> GetAuditLogList(FilterDto filter);
        //public GetForeignKeyTableData GetUserValue(int masterTableId, int portalId);
        public void InsertAuditLogs(List<Domain.Entities.AuditLog.AuditLogs> auditLogs);
        //public AuditLogs CreateAuditLog(TokenModel token, int tableId, int columnId, string oldValue, string newValue, int? patientId, int actionId, string screenName, int portalId);
        public int GetTableId(string tableName);
        //public (int ColumnId, string MasterEntity) GetColumnInfo(int tableId, string columnName);
        //public GetForeignKeyTableData GetMasterEntityValue(string masterEntityName, int masterEntitesId, int portalId);
        void PreloadMasterData();

    }
}
