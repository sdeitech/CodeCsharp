using App.Domain.Entities;

namespace App.Domain.Entities.DynamicQuestionnaire;

/// <summary>
/// Represents a conditional logic rule for a form that defines when to show/hide questions or navigate to different pages
/// </summary>
public class Rule : BaseEntity
{
    /// <summary>
    /// The form this rule belongs to
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// The question that triggers this rule
    /// </summary>
    public int SourceQuestionId { get; set; }

    /// <summary>
    /// The specific option that triggers the rule (for choice-based questions). NULL for sliders.
    /// </summary>
    public int? TriggerOptionId { get; set; }

    /// <summary>
    /// The condition type: 'IsSelected', 'IsNotSelected', 'IsGreaterThan', 'IsLessThan', 'IsEqualTo', 'IsNotEqualTo', 'IsInRange'
    /// </summary>
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// Minimum value for range-based conditions (sliders)
    /// </summary>
    public decimal? MinValue { get; set; }

    /// <summary>
    /// Maximum value for range-based conditions (sliders)
    /// </summary>
    public decimal? MaxValue { get; set; }

    /// <summary>
    /// The action to perform: 'HideQuestion', 'ShowQuestion', 'SkipToPage', 'TerminateForm'
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// The target question to hide/show (for 'HideQuestion'/'ShowQuestion' actions)
    /// </summary>
    public int? TargetQuestionId { get; set; }

    /// <summary>
    /// The target page to skip to (for 'SkipToPage' action)
    /// </summary>
    public int? TargetPageId { get; set; }

    /// <summary>
    /// Matrix row ID for matrix-specific conditions (NULL for non-matrix questions)
    /// </summary>
    public int? MatrixRowId { get; set; }

    /// <summary>
    /// Matrix column ID for matrix-specific conditions (NULL for non-matrix questions)
    /// </summary>
    public int? MatrixColumnId { get; set; }

    /// <summary>
    /// Score value for matrix score-based conditions (NULL for non-score conditions)
    /// </summary>
    public decimal? ScoreValue { get; set; }

    /// <summary>
    /// Indicates if the rule is soft deleted
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Organization ID for multi-tenant data isolation
    /// </summary>
    public int OrganizationId { get; set; }

    // Navigation Properties
    public virtual Form Form { get; set; } = null!;
    public virtual Question SourceQuestion { get; set; } = null!;
    public virtual Option? TriggerOption { get; set; }
    public virtual Question? TargetQuestion { get; set; }
    public virtual Page? TargetPage { get; set; }
    public virtual MatrixRow? MatrixRow { get; set; }
    public virtual MatrixColumn? MatrixColumn { get; set; }
}
