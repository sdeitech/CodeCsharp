using App.Application.Dto.SubscriptionPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Dto.MasterSetting
{
    public class MasterSettingsDto
    {
        /// <summary>
        /// Unique identifier of the setting.
        /// </summary>
        public int Id  { get; set; }

        /// <summary>
        /// The display name of the setting. 
        /// Example: "SMTP Server", "EnableSocialLogin".
        /// </summary>
        public string SettingName { get; set; }

        /// <summary>
        /// The actual value for the setting.
        /// Example: smtp.office365.com, true/false, etc.
        /// </summary>
        public string SettingValue { get; set; }

        /// <summary>
        /// Indicates whether the setting is active (true) or inactive (false).
        /// </summary>
        public bool? IsActive {  get; set; }
        public int? TotalCount {  get; set; }
        public DateTime? CreatedAt {  get; set; } 
       
    }
}
