namespace App.Application.Dto.DynamicQuestionnaire;

/// <summary>
/// DTO for evaluating rules when rendering a form to determine which questions to show/hide
/// </summary>
public class RuleEvaluationRequest
{
    /// <summary>
    /// The form ID being evaluated
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// Current responses for rule evaluation
    /// </summary>
    public List<RuleEvaluationAnswer> Answers { get; set; } = new();
}

/// <summary>
/// Answer for rule evaluation
/// </summary>
public class RuleEvaluationAnswer
{
    /// <summary>
    /// Question ID
    /// </summary>
    public int QuestionId { get; set; }

    /// <summary>
    /// Selected option IDs (for choice-based questions)
    /// </summary>
    public List<int> SelectedOptionIds { get; set; } = new();

    /// <summary>
    /// Numeric value (for slider questions)
    /// </summary>
    public decimal? NumericValue { get; set; }

    /// <summary>
    /// Text value (for text-based questions)
    /// </summary>
    public string? TextValue { get; set; }

    /// <summary>
    /// Matrix answers (for matrix questions)
    /// </summary>
    public List<MatrixAnswer> MatrixAnswers { get; set; } = new();
}

/// <summary>
/// Matrix answer for rule evaluation
/// </summary>
public class MatrixAnswer
{
    /// <summary>
    /// Matrix row ID
    /// </summary>
    public int MatrixRowId { get; set; }

    /// <summary>
    /// Selected column ID
    /// </summary>
    public int SelectedColumnId { get; set; }
}

/// <summary>
/// Result of rule evaluation
/// </summary>
public class RuleEvaluationResult
{
    /// <summary>
    /// Questions that should be hidden
    /// </summary>
    public List<int> HiddenQuestionIds { get; set; } = new();

    /// <summary>
    /// Questions that should be shown (if previously hidden)
    /// </summary>
    public List<int> VisibleQuestionIds { get; set; } = new();

    /// <summary>
    /// Page to skip to (if any)
    /// </summary>
    public int? SkipToPageId { get; set; }

    /// <summary>
    /// Whether the form should be terminated
    /// </summary>
    public bool TerminateForm { get; set; }

    /// <summary>
    /// List of triggered rules for debugging
    /// </summary>
    public List<int> TriggeredRuleIds { get; set; } = new();
}
