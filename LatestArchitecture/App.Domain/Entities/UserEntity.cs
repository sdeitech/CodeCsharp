using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities
{
    public class UserEntity
    {
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password_hash { get; set; } = string.Empty;
        public string SaltName { get; set; } = string.Empty;
        public int OrganizationId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int ThirdPartyLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
