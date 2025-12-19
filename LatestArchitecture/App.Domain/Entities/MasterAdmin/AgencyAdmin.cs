﻿namespace App.Domain.Entities.MasterAdmin
{
    public class AgencyAdmin
    {
        public int AgencyAdminId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string Email { get; set; }
        public int AgencyId { get; set; }
        public string? Notes { get; set; }
        public bool IsDelete { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? DeletedAt { get; set; }

    }
}
