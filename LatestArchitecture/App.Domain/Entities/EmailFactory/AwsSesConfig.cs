using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.EmailFactory
{
    [Table("AwsSesConfig")]
    public class AwsSesConfig : BaseEntity
    {
        [Required]
        public int EmailProviderConfigId { get; set; }

        [MaxLength(100)]
        public string? AwsAccessKey { get; set; }

        [MaxLength(100)]
        public string? AwsSecretKey { get; set; }

        [MaxLength(100)]
        public string? AwsRegion { get; set; }

        [MaxLength(100)]
        public string? FromEmail { get; set; }


        [MaxLength(100)]
        public string? FromName{ get; set; }
        public virtual EmailProviderConfigs? EmailProviderConfig { get; set; }
    }
}
