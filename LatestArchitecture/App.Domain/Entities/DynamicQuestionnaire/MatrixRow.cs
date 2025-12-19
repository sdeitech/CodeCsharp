using App.Domain.Entities;

namespace App.Domain.Entities.DynamicQuestionnaire;

public class MatrixRow : BaseEntity
{
    public int QuestionId { get; set; }
    public string RowLabel { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public int OrganizationId { get; set; }

    // Navigation Properties
    public virtual Question Question { get; set; } = null!;
}
