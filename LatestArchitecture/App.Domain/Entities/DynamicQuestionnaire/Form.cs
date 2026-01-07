namespace App.Domain.Entities.DynamicQuestionnaire;

public class Form : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public bool IsPublished { get; set; } = false;
    public bool AllowResubmission { get; set; } = false;
    public DateTime? PublishedDate { get; set; }
    public string? PublicKey { get; set; }
    public string? PublicURL { get; set; }
    public int OrganizationId { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Navigation Properties
    public virtual ICollection<Page> Pages { get; set; } = new List<Page>();
    public virtual ICollection<Rule> Rules { get; set; } = new List<Rule>();
}
