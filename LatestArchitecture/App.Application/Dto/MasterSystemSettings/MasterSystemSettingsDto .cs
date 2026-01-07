using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Dto.MasterSystemSettings 
{
    public class MasterSystemSettingsDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "System Setting Name is required.")]
        [MinLength(1, ErrorMessage = "System Setting Name cannot be an empty string.")]
        public string SystemSettingName { get; set; } = string.Empty;
        public bool SystemSettingValue { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? TotalCount { get; set; } 

    }
}
