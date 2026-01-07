using System.ComponentModel.DataAnnotations;

namespace App.Application.Dto.DynamicQuestionnaire;

/// <summary>
/// Request DTO for exporting form data
/// </summary>
public class ExportRequestDto
{
    /// <summary>
    /// Form ID to export data for
    /// </summary>
    [Required]
    public int FormId { get; set; }

    /// <summary>
    /// Export format (CSV, Excel, PDF)
    /// </summary>
    [Required]
    public string Format { get; set; } = "CSV";

    /// <summary>
    /// What type of data to export
    /// </summary>
    public ExportDataType DataType { get; set; } = ExportDataType.Responses;

    /// <summary>
    /// Include detailed score breakdown
    /// </summary>
    public bool IncludeScoring { get; set; } = true;

    /// <summary>
    /// Include question text in export
    /// </summary>
    public bool IncludeQuestionText { get; set; } = true;

    /// <summary>
    /// Date range filter for submissions
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    /// Date range filter for submissions
    /// </summary>
    public DateTime? DateTo { get; set; }

    /// <summary>
    /// Filter by respondent email
    /// </summary>
    public string? RespondentEmailFilter { get; set; }

    /// <summary>
    /// Include analytics summary
    /// </summary>
    public bool IncludeAnalytics { get; set; } = false;
}

/// <summary>
/// Types of data that can be exported
/// </summary>
public enum ExportDataType
{
    Responses,
    Analytics,
    FormStructure,
    Complete
}
