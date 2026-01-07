using App.Domain.Entities;

namespace App.Domain.Entities.DynamicQuestionnaire;

public class Question : BaseEntity
{
    public int PageId { get; set; }
    public int QuestionTypeId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = false;
    public int QuestionOrder { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int OrganizationId { get; set; }

    // Navigation Properties
    public virtual Page Page { get; set; } = null!;
    public virtual MasterQuestionType QuestionType { get; set; } = null!;
    public virtual ICollection<Option> Options { get; set; } = new List<Option>();
    public virtual SliderConfig? SliderConfig { get; set; }
    public virtual ICollection<MatrixRow> MatrixRows { get; set; } = new List<MatrixRow>();
    public virtual ICollection<MatrixColumn> MatrixColumns { get; set; } = new List<MatrixColumn>();
}
