using App.Domain.Entities;

namespace App.Domain.Entities.DynamicQuestionnaire;

public class Answer : BaseEntity
{
    public int SubmissionId { get; set; }
    public int QuestionId { get; set; }
    public decimal Score { get; set; } = 0;
    public bool IsDeleted { get; set; } = false;
    public int OrganizationId { get; set; }

    // Navigation Properties
    public virtual Submission Submission { get; set; } = null!;
    public virtual Question Question { get; set; } = null!;
    public virtual ICollection<AnswerValue> AnswerValues { get; set; } = new List<AnswerValue>();
}
