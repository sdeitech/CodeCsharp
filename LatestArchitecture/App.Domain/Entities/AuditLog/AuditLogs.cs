using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.AuditLog
{
    public class AuditLogs
    {
        [Key]
        public int LogID { get; private set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? LogText { get; set; } 
        [ForeignKey("Organization")]
        public int? OrganizationID { get; set; }
        //public int? PatientID { get; set; }
        [ForeignKey("AuditLogsMasterActions")]
        public int ActionID { get; set; }
        [ForeignKey("AuditLogMasterColumn")]
        public int ColumnID { get; set; }
        [MaxLength(100)]
        public string ScreenName { get; set; }
        [ForeignKey("AuditLogMasterTables")]
        public int TableID { get; set; }
        public string IPAddress { get; set; }
        [ForeignKey("AuditLogsStatus")]
        public int StatusID { get; set; }
        //public int? LocationID { get; set; }
        [ForeignKey("MasterPortal")]
        public int PortalID { get; set; }
        public int? ChangedBy { get; set; }
        public DateTime LogDate { get; set; }
        //public double? Latitude { get; set; }
        //public double? Longitude { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; private set; }
    }
}
