using App.Domain.Entities;

namespace App.Domain.Entities.DynamicQuestionnaire;

public class Page : BaseEntity
{
    public int FormId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int PageOrder { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int OrganizationId { get; set; }

    // Navigation Properties
    public virtual Form Form { get; set; } = null!;
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
