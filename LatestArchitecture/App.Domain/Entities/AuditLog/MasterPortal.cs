using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities.AuditLog
{
    public class MasterPortal
    {
        [Key]
        public int PortalID { get; private set; }
        [Required]
        [MaxLength(30)]
        public string PortalName { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }
    }
}
