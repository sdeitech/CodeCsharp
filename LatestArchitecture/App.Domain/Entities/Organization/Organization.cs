using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.Organization
{
    public class Organization : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string OrganizationName { get; private set; }
        [MaxLength(255)]
        public string? LogoLocalPath { get; set; }
        [MaxLength(255)]
        public string? LogoAWSPath { get; set; }
        [MaxLength(255)]
        public string? LogoBlobPath { get; private set; }
        [MaxLength(255)]
        public string? FavIconLocalPath { get; set; }
        [MaxLength(255)]
        public string? FavIconAWSPath { get; set; }
        [MaxLength(255)]
        public string? FavIconBlobPath { get; private set; }
        [ForeignKey("MasterDatabase")]
        public int DatabaseID { get; private set; }
        [MaxLength(200)]
        public string? DomainURL { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
