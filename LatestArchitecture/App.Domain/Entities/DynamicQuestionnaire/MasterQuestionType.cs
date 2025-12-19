namespace App.Domain.Entities.DynamicQuestionnaire;

public class MasterQuestionType 
{
    public int Id { get; private set; }
    public string TypeName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
