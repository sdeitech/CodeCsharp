using App.Application.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Dto.SuperAdminDashboard
{
    public class PagedAuditLogResponseDto
    {
        //public IEnumerable<AuditLogFilterDto> Data { get; set; }
        public DateTime LogDate { get; set; }
        public string TableName { get; set; }
        public string DisplayName { get; set; }
        public string ActionName { get; set; }
        public string StatusName { get; set; }
        public string PortalName { get; set; }
        public int TotalRecords { get; set; }
       

    }
}
