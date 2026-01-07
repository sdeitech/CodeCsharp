using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Domain.Entities.Organization;
namespace App.Domain.Entities.EmailFactory
{
    [Table("EmailProviderConfigs")]
    public class EmailProviderConfigs : BaseEntity
    {
        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public int EmailProviderTypeId { get; set; }  // FK to Provider Type

        [Required]
        public bool IsActive { get; set; } = true;

        public bool EnableSsl { get; set; } = true;

        [MaxLength(500)]
        public string? TemplatesPath { get; set; }

        // Navigation properties
        public virtual EmailProviderType? EmailProviderType { get; set; }
    

        // Provider-specific navigation
        public virtual SmtpConfig? SmtpConfig { get; set; }
        public virtual AwsSesConfig? AwsSesConfig { get; set; }
        public virtual SendGridConfig? SendGridConfig { get; set; }
        public virtual AzureConfig? AzureConfig { get; set; }
    }
}
