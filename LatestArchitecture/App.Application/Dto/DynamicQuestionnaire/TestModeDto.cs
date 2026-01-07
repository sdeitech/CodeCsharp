namespace App.Application.Dto.DynamicQuestionnaire;

/// <summary>
/// Request for testing form submission with rule evaluation and scoring
/// </summary>
public class TestModeSubmissionRequest
{
    /// <summary>
    /// The form ID being tested
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// Current answers for rule evaluation and scoring
    /// </summary>
    public List<TestModeAnswer> Answers { get; set; } = new();

    /// <summary>
    /// Whether to calculate scores for the submission
    /// </summary>
    public bool CalculateScore { get; set; } = true;

    /// <summary>
    /// Whether to evaluate rules for the submission
    /// </summary>
    public bool EvaluateRules { get; set; } = true;
}

/// <summary>
/// Answer for test mode submission
/// </summary>
public class TestModeAnswer
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
    /// Date value (for date questions)
    /// </summary>
    public DateTime? DateValue { get; set; }

    /// <summary>
    /// Matrix answers (for matrix questions)
    /// </summary>
    public List<TestModeMatrixAnswer> MatrixAnswers { get; set; } = new();
}

/// <summary>
/// Matrix answer for test mode submission
/// </summary>
public class TestModeMatrixAnswer
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
/// Result of test mode submission
/// </summary>
public class TestModeSubmissionResult
{
    /// <summary>
    /// Form ID that was tested
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// Test submission timestamp
    /// </summary>
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Rule evaluation results
    /// </summary>
    public RuleEvaluationResult? RuleEvaluation { get; set; }

    /// <summary>
    /// Score calculation results
    /// </summary>
    public TestModeScoreResult? ScoreResult { get; set; }

    /// <summary>
    /// Whether the test submission was successful
    /// </summary>
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// Any validation errors encountered
    /// </summary>
    public List<string> ValidationErrors { get; set; } = new();
}

/// <summary>
/// Score calculation result for test mode
/// </summary>
public class TestModeScoreResult
{
    /// <summary>
    /// Total calculated score
    /// </summary>
    public decimal TotalScore { get; set; }

    /// <summary>
    /// Maximum possible score for the form
    /// </summary>
    public decimal MaxPossibleScore { get; set; }

    /// <summary>
    /// Score percentage
    /// </summary>
    public decimal ScorePercentage { get; set; }

    /// <summary>
    /// Breakdown of scores by question
    /// </summary>
    public List<TestModeQuestionScore> QuestionScores { get; set; } = new();
}

/// <summary>
/// Score breakdown for individual questions
/// </summary>
public class TestModeQuestionScore
{
    /// <summary>
    /// Question ID
    /// </summary>
    public int QuestionId { get; set; }

    /// <summary>
    /// Question text
    /// </summary>
    public string QuestionText { get; set; } = string.Empty;

    /// <summary>
    /// Score earned for this question
    /// </summary>
    public decimal Score { get; set; }

    /// <summary>
    /// Maximum possible score for this question
    /// </summary>
    public decimal MaxScore { get; set; }

    /// <summary>
    /// Selected answers with their scores
    /// </summary>
    public List<TestModeAnswerScore> AnswerScores { get; set; } = new();
}

/// <summary>
/// Score for individual answer/option
/// </summary>
public class TestModeAnswerScore
{
    /// <summary>
    /// Option ID (if applicable)
    /// </summary>
    public int? OptionId { get; set; }

    /// <summary>
    /// Answer text
    /// </summary>
    public string AnswerText { get; set; } = string.Empty;

    /// <summary>
    /// Score for this answer
    /// </summary>
    public decimal Score { get; set; }
}
