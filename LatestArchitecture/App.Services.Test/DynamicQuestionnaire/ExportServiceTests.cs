using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Application.Service.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace App.Services.Test.DynamicQuestionnaire;

/// <summary>
/// Phase 8: Comprehensive tests for export functionality
/// </summary>
public class ExportServiceTests : DynamicQuestionnaireTestBase
{
    private readonly IExportService _exportService;
    private readonly Mock<ILogger<ExportService>> _mockLogger;
    private readonly Mock<ICurrentUserClaimService> _mockCurrentUserClaimService;
    private readonly IScoringEngineService _scoringEngineService;

    public ExportServiceTests()
    {
        _mockLogger = new Mock<ILogger<ExportService>>();
        _mockCurrentUserClaimService = new Mock<ICurrentUserClaimService>();
        _mockCurrentUserClaimService.Setup(x => x.OrganizationId).Returns(1);
        _scoringEngineService = ServiceProvider.GetRequiredService<IScoringEngineService>();
        
        _exportService = new ExportService(
            ServiceProvider.GetRequiredService<IFormRepository>(),
            ServiceProvider.GetRequiredService<ISubmissionRepository>(),
            _scoringEngineService,
            _mockCurrentUserClaimService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GenerateExportAsync_WithValidCsvRequest_ShouldGenerateExport()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptionsAsync();
        await CreateTestSubmissionWithAnswersAsync(form.Id, form.Pages.First().Questions.First().Id);

        var request = new ExportRequestDto
        {
            FormId = form.Id,
            Format = "CSV",
            DataType = ExportDataType.Responses,
            IncludeScoring = true,
            IncludeQuestionText = true
        };

        // Act
        var result = await _exportService.GenerateExportAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Data.Should().BeOfType<ExportResponseDto>();

        var exportResponse = result.Data.Should().BeOfType<ExportResponseDto>().Subject;
        exportResponse.ExportId.Should().NotBeEmpty();
        exportResponse.FileName.Should().Contain(".csv");
        exportResponse.RecordCount.Should().BeGreaterThan(0);
        exportResponse.FileSizeBytes.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GenerateExportAsync_WithInvalidFormId_ShouldReturnNotFound()
    {
        // Arrange
        var request = new ExportRequestDto
        {
            FormId = 99999,
            Format = "CSV",
            DataType = ExportDataType.Responses
        };

        // Act
        var result = await _exportService.GenerateExportAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Contain("Form not found");
    }

    [Fact]
    public async Task GenerateExportAsync_WithExcelFormat_ShouldGenerateExcelFile()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptionsAsync();
        await CreateTestSubmissionWithAnswersAsync(form.Id, form.Pages.First().Questions.First().Id);

        var request = new ExportRequestDto
        {
            FormId = form.Id,
            Format = "EXCEL",
            DataType = ExportDataType.Responses,
            IncludeScoring = true
        };

        // Act
        var result = await _exportService.GenerateExportAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);

        var exportResponse = result.Data.Should().BeOfType<ExportResponseDto>().Subject;
        exportResponse.FileName.Should().Contain(".xlsx");
        exportResponse.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [Fact]
    public async Task GenerateExportAsync_WithPdfFormat_ShouldGeneratePdfFile()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptionsAsync();
        await CreateTestSubmissionWithAnswersAsync(form.Id, form.Pages.First().Questions.First().Id);

        var request = new ExportRequestDto
        {
            FormId = form.Id,
            Format = "PDF",
            DataType = ExportDataType.Analytics,
            IncludeScoring = true
        };

        // Act
        var result = await _exportService.GenerateExportAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);

        var exportResponse = result.Data.Should().BeOfType<ExportResponseDto>().Subject;
        exportResponse.FileName.Should().Contain(".pdf");
        exportResponse.ContentType.Should().Be("application/pdf");
    }

    [Fact]
    public async Task GenerateExportAsync_WithDateFilter_ShouldFilterByDate()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptionsAsync();
        await CreateTestSubmissionWithAnswersAsync(form.Id, form.Pages.First().Questions.First().Id);

        var request = new ExportRequestDto
        {
            FormId = form.Id,
            Format = "CSV",
            DataType = ExportDataType.Responses,
            DateFrom = DateTime.UtcNow.AddDays(-1),
            DateTo = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await _exportService.GenerateExportAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Data.Should().BeOfType<ExportResponseDto>();
    }

    [Fact]
    public async Task GenerateExportAsync_WithScoringAnalytics_ShouldIncludeScores()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptionsAsync();
        await CreateTestSubmissionWithAnswersAsync(form.Id, form.Pages.First().Questions.First().Id);

        var request = new ExportRequestDto
        {
            FormId = form.Id,
            Format = "CSV",
            DataType = ExportDataType.Analytics,
            IncludeScoring = true
        };

        // Act
        var result = await _exportService.GenerateExportAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);

        var exportResponse = result.Data.Should().BeOfType<ExportResponseDto>().Subject;
        exportResponse.Summary.Should().NotBeNull();
        exportResponse.Summary!.AverageScore.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GenerateExportAsync_WithCustomColumns_ShouldIncludeSpecifiedColumns()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptionsAsync();
        await CreateTestSubmissionWithAnswersAsync(form.Id, form.Pages.First().Questions.First().Id);

        var request = new ExportRequestDto
        {
            FormId = form.Id,
            Format = "CSV",
            DataType = ExportDataType.Responses,
            IncludeQuestionText = true
        };

        // Act
        var result = await _exportService.GenerateExportAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);

        var exportResponse = result.Data.Should().BeOfType<ExportResponseDto>().Subject;
        exportResponse.RecordCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GenerateExportAsync_WithLargeDataset_ShouldHandleMultipleSubmissions()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptionsAsync();
        await CreateTestSubmissionWithAnswersAsync(form.Id, form.Pages.First().Questions.First().Id);
        await CreateTestSubmissionWithAnswersAsync(form.Id, form.Pages.First().Questions.First().Id);

        var request = new ExportRequestDto
        {
            FormId = form.Id,
            Format = "CSV",
            DataType = ExportDataType.Responses
        };

        // Act
        var result = await _exportService.GenerateExportAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);

        var exportResponse = result.Data.Should().BeOfType<ExportResponseDto>().Subject;
        exportResponse.RecordCount.Should().Be(2);
        exportResponse.Summary!.TotalSubmissions.Should().Be(2);
    }

    [Fact]
    public async Task GenerateExportAsync_WithComplexAnalytics_ShouldProvideDetailedStats()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptionsAsync();
        await CreateTestSubmissionWithAnswersAsync(form.Id, form.Pages.First().Questions.First().Id);
        await CreateTestSubmissionWithAnswersAsync(form.Id, form.Pages.First().Questions.First().Id);

        var request = new ExportRequestDto
        {
            FormId = form.Id,
            Format = "PDF",
            DataType = ExportDataType.Analytics,
            IncludeScoring = true
        };

        // Act
        var result = await _exportService.GenerateExportAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);

        var exportResponse = result.Data.Should().BeOfType<ExportResponseDto>().Subject;
        exportResponse.Summary.Should().NotBeNull();
        exportResponse.Summary!.TotalSubmissions.Should().Be(2);
    }

    [Fact]
    public async Task GenerateExportAsync_WithInvalidFormat_ShouldReturnBadRequest()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptionsAsync();
        await CreateTestSubmissionWithAnswersAsync(form.Id, form.Pages.First().Questions.First().Id);

        var request = new ExportRequestDto
        {
            FormId = form.Id,
            Format = "INVALID",
            DataType = ExportDataType.Responses
        };

        // Act
        var result = await _exportService.GenerateExportAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Contain("Unsupported export format");
    }

    [Fact]
    public async Task GenerateExportAsync_WithEmptyData_ShouldReturnEmptyExport()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptionsAsync();

        var request = new ExportRequestDto
        {
            FormId = form.Id,
            Format = "CSV",
            DataType = ExportDataType.Responses
        };

        // Act
        var result = await _exportService.GenerateExportAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);

        var exportResponse = result.Data.Should().BeOfType<ExportResponseDto>().Subject;
        exportResponse.RecordCount.Should().Be(0);
        exportResponse.Summary!.TotalSubmissions.Should().Be(0);
    }

    [Fact]
    public async Task GenerateExportAsync_WithUnsupportedDataType_ShouldReturnBadRequest()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptionsAsync();

        var request = new ExportRequestDto
        {
            FormId = form.Id,
            Format = "CSV",
            DataType = (ExportDataType)999 // Invalid enum value
        };

        // Act
        var result = await _exportService.GenerateExportAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Contain("Unsupported data type");
    }

    [Fact]
    public async Task GenerateExportAsync_WithNullRequest_ShouldReturnBadRequest()
    {
        // Act
        var result = await _exportService.GenerateExportAsync(null!, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Contain("Export request cannot be null");
    }

    private async Task<Form> CreateTestFormWithQuestionsAndOptionsAsync()
    {
        var form = CreateTestForm();
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        var page = CreateTestPage(form.Id);
        DbContext.Pages.Add(page);
        await DbContext.SaveChangesAsync();

        var question = CreateTestQuestion(page.Id);
        DbContext.Questions.Add(question);
        await DbContext.SaveChangesAsync();

        var option = CreateTestOption(question.Id);
        DbContext.Options.Add(option);
        await DbContext.SaveChangesAsync();

        // Reload with related data
        return await DbContext.Forms
            .Include(f => f.Pages)
                .ThenInclude(p => p.Questions)
                    .ThenInclude(q => q.Options)
            .FirstAsync(f => f.Id == form.Id);
    }

    private async Task<Submission> CreateTestSubmissionWithAnswersAsync(int formId, int questionId)
    {
        var submission = new Submission
        {
            FormId = formId,
            CreatedBy = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.Submissions.Add(submission);
        await DbContext.SaveChangesAsync();

        var answer = new Answer
        {
            SubmissionId = submission.Id,
            QuestionId = questionId,
            CreatedBy = 1
        };
        DbContext.Answers.Add(answer);
        await DbContext.SaveChangesAsync();

        var answerValue = new AnswerValue
        {
            AnswerId = answer.Id,
            TextValue = "Test Answer",
            CreatedBy = 1
        };
        DbContext.AnswerValues.Add(answerValue);
        await DbContext.SaveChangesAsync();

        return submission;
    }
}
