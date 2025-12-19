using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.MasterSystemSettings
{
    /// <summary>
    /// Entity class representing the MasterSystemSetting table.
    /// Stores system configuration values that can be toggled or activated/deactivated.
    /// </summary>
    [Table("MasterSystemSetting")]
    public class MasterSystemSetting : BaseEntity
    {

        /// <summary>
        /// Name of the system setting.
        /// </summary>
        [Required, MaxLength(100)]
        public string SystemSettingName { get; set; } = string.Empty;
        /// <summary>
        /// Boolean value of the setting.
        /// This will be used as a toggle on the frontend (true = ON, false = OFF).
        /// </summary>
        [Required]
        public bool SystemSettingValue { get; set; }
        /// <summary>
        /// Indicates whether the setting is currently active (true = active, false = inactive).
        /// Default = true.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Indicates whether the setting is marked as deleted (soft delete).
        /// Default = false.
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}
