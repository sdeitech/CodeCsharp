using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities.MasterDatabase
{
    public class MasterDatabase : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string DatabaseName { get; set; }
        [Required]
        [MaxLength(255)]
        public string Password { get; set; }
        [Required]
        [MaxLength(255)]
        public string ServerName { get; set; }
        [Required]
        [MaxLength(255)]
        public string UserName { get; set; }
        public bool? IsCentralized { get; set; }
        public int? ParentOrganizationID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
