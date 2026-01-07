using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities.AuditLog
{
    public class AuditLogsStatus
    {
        [Key]
        public int StatusID { get; private set; }
        [Required]
        [MaxLength(200)]
        public string StatusName { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }
    }
}
