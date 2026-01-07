using System.ComponentModel.DataAnnotations;

namespace App.Application.Dto.EmailFactory
{
    public class EmailProviderConfigDto
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

    public class SmtpConfigDto
    {
        [Required]
        [MaxLength(255)]
        public string SmtpServer { get; set; } = string.Empty;

        [Required]
        public int Port { get; set; }

        [Required]
        [MaxLength(255)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string FromEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FromName { get; set; } = string.Empty;
    }

    public class AwsSesConfigDto
    {
        [Required]
        [MaxLength(255)]
        public string AwsAccessKey { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string AwsSecretKey { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string AwsRegion { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string FromEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FromName { get; set; } = string.Empty;
    }

    public class SendGridConfigDto
    {
        [Required]
        [MaxLength(500)]
        public string ApiKey { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string FromEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FromName { get; set; } = string.Empty;
    }

    public class AzureConfigDto
    {
        [Required]
        [MaxLength(1000)]
        public string ConnectionString { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string SenderAddress { get; set; } = string.Empty;
    }
}
