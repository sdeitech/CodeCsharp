using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.EmailFactory
{
    [Table("EmailProviderTypes")]
    public class EmailProviderType : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<EmailProviderConfigs>? EmailProviderConfigs { get; set; }
    }
}
