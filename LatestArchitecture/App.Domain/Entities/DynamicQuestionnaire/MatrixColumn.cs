using App.Domain.Entities;

namespace App.Domain.Entities.DynamicQuestionnaire;

public class MatrixColumn : BaseEntity
{
    public int QuestionId { get; set; }
    public string ColumnLabel { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public decimal Score { get; set; } = 0;
    public int OrganizationId { get; set; }

    // Navigation Properties
    public virtual Question Question { get; set; } = null!;
}
