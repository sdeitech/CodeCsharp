using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.MasterTimeZone 
{
    [Table("MasterTimeZones")]
    public class MasterTimeZones  :BaseEntity
    {
        [Key]
        public int TimeZoneId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TimeZoneName { get; set; }

        [Required]
        [MaxLength(150)]
        public string DisplayName { get; set; }

        [Required]
        [MaxLength(10)]
        public string UTCOffset { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

    }
}
