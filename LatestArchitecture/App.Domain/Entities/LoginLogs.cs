using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities
{
    public class LoginLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoginLogsId { get; set; }

        public int? UserId { get; set; }

        [StringLength(255)]
        public string? IPAddress { get; set; }

        [ForeignKey("MasterPortal")]
        public int PortalId { get; set; }

        public string LoginLogsStatus { get; set; }

        public int? OrganizationId { get; set; }

        [Column(TypeName = "double")]
        public double? Latitude { get; set; }

        [Column(TypeName = "double")]
        public double? Longitude { get; set; }

        [ForeignKey("LoginLogsMasterActions")]
        public int ActionId { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string ScreenName { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

    }
}
