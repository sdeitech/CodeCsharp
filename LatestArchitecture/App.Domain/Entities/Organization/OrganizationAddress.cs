using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities.Organization
{
    public class OrganizationAddress : BaseEntity
    {
        [Required]
        [ForeignKey("Organization")]
        public int OrganizationID { get; set; }
        [Required]
        [MaxLength(255)]
        public string AddressLine1 { get; set; }
        [MaxLength(255)]
        public string AddressLine2 { get; set; }
        [Required]
        [MaxLength(100)]
        public string City { get; set; }
        [ForeignKey("MasterState")]
        public int? StateID { get; set; }
        [MaxLength(20)]
        public string ZipCode { get; set; }
        [ForeignKey("MasterCountry")]
        public int? CountryID { get; set; }
        [Required]
        public bool IsPrimary { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
    }
}
