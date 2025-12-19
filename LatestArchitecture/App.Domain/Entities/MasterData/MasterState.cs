using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities.MasterData
{
    public class MasterState : BaseEntity
    {
        [MaxLength(50)]
        public string? StateName { get; private set; }
        [MaxLength(4)]
        public string? StateAbbr { get; private set; }
        [ForeignKey("MasterCountry")]
        public int? CountryID { get; private set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DaylightSavingTime { get; private set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? StandardTime { get; private set; }
        [MaxLength(250)]
        public string? TimeZoneName { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }
    }
}
