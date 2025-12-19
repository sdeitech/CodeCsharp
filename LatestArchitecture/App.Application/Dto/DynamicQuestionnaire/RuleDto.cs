namespace App.Application.Dto.DynamicQuestionnaire;

/// <summary>
/// Request DTO for creating a new conditional logic rule
/// </summary>
public class CreateRuleRequest
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
}

/// <summary>
/// Request DTO for updating an existing conditional logic rule
/// </summary>
public class UpdateRuleRequest
{
    /// <summary>
    /// The rule ID to update
    /// </summary>
    public int RuleId { get; set; }

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
}

/// <summary>
/// Response DTO for returning rule information
/// </summary>
public class RuleResponseDto
{
    /// <summary>
    /// Unique identifier for the rule
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// The form this rule belongs to
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// The question that triggers this rule
    /// </summary>
    public int SourceQuestionId { get; set; }

    /// <summary>
    /// Source question text
    /// </summary>
    public string SourceQuestionText { get; set; } = string.Empty;

    /// <summary>
    /// The specific option that triggers the rule (for choice-based questions). NULL for sliders.
    /// </summary>
    public int? TriggerOptionId { get; set; }

    /// <summary>
    /// Trigger option text if applicable
    /// </summary>
    public string? TriggerOptionText { get; set; }

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
    /// Target question text if applicable
    /// </summary>
    public string? TargetQuestionText { get; set; }

    /// <summary>
    /// The target page to skip to (for 'SkipToPage' action)
    /// </summary>
    public int? TargetPageId { get; set; }

    /// <summary>
    /// Target page title if applicable
    /// </summary>
    public string? TargetPageTitle { get; set; }

    /// <summary>
    /// Matrix row ID for matrix-specific conditions (NULL for non-matrix questions)
    /// </summary>
    public int? MatrixRowId { get; set; }

    /// <summary>
    /// Matrix row label if applicable
    /// </summary>
    public string? MatrixRowLabel { get; set; }

    /// <summary>
    /// Matrix column ID for matrix-specific conditions (NULL for non-matrix questions)
    /// </summary>
    public int? MatrixColumnId { get; set; }

    /// <summary>
    /// Matrix column label if applicable
    /// </summary>
    public string? MatrixColumnLabel { get; set; }

    /// <summary>
    /// Score value for matrix score-based conditions (NULL for non-score conditions)
    /// </summary>
    public decimal? ScoreValue { get; set; }

    /// <summary>
    /// When this rule was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When this rule was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
