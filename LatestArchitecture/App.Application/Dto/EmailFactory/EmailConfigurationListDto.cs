using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Dto.EmailFactory
{
    public class EmailConfigurationListDto
    {
        public List<EmailConfigurationItemDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
    }

    public class EmailConfigurationItemDto
    {
        public int ConfigId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool EnableSsl { get; set; }
        public string? TemplatesPath { get; set; }
        public string? SmtpServer { get; set; }
        public string? FromEmail { get; set; }
        public string? FromName { get; set; }
        public string? SendGridApiKey { get; set; }
        public string? AzureConnection { get; set; }
        public string? AzureSender { get; set; }
        public string? AwsAccessKey { get; set; }
        public string? AwsRegion { get; set; }
        public int? TotalCount { get; set; }
    }
    public class EmailConfigurationResult
    {
        public int ConfigId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
