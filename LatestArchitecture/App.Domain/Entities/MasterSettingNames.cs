using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities
{
    public class MasterSettingNames
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string SettingName { get; set; }

        [Required]
        [MaxLength(255)]
        public string DisplayName { get; set; }

        [Required]
        [MaxLength(50)]
        public string SettingCategory { get; set; } // e.g., System, SMTP, SOS

        public bool IsActive { get; set; } = true;

    }
}
