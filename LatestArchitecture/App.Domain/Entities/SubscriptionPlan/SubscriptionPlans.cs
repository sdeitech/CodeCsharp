using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.SubscriptionPlan
{
    public class SubscriptionPlans : BaseEntity
    {
        public string PlanName { get; set; }
        public int PerClient { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsMonthly { get; set; }
        public decimal PlatFormRate { get; set; }
        public string? ProductId { get; set; }
        public string? PriceId { get; set; }
        public int? ClientCount { get; set; }
        public bool? IsLicensed { get; set; }
    }
}
