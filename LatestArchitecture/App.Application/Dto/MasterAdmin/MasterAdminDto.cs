using System;
using System.ComponentModel.DataAnnotations;

namespace App.Application.Dto.MasterAdmin
{
    public class MasterAdminDto
    {
        public int? AgencyAdminId { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Name must be between 2 and 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "RoleId is required")]
        public int RoleId { get; set; } // FK to MasterRoles

        public string? RoleName { get; set; } // From MasterRoles

        [Required(ErrorMessage = "AgencyId is required")]
        public int AgencyId { get; set; } // FK to Agency

        public string? AgencyName { get; set; } // From Agency

        [StringLength(1000, ErrorMessage = "Notes must not exceed 1000 characters")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(255, ErrorMessage = "Email must not exceed 255 characters")]
        public string? Email { get; set; }

        // Status
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        public int? TotalCount { get; set; }
    }



    public class AddressDto
    {
        public int AgencyAddressId { get; set; } 
        public int AgencyId { get; set; } // FK to Agency
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipPostalCode { get; set; }
        public int? CountryId { get; set; } // FK to MasterCountry
        public string? CountryName { get; set; } // From MasterCountry
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ContactDto
    {
        public int AgencyContactId { get; set; } 
        public string? AgencyContactFirstName { get; set; }
        public string? AgencyContactLastName { get; set; }
        public string? AgencyContactEmail { get; set; }
        public string? AgencyContactPhone { get; set; }
        public int? AgencyId { get; set; }
        public int? PositionId { get; set; } // FK to MasterPosition
        public string? PositionName { get; set; } // From MasterPosition
        public int? ContactTypeId { get; set; } // FK to MasterContactType
        public string? ContactTypeName { get; set; } // From MasterContactType
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class AdminResponseDto
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int? AgencyAdminId { get; set; }
    }

}
