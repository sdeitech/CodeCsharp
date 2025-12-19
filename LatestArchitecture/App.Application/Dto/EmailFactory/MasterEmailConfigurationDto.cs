using System.ComponentModel.DataAnnotations;

namespace App.Application.Dto.EmailFactory
{
    public class MasterEmailConfigurationDto
    {
        public int? Id { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public int EmailProviderTypeId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public bool EnableSsl { get; set; } = true;

        [MaxLength(500)]
        public string? TemplatesPath { get; set; }

        // SMTP Configuration
        public SmtpConfigDto? SmtpConfig { get; set; }

        // AWS SES Configuration
        public AwsSesConfigDto? AwsSesConfig { get; set; }

        // SendGrid Configuration
        public SendGridConfigDto? SendGridConfig { get; set; }

        // Azure Configuration
        public AzureConfigDto? AzureConfig { get; set; }
    }
}
