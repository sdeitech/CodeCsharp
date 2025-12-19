using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities
{
    public class OrganizationAdmin
    {
        [Key]
        public int ID { get; private set; }
        [ForeignKey("User")]
        public int UserID { get; private set; }
        [ForeignKey("Organization")]
        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? DeletedAt { get; private set; }
    }
}
