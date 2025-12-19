using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.Organization
{
    public class OrganizationContact : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        [MaxLength(20)]
        public string Phone { get; set; }
        [ForeignKey("Organization")]
        public int OrganizationID { get; set; }
        [ForeignKey("MasterContactType")]
        public int? ContactTypeID { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
    }
}
