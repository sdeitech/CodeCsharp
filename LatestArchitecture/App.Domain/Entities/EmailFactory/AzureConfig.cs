using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.EmailFactory
{
    [Table("AzureConfig")]
    public class AzureConfig : BaseEntity
    {
        [Required]
        public int EmailProviderConfigId { get; set; }

        [MaxLength(500)]
        public string? ConnectionString { get; set; }

        [MaxLength(100)]
        public string? SenderAddress { get; set; }

        public virtual EmailProviderConfigs? EmailProviderConfig { get; set; }
    }
}
