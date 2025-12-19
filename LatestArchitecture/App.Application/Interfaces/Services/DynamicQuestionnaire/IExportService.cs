using App.Application.Dto.DynamicQuestionnaire;
using App.Common.Models;

namespace App.Application.Interfaces.Services.DynamicQuestionnaire;

/// <summary>
/// Service interface for enhanced export functionality
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Generate export file for form data
    /// </summary>
    /// <param name="request">Export configuration</param>
    /// <param name="userId">User performing the export</param>
    /// <returns>Export response with file details</returns>
    Task<JsonModel> GenerateExportAsync(ExportRequestDto request, int userId);

    /// <summary>
    /// Download previously generated export file
    /// </summary>
    /// <param name="exportId">Export identifier</param>
    /// <returns>File stream and metadata</returns>
    Task<JsonModel> DownloadExportAsync(string exportId);

    /// <summary>
    /// Get list of available exports for a form
    /// </summary>
    /// <param name="formId">Form ID</param>
    /// <param name="userId">User requesting the list</param>
    /// <returns>List of available exports</returns>
    Task<JsonModel> GetFormExportsAsync(int formId, int userId);

    /// <summary>
    /// Delete an export file
    /// </summary>
    /// <param name="exportId">Export identifier</param>
    /// <param name="userId">User performing the deletion</param>
    /// <returns>Deletion result</returns>
    Task<JsonModel> DeleteExportAsync(string exportId, int userId);

    /// <summary>
    /// Generate CSV content from form responses
    /// </summary>
    /// <param name="formId">Form ID</param>
    /// <param name="request">Export parameters</param>
    /// <returns>CSV content as byte array</returns>
    Task<byte[]> GenerateCsvAsync(int formId, ExportRequestDto request);

    /// <summary>
    /// Generate Excel content from form responses
    /// </summary>
    /// <param name="formId">Form ID</param>
    /// <param name="request">Export parameters</param>
    /// <returns>Excel content as byte array</returns>
    Task<byte[]> GenerateExcelAsync(int formId, ExportRequestDto request);

    /// <summary>
    /// Generate PDF report from form responses
    /// </summary>
    /// <param name="formId">Form ID</param>
    /// <param name="request">Export parameters</param>
    /// <returns>PDF content as byte array</returns>
    Task<byte[]> GeneratePdfAsync(int formId, ExportRequestDto request);

    /// <summary>
    /// Clean up expired export files
    /// </summary>
    /// <returns>Number of files cleaned up</returns>
    int CleanupExpiredExportsAsync();
}
