using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities.MasterData
{
    public class MasterCountry : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string CountryName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
