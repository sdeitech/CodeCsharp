namespace App.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? DeletedBy { get; set; }
    public int? UpdatedBy { get; set; }

}
public abstract class BaseEntityWithoutID
{

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CreatedBy { get; set; }
    public int DeletedBy { get; set; }
    public int UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;

}