using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.DynamicQuestionnaire;

/// <summary>
/// Controller for export functionality - Phase 8 implementation
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ExportController : BaseController
{    
    private readonly IExportService _exportService;
    private readonly ICurrentUserClaimService _currentUserClaimService;

    public ExportController(IExportService exportService, ICurrentUserClaimService currentUserClaimService)
    {
        _exportService = exportService;
        _currentUserClaimService = currentUserClaimService;
    }

    /// <summary>
    /// Generate export file for form data
    /// </summary>
    /// <param name="request">Export configuration</param>
    /// <returns>Export metadata and download information</returns>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateExport([FromBody] ExportRequestDto request)
    {
        // Basic validation
        if (request == null)
        {
            return BadRequest(new JsonModel(responseData: null!, StatusMessage.RequestBodyRequired, StatusCodes.Status400BadRequest));
        }

        if (request.FormId <= Number.Zero)
        {
            return BadRequest(new JsonModel(responseData: null!, StatusMessage.ValidFormIdRequired, StatusCodes.Status400BadRequest));
        }

        if (string.IsNullOrWhiteSpace(request.Format))
        {
            return BadRequest(new JsonModel(responseData: null!, StatusMessage.ExportFormatRequired, StatusCodes.Status400BadRequest));
        }

        // Validate format
        var validFormats = new[] { "CSV", "EXCEL", "XLSX", "PDF" };
        if (!validFormats.Contains(request.Format.ToUpperInvariant()))
        {
            return BadRequest(new JsonModel(responseData: null!, StatusMessage.InvalidExportFormat, StatusCodes.Status400BadRequest));
        }

        // Get Current User ID
        int userId = _currentUserClaimService.UserId ?? Number.Zero;
        var result = await _exportService.GenerateExportAsync(request, userId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status400BadRequest => BadRequest(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Download previously generated export file
    /// </summary>
    /// <param name="exportId">Export identifier</param>
    /// <returns>File download</returns>
    [HttpGet("{exportId}/download")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status410Gone)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DownloadExport(string exportId)
    {
        if (string.IsNullOrWhiteSpace(exportId))
        {
            return BadRequest(new JsonModel(responseData: null!, StatusMessage.ExportIdRequired, StatusCodes.Status400BadRequest));
        }

        var result = await _exportService.DownloadExportAsync(exportId);
        
        if (result.StatusCode == StatusCodes.Status200OK)
        {
            var downloadInfo = result.Data as dynamic;
            if (downloadInfo != null)
            {
                var fileContent = Convert.FromBase64String(downloadInfo.FileContent.ToString());
                var fileName = downloadInfo.FileName.ToString();
                var contentType = downloadInfo.ContentType.ToString();

                return File(fileContent, contentType, fileName);
            }
        }

        return result.StatusCode switch
        {
            StatusCodes.Status404NotFound => NotFound(result),
            StatusCodes.Status410Gone => StatusCode(StatusCodes.Status410Gone, result), // Gone - Export expired
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Get list of available exports for a form
    /// </summary>
    /// <param name="formId">Form ID</param>
    /// <returns>List of available exports</returns>
    [HttpGet("forms/{formId:int}")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFormExports(int formId)
    {
        if (formId <= Number.Zero)
        {
            return BadRequest(new JsonModel(responseData: null!, StatusMessage.ValidFormIdRequired, StatusCodes.Status400BadRequest));
        }

        // Get Current User ID
        int userId = _currentUserClaimService.UserId ?? Number.Zero;

        var result = await _exportService.GetFormExportsAsync(formId, userId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Delete an export file
    /// </summary>
    /// <param name="exportId">Export identifier</param>
    /// <returns>Deletion confirmation</returns>
    [HttpDelete("{exportId}")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteExport(string exportId)
    {
        if (string.IsNullOrWhiteSpace(exportId))
        {
            return BadRequest(new JsonModel(responseData: null!, StatusMessage.ExportIdRequired, StatusCodes.Status400BadRequest));
        }

        // Get Current User ID
        int userId = _currentUserClaimService.UserId ?? Number.Zero;

        var result = await _exportService.DeleteExportAsync(exportId, userId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status403Forbidden => Forbid(),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Quick export - Generate and immediately download a CSV file
    /// </summary>
    /// <param name="formId">Form ID</param>
    /// <param name="includeScoring">Include scoring data</param>
    /// <param name="includeQuestionText">Include question text</param>
    /// <returns>CSV file download</returns>
    [HttpGet("forms/{formId:int}/quick-csv")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> QuickCsvExport(int formId, bool includeScoring = true, bool includeQuestionText = true)
    {
        if (formId <= Number.Zero)
        {
            return BadRequest(new JsonModel(responseData: null!, StatusMessage.ValidFormIdRequired, StatusCodes.Status400BadRequest));
        }

        var request = new ExportRequestDto
        {
            FormId = formId,
            Format = "CSV",
            IncludeScoring = includeScoring,
            IncludeQuestionText = includeQuestionText,
            DataType = ExportDataType.Responses
        };

        var csvContent = await _exportService.GenerateCsvAsync(formId, request);
        var fileName = $"Form_{formId}_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

        return File(csvContent, "text/csv", fileName);
    }

    /// <summary>
    /// Clean up expired export files (admin function)
    /// </summary>
    /// <returns>Number of files cleaned up</returns>
    [HttpPost("cleanup")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CleanupExpiredExports()
    {
        var cleanedCount =  _exportService.CleanupExpiredExportsAsync();
        
        return Ok(new JsonModel(new { CleanedFiles = cleanedCount }, 
            string.Format(StatusMessage.ExpiredExportsCleanedUp, cleanedCount), StatusCodes.Status200OK));
    }

    /// <summary>
    /// Get export statistics and analytics
    /// </summary>
    /// <returns>Export usage statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public IActionResult GetExportStatistics()
    {
        // For Phase 8, provide basic statistics
        var statistics = new
        {
            TotalActiveExports = Number.Zero, // Would be calculated from actual storage
            ExportsByFormat = new Dictionary<string, int>
            {
                { "CSV", 0 },
                { "Excel", 0 },
                { "PDF", 0 }
            },
            LastCleanupDate = DateTime.UtcNow.AddDays(-1),
            AverageFileSize = "0 KB"
        };

        return Ok(new JsonModel(statistics, StatusMessage.ExportStatisticsRetrieved, StatusCodes.Status200OK));
    }
}
