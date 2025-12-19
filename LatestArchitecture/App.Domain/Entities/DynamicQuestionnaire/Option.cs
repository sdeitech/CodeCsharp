using App.Domain.Entities;

namespace App.Domain.Entities.DynamicQuestionnaire;

public class Option : BaseEntity
{
    public int QuestionId { get; set; }
    public string? OptionText { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public decimal Score { get; set; } = 0;
    public bool IsDeleted { get; set; } = false;
    public int OrganizationId { get; set; }

    // Navigation Properties
    public virtual Question Question { get; set; } = null!;
}
