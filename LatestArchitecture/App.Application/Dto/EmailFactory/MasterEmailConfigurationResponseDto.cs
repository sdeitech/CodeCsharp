using System.ComponentModel.DataAnnotations;

namespace App.Application.Dto.EmailFactory
{
    public class MasterEmailConfigurationResponseDto
    {
        public int Id { get; set; }

        public int OrganizationId { get; set; }

        public string? OrganizationName { get; set; }

        public int EmailProviderTypeId { get; set; }

        public string? EmailProviderTypeName { get; set; }

        public bool IsActive { get; set; }

        public bool EnableSsl { get; set; }

        [MaxLength(500)]
        public string? TemplatesPath { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // SMTP Configuration
        public SmtpConfigResponseDto? SmtpConfig { get; set; }

        // AWS SES Configuration
        public AwsSesConfigResponseDto? AwsSesConfig { get; set; }

        // SendGrid Configuration
        public SendGridConfigResponseDto? SendGridConfig { get; set; }

        // Azure Configuration
        public AzureConfigResponseDto? AzureConfig { get; set; }
    }

    public class SmtpConfigResponseDto
    {
        public int Id { get; set; }

        public string? SmtpServer { get; set; }

        public int? Port { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string? FromEmail { get; set; }

        public string? FromName { get; set; }
    }

    public class AwsSesConfigResponseDto
    {
        public int Id { get; set; }

        public string? AwsAccessKey { get; set; }

        public string? AwsSecretKey { get; set; }

        public string? AwsRegion { get; set; }

        public string? FromEmail { get; set; }

        public string? FromName { get; set; }
    }

    public class SendGridConfigResponseDto
    {
        public int Id { get; set; }

        public string? ApiKey { get; set; }

        public string? FromEmail { get; set; }

        public string? FromName { get; set; }
    }

    public class AzureConfigResponseDto
    {
        public int Id { get; set; }

        public string? ConnectionString { get; set; }

        public string? SenderAddress { get; set; }
    }
}
