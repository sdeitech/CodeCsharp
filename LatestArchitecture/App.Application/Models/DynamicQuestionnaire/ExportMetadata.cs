namespace App.Application.Models.DynamicQuestionnaire;

/// <summary>
/// Internal metadata class for storing export information
/// </summary>
public sealed class ExportMetadata
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
    /// User ID who requested the export
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Binary content of the generated file
    /// </summary>
    public byte[] FileContent { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// File name for the export
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File content type for download
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// When the export was generated
    /// </summary>
    public DateTime GeneratedAt { get; set; }

    /// <summary>
    /// File will be available until this date
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Number of records exported
    /// </summary>
    public int RecordCount { get; set; }
}
