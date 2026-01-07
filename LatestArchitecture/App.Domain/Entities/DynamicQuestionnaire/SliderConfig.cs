using App.Domain.Entities;

namespace App.Domain.Entities.DynamicQuestionnaire;

public class SliderConfig : BaseEntity
{
    public int QuestionId { get; set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public int StepValue { get; set; }
    public string? MinLabel { get; set; }
    public string? MaxLabel { get; set; }
    public int OrganizationId { get; set; }

    // Navigation Properties
    public virtual Question Question { get; set; } = null!;
}
