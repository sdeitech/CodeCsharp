using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities.AuditLog
{
    public class AuditLogsMasterActions
    {
        [Key]
        public int ActionID { get; private set; }
        [Required]
        [MaxLength(100)]
        public string ActionName { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }
    }
}
