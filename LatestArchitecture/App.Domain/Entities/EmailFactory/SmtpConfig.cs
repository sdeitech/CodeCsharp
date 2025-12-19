using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.EmailFactory
{
    [Table("SmtpConfig")]
    public class SmtpConfig : BaseEntity
    {
        [Required]
        public int EmailProviderConfigId { get; set; }

        [MaxLength(200)]
        public string? SmtpServer { get; set; }

        public int? Port { get; set; }

        [MaxLength(100)]
        public string? UserName { get; set; }

        [MaxLength(500)]
        public string? Password { get; set; }

        [MaxLength(100)]
        public string? FromEmail { get; set; }

        [MaxLength(100)]
        public string? FromName { get; set; }

        public virtual EmailProviderConfigs? EmailProviderConfig { get; set; }
    }
}
