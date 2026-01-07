using App.Application.Dto.Common;
using App.Application.Dto.DynamicQuestionnaire;
using App.Common.Models;

namespace App.Application.Interfaces.Services.DynamicQuestionnaire;

/// <summary>
/// Service interface for scoring engine functionality
/// </summary>
public interface IScoringEngineService
{
    /// <summary>
    /// Calculate total score for a form submission
    /// </summary>
    /// <param name="submissionId">The submission ID to calculate score for</param>
    /// <returns>Calculated total score</returns>
    Task<JsonModel> CalculateSubmissionScoreAsync(int submissionId);

    /// <summary>
    /// Recalculate scores for all submissions of a form (useful after score updates)
    /// </summary>
    /// <param name="formId">The form ID to recalculate scores for</param>
    /// <returns>Number of submissions updated</returns>
    Task<JsonModel> RecalculateFormScoresAsync(int formId);

    /// <summary>
    /// Get scoring analytics for a form
    /// </summary>
    /// <param name="formId">The form ID to get analytics for</param>
    /// <returns>Scoring analytics data</returns>
    Task<JsonModel> GetFormScoringAnalyticsAsync(int formId);

    /// <summary>
    /// Get detailed score breakdown for a specific submission
    /// </summary>
    /// <param name="submissionId">The submission ID to get breakdown for</param>
    /// <returns>Detailed score breakdown</returns>
    Task<JsonModel> GetSubmissionScoreBreakdownAsync(int submissionId);

    /// <summary>
    /// Update scoring configuration for a form's options
    /// </summary>
    /// <param name="request">Score update request</param>
    /// <param name="userId">User performing the update</param>
    /// <returns>Update result</returns>
    Task<JsonModel> UpdateFormScoringAsync(UpdateFormScoringRequest request, int userId);

    /// <summary>
    /// Get score distribution for a form
    /// </summary>
    /// <param name="formId">The form ID to get distribution for</param>
    /// <returns>Score distribution data</returns>
    Task<JsonModel> GetScoreDistributionAsync(int formId);

    /// <summary>
    /// Get top performers for a form
    /// </summary>
    /// <param name="formId">The form ID to get top performers for</param>
    /// <param name="topCount">Number of top performers to return</param>
    /// <returns>Top performers list</returns>
    Task<JsonModel> GetTopPerformersAsync(int formId, int topCount = 10);

    /// <summary>
    /// Export scoring report to various formats
    /// </summary>
    /// <param name="formId">The form ID to export scores for</param>
    /// <param name="format">Export format (CSV, Excel, PDF)</param>
    /// <returns>Export file data</returns>
    Task<JsonModel> ExportScoringReportAsync(int formId, string format = "CSV");
}
