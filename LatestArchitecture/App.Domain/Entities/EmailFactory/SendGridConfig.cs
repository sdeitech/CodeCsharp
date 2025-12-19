using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.EmailFactory
{
    [Table("SendGridConfig")]
    public class SendGridConfig : BaseEntity
    {
        [Required]
        public int EmailProviderConfigId { get; set; }

        [MaxLength(500)]
        public string? ApiKey { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string FromEmail { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? FromName { get; set; }
        public virtual EmailProviderConfigs? EmailProviderConfig { get; set; }
    }

}
