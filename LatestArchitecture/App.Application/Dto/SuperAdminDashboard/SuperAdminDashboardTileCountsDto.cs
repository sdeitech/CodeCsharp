using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Dto.SuperAdminDashboard
{
    public class SuperAdminDashboardTileCountsDto
    {
        public int TotalAgencies { get; set; }
        public int TotalSubscriptionPlans { get; set; }
        public int TotalDatabases { get; set; }
        public int TotalTickets { get; set; }
    }
}
