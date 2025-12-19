using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.AuditLogs.AuditLog
{
    public class AuditLogMasterColumn
    {
        [Key]
        public int ColumnID { get; set; }
        [ForeignKey("AuditLogMasterTables")]
        public int TableID { get; set; }

        //public AuditLogMasterColumn(int tableId)
        //{
        //    TableID = tableId;
        //}

        [Required]
        [MaxLength(100)]
        public string ColumnName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? DisplayName { get; set; }
        [MaxLength(100)]
        public string? MasterEntityTableName { get; set; }
        [MaxLength(75)]
        public string? MasterEntityPIDName { get; set; }
        [MaxLength(75)]
        public string? MasterEntityRefName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
