using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Application.Models.DynamicQuestionnaire;
using App.Common.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Globalization;
using App.Common.Constant;
using Microsoft.AspNetCore.Http;
using App.Application.Interfaces.Services.AuthenticationModule;

namespace App.Application.Service.DynamicQuestionnaire;

/// <summary>
/// Enhanced export service implementation for Phase 8
/// </summary>
public class ExportService : IExportService
{
    #region Constants and Configuration

    private const string ExportIdPrefix = "EXP";
    private const string CsvContentType = "text/csv";
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private const string PdfContentType = "application/pdf";
    private const string CsvExtension = "csv";
    private const string ExcelExtension = "xlsx";
    private const string PdfExtension = "pdf";
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    private const string FileDateTimeFormat = "yyyyMMdd_HHmmss";
    private const int DefaultExportExpirationDays = 7;
    private const int MaxPdfSubmissionLimit = 100;
    private const string DefaultFileName = "Export";
    private const string ExportEndpointTemplate = "/api/v1/DynamicQuestionnaire/exports/{0}/download";
    
    private static readonly Dictionary<string, (string ContentType, string Extension)> SupportedFormats = new()
    {
        { "CSV", (CsvContentType, CsvExtension) },
        { "EXCEL", (ExcelContentType, ExcelExtension) },
        { "XLSX", (ExcelContentType, ExcelExtension) },
        { "PDF", (PdfContentType, PdfExtension) }
    };

    #endregion

    #region Fields

    #endregion

    #region Fields

    private readonly IFormRepository _formRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IScoringEngineService _scoringEngineService;
    private readonly ILogger<ExportService> _logger;
    private readonly ICurrentUserClaimService _currentUserClaimService;

    // In a real application, this would be replaced with a proper file storage service
    private static readonly Dictionary<string, ExportMetadata> _exportCache = new();

    #endregion

    #region Constructor

    #endregion

    #region Constructor

    public ExportService(
        IFormRepository formRepository,
        ISubmissionRepository submissionRepository,
        IScoringEngineService scoringEngineService,
        ICurrentUserClaimService currentUserClaimService,
        ILogger<ExportService> logger)
    {
        _formRepository = formRepository ?? throw new ArgumentNullException(nameof(formRepository));
        _submissionRepository = submissionRepository ?? throw new ArgumentNullException(nameof(submissionRepository));
        _scoringEngineService = scoringEngineService ?? throw new ArgumentNullException(nameof(scoringEngineService));
        _currentUserClaimService = currentUserClaimService ?? throw new ArgumentNullException(nameof(currentUserClaimService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #endregion

    #region Public Methods

    #endregion

    #region Public Methods

    public async Task<JsonModel> GenerateExportAsync(ExportRequestDto request, int userId)
    {
        try
        {
            // Validate input parameters
            var validationResult = ValidateExportRequest(request);
            if (validationResult != null)
                return validationResult;

            _logger.LogInformation("Generating export for form {FormId} in {Format} format", request.FormId, request.Format);

            // Get and validate organization
            var organizationResult = await ValidateOrganizationAsync();
            if (organizationResult.ValidationFailed)
                return organizationResult.ErrorResponse!;

            // Validate form exists
            var form = await _formRepository.GetByIdWithDetailsAsync(request.FormId, organizationResult.OrganizationId);
            if (form == null)
            {
                return CreateErrorResponse(StatusMessage.FormNotFound, StatusCodes.Status404NotFound);
            }

            // Generate export
            var exportResult = await ProcessExportGenerationAsync(request, userId, form);
            
            _logger.LogInformation("Export generated successfully: {ExportId}, Size: {Size} bytes",
                exportResult.ExportId, exportResult.FileSizeBytes);

            return new JsonModel(exportResult, StatusMessage.ExportGeneratedSuccessfully, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating export for form {FormId}", request?.FormId);
            return CreateErrorResponse("An error occurred while generating the export", StatusCodes.Status500InternalServerError);
        }
    }

    public Task<JsonModel> DownloadExportAsync(string exportId)
    {
        if (string.IsNullOrWhiteSpace(exportId))
        {
            return Task.FromResult(CreateErrorResponse("Export ID is required", StatusCodes.Status400BadRequest));
        }

        if (!_exportCache.TryGetValue(exportId, out var metadata))
        {
            return Task.FromResult(CreateErrorResponse(StatusMessage.ExportNotFound, StatusCodes.Status404NotFound));
        }

        if (DateTime.UtcNow > metadata.ExpiresAt)
        {
            _exportCache.Remove(exportId);
            return Task.FromResult(CreateErrorResponse(StatusMessage.ErrorExportExpired, 410));
        }

        var downloadInfo = new
        {
            FileName = metadata.FileName,
            ContentType = metadata.ContentType,
            FileContent = Convert.ToBase64String(metadata.FileContent),
            FileSizeBytes = metadata.FileContent.Length
        };

        return Task.FromResult(new JsonModel(downloadInfo, StatusMessage.ExportDownload, StatusCodes.Status200OK));
    }

    public async Task<JsonModel> GetFormExportsAsync(int formId, int userId)
    {
        try
        {
            var formExists = await _formRepository.ExistsAsync(formId);
            if (!formExists)
            {
                return CreateErrorResponse(StatusMessage.FormNotFound, StatusCodes.Status404NotFound);
            }

            var userExports = GetUserExportsForForm(formId, userId);
            return new JsonModel(userExports, StatusMessage.ErrorRetrieveSuccess, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving exports for form {FormId} and user {UserId}", formId, userId);
            return CreateErrorResponse("An error occurred while retrieving exports", StatusCodes.Status500InternalServerError);
        }
    }

    public Task<JsonModel> DeleteExportAsync(string exportId, int userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(exportId))
            {
                return Task.FromResult(CreateErrorResponse("Export ID is required", StatusCodes.Status400BadRequest));
            }

            if (!_exportCache.TryGetValue(exportId, out var metadata))
            {
                return Task.FromResult(CreateErrorResponse(StatusMessage.ExportNotFound, StatusCodes.Status404NotFound));
            }

            if (metadata.UserId != userId)
            {
                return Task.FromResult(CreateErrorResponse(StatusMessage.UnauthorizedAccess, StatusCodes.Status403Forbidden));
            }

            _exportCache.Remove(exportId);
            _logger.LogInformation("Export {ExportId} deleted by user {UserId}", exportId, userId);

            return Task.FromResult(new JsonModel(new { ExportId = exportId }, StatusMessage.ExportDelete, StatusCodes.Status200OK));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting export {ExportId} for user {UserId}", exportId, userId);
            return Task.FromResult(CreateErrorResponse("An error occurred while deleting the export", StatusCodes.Status500InternalServerError));
        }
    }

    public async Task<byte[]> GenerateCsvAsync(int formId, ExportRequestDto request)
    {
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue) return Encoding.UTF8.GetBytes(string.Empty);

        var form = await _formRepository.GetByIdWithDetailsAsync(formId, organizationId.Value);
        var submissions = await GetFilteredSubmissions(formId, request);

        var csv = new StringBuilder();

        // Generate headers and data
        await GenerateCsvHeaders(form!, request, csv);
        await GenerateCsvRows(form!, submissions, request, formId, organizationId.Value, csv);

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private static Task GenerateCsvHeaders(Domain.Entities.DynamicQuestionnaire.Form form, ExportRequestDto request, StringBuilder csv)
    {
        var headers = new List<string> { "Submission ID", "Respondent Email", "Respondent Name", "Submitted Date" };

        if (request.IncludeScoring)
        {
            headers.Add("Total Score");
            headers.Add("Score Percentage");
        }

        // Add question headers
        foreach (var page in form.Pages.OrderBy(p => p.PageOrder))
        {
            foreach (var question in page.Questions.OrderBy(q => q.QuestionOrder))
            {
                var questionHeader = request.IncludeQuestionText
                    ? $"Q{question.QuestionOrder}: {question.QuestionText}"
                    : $"Question {question.QuestionOrder}";
                headers.Add(questionHeader);

                if (request.IncludeScoring)
                {
                    headers.Add($"{questionHeader} - Score");
                }
            }
        }

        csv.AppendLine(string.Join(",", headers.Select(h => $"\"{h}\"")));
        return Task.CompletedTask;
    }

    private async Task GenerateCsvRows(
        Domain.Entities.DynamicQuestionnaire.Form form,
        IEnumerable<Domain.Entities.DynamicQuestionnaire.Submission> submissions,
        ExportRequestDto request, int formId, int organizationId, StringBuilder csv)
    {
        // Avoid N+1: load all submissions with details once
        var submissionsWithDetails = await _submissionRepository.GetByFormIdWithDetailsAsync(formId, organizationId);
        var submissionsById = submissionsWithDetails.ToDictionary(s => s.Id);
        foreach (var submission in submissions)
        {
            if (!submissionsById.TryGetValue(submission.Id, out var submissionWithDetails)) continue;
            var row = await BuildCsvRow(form, submissionWithDetails, request, formId);
            csv.AppendLine(string.Join(",", row));
        }
    }

    private async Task<List<string>> BuildCsvRow(Domain.Entities.DynamicQuestionnaire.Form form,
        Domain.Entities.DynamicQuestionnaire.Submission submission, ExportRequestDto request, int formId)
    {
        var row = new List<string>
        {
            submission.Id.ToString(),
            $"\"{submission.RespondentEmail}\"",
            $"\"{submission.RespondentName}\"",
            submission.SubmittedDate.ToString(DateTimeFormat)
        };

        if (request.IncludeScoring)
        {
            var organizationId = _currentUserClaimService.OrganizationId;
            var maxScore = organizationId.HasValue ? await CalculateMaxPossibleScore(formId, organizationId.Value) : Number.Zero;
            var scorePercentage = maxScore > 0 ? (submission.TotalScore / maxScore) * 100 : 0;

            row.Add(submission.TotalScore.ToString("F2"));
            row.Add(scorePercentage.ToString("F1") + "%");
        }

        AddAnswerDataToRow(form, submission, request, row);
        return row;
    }

    private static void AddAnswerDataToRow(
        Domain.Entities.DynamicQuestionnaire.Form form,
        Domain.Entities.DynamicQuestionnaire.Submission submission,
        ExportRequestDto request,
        List<string> row)
    {
        foreach (var page in form.Pages.OrderBy(p => p.PageOrder))
        {
            foreach (var question in page.Questions.OrderBy(q => q.QuestionOrder))
            {
                var answer = submission.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                var answerText = FormatAnswerForCsv(answer, question);
                row.Add($"\"{answerText}\"");

                if (request.IncludeScoring)
                {
                    row.Add((answer?.Score ?? 0).ToString("F2"));
                }
            }
        }
    }

    public async Task<byte[]> GenerateExcelAsync(int formId, ExportRequestDto request)
    {
        // For Phase 8, we'll create a simple Excel-like format
        // In a real implementation, you would use a library like EPPlus or ClosedXML

        var csvContent = await GenerateCsvAsync(formId, request);

        // Convert CSV to tab-separated format as a simple Excel simulation
        var csvString = Encoding.UTF8.GetString(csvContent);
        var excelContent = csvString.Replace(",", "\t");

        return Encoding.UTF8.GetBytes(excelContent);
    }

    public async Task<byte[]> GeneratePdfAsync(int formId, ExportRequestDto request)
    {
        // For Phase 8, we'll create a simple PDF-like text format
        // In a real implementation, you would use a library like iTextSharp or PdfSharp

        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue) return Encoding.UTF8.GetBytes(string.Empty);

        var form = await _formRepository.GetByIdWithDetailsAsync(formId, organizationId.Value);
        var submissions = await GetFilteredSubmissions(formId, request);

        var pdf = new StringBuilder();
        pdf.AppendLine($"Form Export Report");
        pdf.AppendLine($"Form: {form!.Title}");
        pdf.AppendLine($"Generated: {DateTime.UtcNow.ToString(DateTimeFormat)}");
        pdf.AppendLine($"Total Submissions: {submissions.Count()}");
        pdf.AppendLine(new string('=', 50));
        pdf.AppendLine();

        if (request.IncludeAnalytics)
        {
            var analyticsResult = await _scoringEngineService.GetFormScoringAnalyticsAsync(formId);
            if (analyticsResult.StatusCode == StatusCodes.Status200OK)
            {
                pdf.AppendLine("ANALYTICS SUMMARY");
                pdf.AppendLine(new string('-', 20));
                pdf.AppendLine($"Analytics data would be included here...");
                pdf.AppendLine();
            }
        }

        foreach (var submission in submissions.Take(MaxPdfSubmissionLimit)) // Limit for demo
        {
            pdf.AppendLine($"Submission ID: {submission.Id}");
            pdf.AppendLine($"Respondent: {submission.RespondentEmail}");
            pdf.AppendLine($"Submitted: {submission.SubmittedDate.ToString(DateTimeFormat)}");
            if (request.IncludeScoring)
            {
                pdf.AppendLine($"Score: {submission.TotalScore:F2}");
            }
            pdf.AppendLine(new string('-', 30));
        }

        return Encoding.UTF8.GetBytes(pdf.ToString());
    }

    public int CleanupExpiredExportsAsync()
    {
        try
        {
            var expiredExports = _exportCache.Where(kvp => DateTime.UtcNow > kvp.Value.ExpiresAt).ToList();

            foreach (var expiredExport in expiredExports)
            {
                _exportCache.Remove(expiredExport.Key);
            }

            _logger.LogInformation("Cleaned up {Count} expired exports", expiredExports.Count);
            return expiredExports.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during export cleanup");
            return 0;
        }
    }

    #endregion

    #region Private Helper Methods

    private JsonModel? ValidateExportRequest(ExportRequestDto request)
    {
        if (request == null)
        {
            _logger.LogWarning("Export request is null");
            return CreateErrorResponse(StatusMessage.RequestNull, StatusCodes.Status400BadRequest);
        }

        if (!Enum.IsDefined(typeof(ExportDataType), request.DataType))
        {
            _logger.LogWarning("Invalid DataType: {DataType}", (int)request.DataType);
            return CreateErrorResponse(StatusMessage.UnsupportedDataType, StatusCodes.Status400BadRequest);
        }

        if (string.IsNullOrWhiteSpace(request.Format) || !SupportedFormats.ContainsKey(request.Format.ToUpperInvariant()))
        {
            return CreateErrorResponse(StatusMessage.UnsupportedExportFormat, StatusCodes.Status400BadRequest);
        }

        return null;
    }

    private Task<(bool ValidationFailed, JsonModel? ErrorResponse, int OrganizationId)> ValidateOrganizationAsync()
    {
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return Task.FromResult<(bool, JsonModel?, int)>((true, CreateErrorResponse("User must belong to an organization", StatusCodes.Status401Unauthorized), 0));
        }

        return Task.FromResult<(bool, JsonModel?, int)>((false, null, organizationId.Value));
    }

    private async Task<ExportResponseDto> ProcessExportGenerationAsync(ExportRequestDto request, int userId, Domain.Entities.DynamicQuestionnaire.Form form)
    {
        // Generate unique export ID with GUID
        var exportId = GenerateUniqueExportId();
        
        // Generate file based on format
        var formatInfo = SupportedFormats[request.Format.ToUpperInvariant()];
        var fileContent = await GenerateFileContent(request, formatInfo.ContentType);
        
        // Generate unique file name with GUID
        var fileName = GenerateUniqueFileName(form.Title, request.DataType, formatInfo.Extension);
        
        // Get export summary
        var summary = await GenerateExportSummary(request.FormId, request);
        
        // Store export metadata
        var metadata = CreateExportMetadata(exportId, request.FormId, userId, fileContent, fileName, 
            formatInfo.ContentType, summary.TotalSubmissions);
        
        _exportCache[exportId] = metadata;
        
        // Create response
        return CreateExportResponse(metadata, form, request, summary);
    }

    private string GenerateUniqueExportId()
    {
        return $"{ExportIdPrefix}_{Guid.NewGuid():N}_{DateTime.UtcNow:yyyyMMddHHmmss}";
    }

    private string GenerateUniqueFileName(string formTitle, ExportDataType dataType, string extension)
    {
        var sanitizedFormTitle = SanitizeFileName(formTitle);
        var uniqueId = Guid.NewGuid().ToString("N")[..8]; // First 8 characters of GUID
        var timestamp = DateTime.UtcNow.ToString(FileDateTimeFormat);
        return $"{sanitizedFormTitle}_{dataType}_{timestamp}_{uniqueId}.{extension}";
    }

    private async Task<byte[]> GenerateFileContent(ExportRequestDto request, string contentType)
    {
        return contentType switch
        {
            CsvContentType => await GenerateCsvAsync(request.FormId, request),
            ExcelContentType => await GenerateExcelAsync(request.FormId, request),
            PdfContentType => await GeneratePdfAsync(request.FormId, request),
            _ => throw new NotSupportedException($"Unsupported content type: {contentType}")
        };
    }

    private ExportMetadata CreateExportMetadata(string exportId, int formId, int userId, byte[] fileContent,
        string fileName, string contentType, int recordCount)
    {
        return new ExportMetadata
        {
            ExportId = exportId,
            FormId = formId,
            UserId = userId,
            FileContent = fileContent,
            FileName = fileName,
            ContentType = contentType,
            GeneratedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(DefaultExportExpirationDays),
            RecordCount = recordCount
        };
    }

    private ExportResponseDto CreateExportResponse(ExportMetadata metadata, Domain.Entities.DynamicQuestionnaire.Form form,
        ExportRequestDto request, ExportSummaryDto summary)
    {
        return new ExportResponseDto
        {
            ExportId = metadata.ExportId,
            FormId = request.FormId,
            FormTitle = form.Title,
            Format = request.Format,
            FileName = metadata.FileName,
            ContentType = metadata.ContentType,
            FileSizeBytes = metadata.FileContent.Length,
            RecordCount = summary.TotalSubmissions,
            GeneratedAt = metadata.GeneratedAt,
            DownloadUrl = string.Format(ExportEndpointTemplate, metadata.ExportId),
            ExpiresAt = metadata.ExpiresAt,
            Summary = summary
        };
    }

    private List<ExportResponseDto> GetUserExportsForForm(int formId, int userId)
    {
        return _exportCache.Values
            .Where(e => e.FormId == formId && e.UserId == userId && DateTime.UtcNow <= e.ExpiresAt)
            .Select(e => new ExportResponseDto
            {
                ExportId = e.ExportId,
                FormId = e.FormId,
                FileName = e.FileName,
                ContentType = e.ContentType,
                FileSizeBytes = e.FileContent.Length,
                RecordCount = e.RecordCount,
                GeneratedAt = e.GeneratedAt,
                DownloadUrl = string.Format(ExportEndpointTemplate, e.ExportId),
                ExpiresAt = e.ExpiresAt
            })
            .OrderByDescending(e => e.GeneratedAt)
            .ToList();
    }

    private JsonModel CreateErrorResponse(string message, int statusCode)
    {
        return new JsonModel(new { }, message, statusCode);
    }

    private async Task<IEnumerable<Domain.Entities.DynamicQuestionnaire.Submission>> GetFilteredSubmissions(int formId, ExportRequestDto request)
    {
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue) return Enumerable.Empty<Domain.Entities.DynamicQuestionnaire.Submission>();

        var submissions = await _submissionRepository.GetByFormIdAsync(formId, organizationId.Value);

        if (request.DateFrom.HasValue)
        {
            submissions = submissions.Where(s => s.SubmittedDate >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            submissions = submissions.Where(s => s.SubmittedDate <= request.DateTo.Value);
        }

        if (!string.IsNullOrEmpty(request.RespondentEmailFilter))
        {
            submissions = submissions.Where(s => s.RespondentEmail.Contains(request.RespondentEmailFilter, StringComparison.OrdinalIgnoreCase));
        }

        return submissions.OrderBy(s => s.SubmittedDate);
    }

    private async Task<ExportSummaryDto> GenerateExportSummary(int formId, ExportRequestDto request)
    {
        var submissions = await GetFilteredSubmissions(formId, request);
        var submissionsList = submissions.ToList();

        var summary = new ExportSummaryDto
        {
            TotalSubmissions = submissionsList.Count,
            UniqueRespondents = submissionsList.Select(s => s.RespondentEmail).Distinct().Count()
        };

        if (submissionsList.Any())
        {
            summary.EarliestSubmission = submissionsList.Min(s => s.SubmittedDate);
            summary.LatestSubmission = submissionsList.Max(s => s.SubmittedDate);

            if (request.IncludeScoring)
            {
                var scores = submissionsList.Select(s => s.TotalScore).ToList();
                summary.AverageScore = scores.Average();
                summary.HighestScore = scores.Max();
                summary.LowestScore = scores.Min();
            }
        }

        return summary;
    }

    private static string FormatAnswerForCsv(Domain.Entities.DynamicQuestionnaire.Answer? answer, Domain.Entities.DynamicQuestionnaire.Question question)
    {
        if (answer?.AnswerValues == null || !answer.AnswerValues.Any())
            return "";

        var answerTexts = new List<string>();

        foreach (var answerValue in answer.AnswerValues)
        {
            if (!string.IsNullOrEmpty(answerValue.TextValue))
            {
                answerTexts.Add(answerValue.TextValue);
            }
            else if (answerValue.NumericValue.HasValue)
            {
                answerTexts.Add(answerValue.NumericValue.Value.ToString(CultureInfo.InvariantCulture));
            }
            else if (answerValue.SelectedOptionId.HasValue)
            {
                var option = question.Options?.FirstOrDefault(o => o.Id == answerValue.SelectedOptionId.Value);
                if (option != null && !string.IsNullOrEmpty(option.OptionText))
                {
                    answerTexts.Add(option.OptionText);
                }
            }
        }

        return string.Join("; ", answerTexts);
    }

    private async Task<decimal> CalculateMaxPossibleScore(int formId, int organizationId)
    {
        var form = await _formRepository.GetByIdWithDetailsAsync(formId, organizationId);
        if (form == null) return Number.Zero;

        return form.Pages
            .SelectMany(page => page.Questions)
            .SelectMany(question => question.Options ?? Enumerable.Empty<Domain.Entities.DynamicQuestionnaire.Option>())
            .Sum(option => option.Score);
    }

    private static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return DefaultFileName;

        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        return string.IsNullOrEmpty(sanitized) ? DefaultFileName : sanitized;
    }

    #endregion
}
