namespace App.Application.Dto.DynamicQuestionnaire;

/// <summary>
/// Request DTO for updating form scoring configuration
/// </summary>
public class UpdateFormScoringRequest
{
    /// <summary>
    /// The form ID to update scoring for
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// List of option score updates
    /// </summary>
    public List<OptionScoreUpdate> OptionScores { get; set; } = new();

    /// <summary>
    /// List of matrix column score updates
    /// </summary>
    public List<MatrixColumnScoreUpdate> MatrixColumnScores { get; set; } = new();
}

/// <summary>
/// Option score update information
/// </summary>
public class OptionScoreUpdate
{
    /// <summary>
    /// Option ID to update score for
    /// </summary>
    public int OptionId { get; set; }

    /// <summary>
    /// New score value
    /// </summary>
    public decimal Score { get; set; }
}

/// <summary>
/// Matrix column score update information
/// </summary>
public class MatrixColumnScoreUpdate
{
    /// <summary>
    /// Matrix column ID to update score for
    /// </summary>
    public int MatrixColumnId { get; set; }

    /// <summary>
    /// New score value
    /// </summary>
    public decimal Score { get; set; }
}

/// <summary>
/// Scoring analytics response DTO
/// </summary>
public class ScoringAnalyticsDto
{
    /// <summary>
    /// Form ID
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// Form title
    /// </summary>
    public string FormTitle { get; set; } = string.Empty;

    /// <summary>
    /// Total number of submissions
    /// </summary>
    public int TotalSubmissions { get; set; }

    /// <summary>
    /// Average score across all submissions
    /// </summary>
    public decimal AverageScore { get; set; }

    /// <summary>
    /// Highest score achieved
    /// </summary>
    public decimal HighestScore { get; set; }

    /// <summary>
    /// Lowest score achieved
    /// </summary>
    public decimal LowestScore { get; set; }

    /// <summary>
    /// Median score
    /// </summary>
    public decimal MedianScore { get; set; }

    /// <summary>
    /// Maximum possible score for this form
    /// </summary>
    public decimal MaxPossibleScore { get; set; }

    /// <summary>
    /// Minimum possible score for this form
    /// </summary>
    public decimal MinPossibleScore { get; set; }

    /// <summary>
    /// Standard deviation of scores
    /// </summary>
    public decimal StandardDeviation { get; set; }

    /// <summary>
    /// Score ranges and their counts
    /// </summary>
    public List<ScoreRangeDto> ScoreRanges { get; set; } = new();

    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Score range information
/// </summary>
public class ScoreRangeDto
{
    /// <summary>
    /// Range label (e.g., "0-20", "21-40")
    /// </summary>
    public string RangeLabel { get; set; } = string.Empty;

    /// <summary>
    /// Minimum score in range
    /// </summary>
    public decimal MinScore { get; set; }

    /// <summary>
    /// Maximum score in range
    /// </summary>
    public decimal MaxScore { get; set; }

    /// <summary>
    /// Number of submissions in this range
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Percentage of total submissions
    /// </summary>
    public decimal Percentage { get; set; }
}

/// <summary>
/// Detailed score breakdown for a submission
/// </summary>
public class SubmissionScoreBreakdownDto
{
    /// <summary>
    /// Submission ID
    /// </summary>
    public int SubmissionId { get; set; }

    /// <summary>
    /// Respondent email
    /// </summary>
    public string RespondentEmail { get; set; } = string.Empty;

    /// <summary>
    /// Total score achieved
    /// </summary>
    public decimal TotalScore { get; set; }

    /// <summary>
    /// Maximum possible score
    /// </summary>
    public decimal MaxPossibleScore { get; set; }

    /// <summary>
    /// Score percentage
    /// </summary>
    public decimal ScorePercentage { get; set; }

    /// <summary>
    /// Breakdown by question
    /// </summary>
    public List<QuestionScoreDto> QuestionScores { get; set; } = new();

    /// <summary>
    /// Submission date
    /// </summary>
    public DateTime SubmittedAt { get; set; }
}

/// <summary>
/// Question-level score information
/// </summary>
public class QuestionScoreDto
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
    /// Question type
    /// </summary>
    public string QuestionType { get; set; } = string.Empty;

    /// <summary>
    /// Score earned for this question
    /// </summary>
    public decimal ScoreEarned { get; set; }

    /// <summary>
    /// Maximum possible score for this question
    /// </summary>
    public decimal MaxPossibleScore { get; set; }

    /// <summary>
    /// Selected options with their scores (for choice-based questions)
    /// </summary>
    public List<SelectedOptionScoreDto> SelectedOptions { get; set; } = new();

    /// <summary>
    /// Matrix answers with scores (for matrix questions)
    /// </summary>
    public List<MatrixAnswerScoreDto> MatrixAnswers { get; set; } = new();
}

/// <summary>
/// Selected option score information
/// </summary>
public class SelectedOptionScoreDto
{
    /// <summary>
    /// Option ID
    /// </summary>
    public int OptionId { get; set; }

    /// <summary>
    /// Option text
    /// </summary>
    public string OptionText { get; set; } = string.Empty;

    /// <summary>
    /// Score for this option
    /// </summary>
    public decimal Score { get; set; }
}

/// <summary>
/// Matrix answer score information
/// </summary>
public class MatrixAnswerScoreDto
{
    /// <summary>
    /// Matrix row ID
    /// </summary>
    public int MatrixRowId { get; set; }

    /// <summary>
    /// Row label
    /// </summary>
    public string RowLabel { get; set; } = string.Empty;

    /// <summary>
    /// Selected column ID
    /// </summary>
    public int SelectedColumnId { get; set; }

    /// <summary>
    /// Column label
    /// </summary>
    public string ColumnLabel { get; set; } = string.Empty;

    /// <summary>
    /// Score for this selection
    /// </summary>
    public decimal Score { get; set; }
}

/// <summary>
/// Score distribution response DTO
/// </summary>
public class ScoreDistributionDto
{
    /// <summary>
    /// Form ID
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// Distribution data points
    /// </summary>
    public List<ScoreDistributionPoint> DistributionPoints { get; set; } = new();

    /// <summary>
    /// Statistical summary
    /// </summary>
    public ScoreStatistics Statistics { get; set; } = new();
}

/// <summary>
/// Score distribution point
/// </summary>
public class ScoreDistributionPoint
{
    /// <summary>
    /// Score value
    /// </summary>
    public decimal Score { get; set; }

    /// <summary>
    /// Number of submissions with this score
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Cumulative percentage up to this score
    /// </summary>
    public decimal CumulativePercentage { get; set; }
}

/// <summary>
/// Score statistics
/// </summary>
public class ScoreStatistics
{
    /// <summary>
    /// Mean score
    /// </summary>
    public decimal Mean { get; set; }

    /// <summary>
    /// Median score
    /// </summary>
    public decimal Median { get; set; }

    /// <summary>
    /// Mode (most frequent score)
    /// </summary>
    public decimal Mode { get; set; }

    /// <summary>
    /// Standard deviation
    /// </summary>
    public decimal StandardDeviation { get; set; }

    /// <summary>
    /// Variance
    /// </summary>
    public decimal Variance { get; set; }

    /// <summary>
    /// Skewness (measure of asymmetry)
    /// </summary>
    public decimal Skewness { get; set; }

    /// <summary>
    /// Kurtosis (measure of tail heaviness)
    /// </summary>
    public decimal Kurtosis { get; set; }
}

/// <summary>
/// Top performer information
/// </summary>
public class TopPerformerDto
{
    /// <summary>
    /// Submission ID
    /// </summary>
    public int SubmissionId { get; set; }

    /// <summary>
    /// Respondent email
    /// </summary>
    public string RespondentEmail { get; set; } = string.Empty;

    /// <summary>
    /// Total score achieved
    /// </summary>
    public decimal TotalScore { get; set; }

    /// <summary>
    /// Score percentage
    /// </summary>
    public decimal ScorePercentage { get; set; }

    /// <summary>
    /// Rank position
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// Submission date
    /// </summary>
    public DateTime SubmittedAt { get; set; }

    /// <summary>
    /// Time taken to complete (if tracked)
    /// </summary>
    public TimeSpan? TimeTaken { get; set; }
}
