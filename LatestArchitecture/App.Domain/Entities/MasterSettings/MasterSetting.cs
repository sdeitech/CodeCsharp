using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.MasterSettings
{
    /// <summary>
    /// Represents a configurable system setting stored in the application.
    /// </summary>
    public class MasterSetting : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of the setting.
        /// Example: "SMTP Email Server", "Billing Module", "SSO Login".
        /// </summary>
        public string SettingName { get; set; }

        /// <summary>
        /// Gets or sets the value associated with the setting.
        /// Example: "Enabled/Disabled", "smtp.office365.com", "true/false".
        /// </summary>
        public string SettingValue { get; set; }

        /// <summary>
        /// Gets or sets an optional description that explains the purpose of the setting.
        /// Example: "Defines whether billing module is active", "SMTP server used for outgoing emails".
        /// </summary>
        //public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this setting is active.
        /// Example: true = Active, false = Inactive.
        /// </summary>
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

}
