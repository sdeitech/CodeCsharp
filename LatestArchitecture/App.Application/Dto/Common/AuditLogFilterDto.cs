using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Dto.Common
{
    public class AuditLogFilterDto
    {
        public int PageNumber { get; set; } = 1;

     
        public int PageSize { get; set; } = 10;

  
        public string SortColumn { get; set; } = "LogDate";

       
        public string SortOrder { get; set; } = "DESC";

        [MaxLength(100)]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Optional filter: fetch logs from this date onwards.
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Optional filter: fetch logs up to this date.
        /// </summary>
        public DateTime? ToDate { get; set; }
    }
}
