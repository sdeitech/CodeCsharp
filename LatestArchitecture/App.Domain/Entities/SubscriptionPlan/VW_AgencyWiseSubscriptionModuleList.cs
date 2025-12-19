using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.SubscriptionPlan
{
    public class VW_AgencyWiseSubscriptionModuleList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public decimal PlatFormRate { get; set; }
        public int PerClient { get; set; }
        public bool? IsActive { get; set; }
        public int ModuleId { get; set; }
        public int AgencyPlanId { get; set; }
        public int OrganizationId { get; set; }
        public string ModuleName { get; set; }
    }
}
