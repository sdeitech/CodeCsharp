using App.Domain.Entities;

namespace App.Domain.Entities.DynamicQuestionnaire;

public class AnswerValue : BaseEntity
{
    public int AnswerId { get; set; }
    
    // For Radio, Dropdown, Multi-select answers
    public int? SelectedOptionId { get; set; }
    
    // For Matrix answers (identifies the row)
    public int? MatrixRowId { get; set; }
    
    // For Matrix answers (identifies the selected column for a given row)
    public int? SelectedMatrixColumnId { get; set; }
    
    // For Text-based answers
    public string? TextValue { get; set; }
    
    // For Slider answers
    public decimal? NumericValue { get; set; }
    
    public bool IsDeleted { get; set; } = false;
    public int OrganizationId { get; set; }

    // Navigation Properties
    public virtual Answer Answer { get; set; } = null!;
    public virtual Option? SelectedOption { get; set; }
}
