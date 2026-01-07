using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.AgencySubscriptions
{
    public class AgencySubscriptionDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("AgencySubscriptionDetailID")]
        public int Id { get; set; }

        public int OrganizationID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Planid { get; set; }

        public string? Status { get; set; }   // varchar(200) NULL
        public string? CustomerId { get; set; }
        public int ClientCount { get; set; }
        public decimal PlatFormRate { get; set; }
        public bool IsLicensed { get; set; }
        public bool IsMonthly { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? SubscriptionID { get; set; }
        public int PerClient { get; set; }
        public string? Note { get; set; }

        public decimal? Discount { get; set; }
        public string? BillingCycle { get; set; }
        public decimal? FinalPrice { get; set; }

        public DateTime? CancelledDate { get; set; }
        public int? Frequency { get; set; }
    }

}
