using App.Domain.Entities;

namespace App.Domain.Entities.DynamicQuestionnaire;

public class Submission : BaseEntity
{
    public int FormId { get; set; }
    public string RespondentEmail { get; set; } = string.Empty;
    public string? RespondentName { get; set; }
    public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
    public decimal TotalScore { get; set; } = 0;
    public int OrganizationId { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation Properties
    public virtual Form Form { get; set; } = null!;
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
