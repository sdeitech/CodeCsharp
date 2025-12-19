using App.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.MasterOrg
{
    public class MasterOrganization : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("OrganizationID")]
        public int Id { get; set; }
        [MaxLength(100)]
        public string OrganizationName { get; set; }

        [MaxLength(100)]
        public string BusinessName { get; set; }

        // [NotMapped]
        //public string value { get { return this.OrganizationName; } set { this.OrganizationName = value; } }
        // public string OrganizationName { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }

        [StringLength(200)]
        public string Address1 { get; set; }

        [StringLength(200)]
        public string Address2 { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        public int? StateID { get; set; }

        [StringLength(20)]
        public string Zip { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        public int? CountryID { get; set; }

        [StringLength(40)]
        public string Fax { get; set; }

        [StringLength(250)]
        [EmailAddress]
        public string Email { get; set; }

        public string Logo { get; set; }

        public string Favicon { get; set; }

        [StringLength(50)]
        public string ContactPersonFirstName { get; set; }

        [StringLength(50)]
        public string ContactPersonMiddleName { get; set; }

        [StringLength(50)]
        public string ContactPersonLastName { get; set; }
        [StringLength(15)]
        public string ContactPersonPhoneNumber { get; set; }
        [NotMapped]
        public int UserID { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public int DatabaseDetailId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [StringLength(20)]
        public string ApartmentNumber { get; set; }
        [StringLength(50)]
        public string VendorIdDirect { get; set; }
        [StringLength(50)]
        public string VendorIdIndirect { get; set; }
        [StringLength(250)]
        public string VendorNameDirect { get; set; }
        [StringLength(250)]

        public string VendorNameIndirect { get; set; }
        [Column(TypeName = "varchar(15)")]
        public string PayrollStartWeekDay { get; set; }
        [Column(TypeName = "varchar(15)")]
        public string PayrollEndWeekDay { get; set; }
        public int? AgencyTypeId { get; set; }
        public string medicareId { get; set; }
        public string MedicaidId { get; set; }
        public string evvAssignedNumber { get; set; }
        public string stateAssignedId { get; set; } 
        public bool? UseGoogleAddress { get; set; } 
        public int? DurationId { get; set; }
        public int? Duration {  get; set; }
        public bool? IsDODDPassport { get; set; }
        public bool? IsDODDWaiverNursing { get; set; }
        public bool? IsActive { get; set; }

    }
}
