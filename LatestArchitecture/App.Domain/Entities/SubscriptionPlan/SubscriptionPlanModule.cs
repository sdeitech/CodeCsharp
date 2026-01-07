using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.SubscriptionPlan { 
    public class SubscriptionPlanModules
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubscriptionModuleid { get; set; }
        public int Moduleid { get; set; }
        public int? Screenid { get; set; }
        public int? Actionid { get; set; }
        public int Planid { get; set; }

    }
}
