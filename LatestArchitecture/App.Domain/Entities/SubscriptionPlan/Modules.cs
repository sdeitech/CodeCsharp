using App.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.SubscriptionPlan
{
    public class Modules
    {
        [Key]
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ModuleId")]
        public int ModuleId { get; set; }

        [StringLength(100)]
        public string ModuleName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string ModuleKey { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        [Column(TypeName = "varchar(250)")]
        public string ModuleIcon { get; set; }
        [StringLength(100)]
        public string NavigationLink { get; set; }
        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        [ForeignKey("Menu")]
        public int? MenuID { get; set; }

        public virtual Menu Menu { get; set; }
    }


}
