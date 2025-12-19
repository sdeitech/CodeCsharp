namespace App.Application.Dto.DynamicQuestionnaire;

/// <summary>
/// Response DTO for export operations
/// </summary>
public class ExportResponseDto
{
    /// <summary>
    /// Unique identifier for the export operation
    /// </summary>
    public string ExportId { get; set; } = string.Empty;

    /// <summary>
    /// Form ID that was exported
    /// </summary>
    public int FormId { get; set; }

    /// <summary>
    /// Form title
    /// </summary>
    public string FormTitle { get; set; } = string.Empty;

    /// <summary>
    /// Export format used
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// File name for the export
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File content type for download
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Number of records exported
    /// </summary>
    public int RecordCount { get; set; }

    /// <summary>
    /// When the export was generated
    /// </summary>
    public DateTime GeneratedAt { get; set; }

    /// <summary>
    /// Download URL for the file
    /// </summary>
    public string DownloadUrl { get; set; } = string.Empty;

    /// <summary>
    /// File will be available until this date
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Export summary statistics
    /// </summary>
    public ExportSummaryDto? Summary { get; set; }
}

/// <summary>
/// Summary statistics for exported data
/// </summary>
public class ExportSummaryDto
{
    /// <summary>
    /// Total number of submissions exported
    /// </summary>
    public int TotalSubmissions { get; set; }

    /// <summary>
    /// Date range of exported submissions
    /// </summary>
    public DateTime? EarliestSubmission { get; set; }

    /// <summary>
    /// Latest submission date
    /// </summary>
    public DateTime? LatestSubmission { get; set; }

    /// <summary>
    /// Number of unique respondents
    /// </summary>
    public int UniqueRespondents { get; set; }

    /// <summary>
    /// Average score (if scoring included)
    /// </summary>
    public decimal? AverageScore { get; set; }

    /// <summary>
    /// Highest score achieved
    /// </summary>
    public decimal? HighestScore { get; set; }

    /// <summary>
    /// Lowest score achieved
    /// </summary>
    public decimal? LowestScore { get; set; }
}
