namespace App.SuperAdmin.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; private set; }
        //public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
        //public DateTimeOffset? DeletedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public int CreatedBy { get; private set; }
        public int UpdatedBy { get; private set; }
        public int DeletedBy { get; private set; }
        public DateTime? CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; private set; }
        public DateTime? DeleteDate { get; private set; }
    }
}
