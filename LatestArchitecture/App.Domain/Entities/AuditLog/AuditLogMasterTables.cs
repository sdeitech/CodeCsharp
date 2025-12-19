using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities.AuditLogs.AuditLog
{
    public class AuditLogMasterTables
    {
        [Key]
        public int TableID { get; set; }
        [Required]
        [MaxLength(100)]
        public string TableName { get; private set; }
        [MaxLength(100)]
        public string? DisplayName { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }
    }
}
