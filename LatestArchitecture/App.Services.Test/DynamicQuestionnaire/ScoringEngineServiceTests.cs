using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Service.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace App.Services.Test.DynamicQuestionnaire;

public class ScoringEngineServiceTests : DynamicQuestionnaireTestBase
{
    private readonly ScoringEngineService _scoringEngineService;
    private readonly Mock<ILogger<ScoringEngineService>> _mockLogger;
    private readonly Mock<ICurrentUserClaimService> _mockCurrentUserClaimService;
    private readonly IFormRepository _formRepository;
    private readonly ISubmissionRepository _submissionRepository;

    public ScoringEngineServiceTests()
    {
        _mockLogger = new Mock<ILogger<ScoringEngineService>>();
        _mockCurrentUserClaimService = new Mock<ICurrentUserClaimService>();
        _mockCurrentUserClaimService.Setup(x => x.OrganizationId).Returns(1);
        _formRepository = ServiceProvider.GetRequiredService<IFormRepository>();
        _submissionRepository = ServiceProvider.GetRequiredService<ISubmissionRepository>();
        
        _scoringEngineService = new ScoringEngineService(
            _formRepository,
            _submissionRepository,
            _mockCurrentUserClaimService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CalculateSubmissionScoreAsync_ValidSubmission_ReturnsCalculatedScore()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptions();
        var submission = await CreateTestSubmissionWithAnswers(form.Id, form.Pages.First().Questions.First().Id);

        // Act
        var result = await _scoringEngineService.CalculateSubmissionScoreAsync(submission.Id);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Score calculated successfully");

        // Check if data contains expected properties
        result.Data.Should().NotBeNull();
        result.Data.ToString().Should().Contain("SubmissionId");
        result.Data.ToString().Should().Contain("TotalScore");
    }

    [Fact]
    public async Task CalculateSubmissionScoreAsync_NonExistentSubmission_ReturnsNotFound()
    {
        // Act
        var result = await _scoringEngineService.CalculateSubmissionScoreAsync(999);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("Submission not found");
    }

    [Fact]
    public async Task GetFormScoringAnalyticsAsync_FormWithSubmissions_ReturnsAnalytics()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptions();
        var submission1 = await CreateTestSubmissionWithAnswers(form.Id, form.Pages.First().Questions.First().Id);
        var submission2 = await CreateTestSubmissionWithAnswers(form.Id, form.Pages.First().Questions.First().Id);

        // Set different scores for testing
        submission1.TotalScore = 80;
        submission2.TotalScore = 90;
        _submissionRepository.Update(submission1);
        _submissionRepository.Update(submission2);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _scoringEngineService.GetFormScoringAnalyticsAsync(form.Id);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Analytics retrieved successfully");

        var analytics = result.Data as ScoringAnalyticsDto;
        analytics.Should().NotBeNull();
        analytics!.FormId.Should().Be(form.Id);
        analytics.FormTitle.Should().Be(form.Title);
        analytics.TotalSubmissions.Should().Be(2);
        analytics.AverageScore.Should().Be(85);
        analytics.HighestScore.Should().Be(90);
        analytics.LowestScore.Should().Be(80);
    }

    [Fact]
    public async Task GetSubmissionScoreBreakdownAsync_ValidSubmission_ReturnsBreakdown()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptions();
        var submission = await CreateTestSubmissionWithAnswers(form.Id, form.Pages.First().Questions.First().Id);
        submission.TotalScore = 75;
        submission.RespondentEmail = "test@example.com";
        _submissionRepository.Update(submission);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _scoringEngineService.GetSubmissionScoreBreakdownAsync(submission.Id);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Score breakdown retrieved successfully");

        var breakdown = result.Data as SubmissionScoreBreakdownDto;
        breakdown.Should().NotBeNull();
        breakdown!.SubmissionId.Should().Be(submission.Id);
        breakdown.RespondentEmail.Should().Be("test@example.com");
        breakdown.TotalScore.Should().Be(75);
    }

    [Fact]
    public async Task UpdateFormScoringAsync_ValidRequest_UpdatesScoring()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptions();
        var option = form.Pages.First().Questions.First().Options.First();
        
        var request = new UpdateFormScoringRequest
        {
            FormId = form.Id,
            OptionScores = new List<OptionScoreUpdate>
            {
                new OptionScoreUpdate { OptionId = option.Id, Score = 25 }
            },
            MatrixColumnScores = new List<MatrixColumnScoreUpdate>()
        };

        // Act
        var result = await _scoringEngineService.UpdateFormScoringAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Scoring updated successfully");

        // Verify option score was updated
        var updatedForm = await _formRepository.GetByIdWithDetailsAsync(form.Id, 1);
        var updatedOption = updatedForm!.Pages.First().Questions.First().Options.First();
        updatedOption.Score.Should().Be(25);
    }

    [Fact]
    public async Task GetTopPerformersAsync_FormWithSubmissions_ReturnsTopPerformers()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptions();
        var submission1 = await CreateTestSubmissionWithAnswers(form.Id, form.Pages.First().Questions.First().Id);
        var submission2 = await CreateTestSubmissionWithAnswers(form.Id, form.Pages.First().Questions.First().Id);
        var submission3 = await CreateTestSubmissionWithAnswers(form.Id, form.Pages.First().Questions.First().Id);

        submission1.TotalScore = 95;
        submission1.RespondentEmail = "top1@example.com";
        submission2.TotalScore = 85;
        submission2.RespondentEmail = "top2@example.com";
        submission3.TotalScore = 75;
        submission3.RespondentEmail = "top3@example.com";

        _submissionRepository.Update(submission1);
        _submissionRepository.Update(submission2);
        _submissionRepository.Update(submission3);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _scoringEngineService.GetTopPerformersAsync(form.Id, 2);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Top performers retrieved successfully");

        var topPerformers = result.Data as List<TopPerformerDto>;
        topPerformers.Should().NotBeNull();
        topPerformers!.Should().HaveCount(2);
        topPerformers![0].TotalScore.Should().Be(95);
        topPerformers[0].Rank.Should().Be(1);
        topPerformers[1].TotalScore.Should().Be(85);
        topPerformers[1].Rank.Should().Be(2);
    }

    [Fact]
    public async Task GetFormScoringAnalyticsAsync_NonExistentForm_ReturnsNotFound()
    {
        // Act
        var result = await _scoringEngineService.GetFormScoringAnalyticsAsync(999);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("Form not found");
    }

    [Fact]
    public async Task GetSubmissionScoreBreakdownAsync_NonExistentSubmission_ReturnsNotFound()
    {
        // Act
        var result = await _scoringEngineService.GetSubmissionScoreBreakdownAsync(999);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("Submission not found");
    }

    [Fact]
    public async Task UpdateFormScoringAsync_NonExistentForm_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateFormScoringRequest
        {
            FormId = 999,
            OptionScores = new List<OptionScoreUpdate>(),
            MatrixColumnScores = new List<MatrixColumnScoreUpdate>()
        };

        // Act
        var result = await _scoringEngineService.UpdateFormScoringAsync(request, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("Form not found");
    }

    [Fact]
    public async Task ExportScoringReportAsync_FormWithSubmissions_ReturnsReportData()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptions();
        var submission = await CreateTestSubmissionWithAnswers(form.Id, form.Pages.First().Questions.First().Id);
        submission.TotalScore = 88;
        submission.RespondentEmail = "reporter@example.com";
        _submissionRepository.Update(submission);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _scoringEngineService.ExportScoringReportAsync(form.Id, "CSV");

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Report data prepared for export");

        // Check if data contains expected report data
        result.Data.Should().NotBeNull();
        result.Data.ToString().Should().Contain("FormTitle");
        result.Data.ToString().Should().Contain("CSV");
    }

    #region Helper Methods

    private async Task<Form> CreateTestFormWithQuestionsAndOptions()
    {
        var form = new Form
        {
            Id = 1,
            Title = "Test Scoring Form",
            Description = "Form for testing scoring engine",
            IsPublished = true,
            CreatedBy = 1,
            CreatedAt = DateTime.UtcNow,
            Pages = new List<Page>()
        };

        var page = new Page
        {
            Id = 1,
            FormId = form.Id,
            PageOrder = 1,
            Title = "Test Page",
            Questions = new List<Question>()
        };

        var question = new Question
        {
            Id = 1,
            PageId = page.Id,
            QuestionText = "Test Question",
            QuestionTypeId = 1, // Assuming Multi type exists
            IsRequired = true,
            QuestionOrder = 1,
            Options = new List<Option>()
        };

        var option1 = new Option
        {
            Id = 1,
            QuestionId = question.Id,
            OptionText = "Option 1",
            Score = 10,
            DisplayOrder = 1
        };

        var option2 = new Option
        {
            Id = 2,
            QuestionId = question.Id,
            OptionText = "Option 2",
            Score = 20,
            DisplayOrder = 2
        };

        question.Options.Add(option1);
        question.Options.Add(option2);
        page.Questions.Add(question);
        form.Pages.Add(page);

        await _formRepository.CreateAsync(form);
        return form;
    }

    private async Task<Submission> CreateTestSubmissionWithAnswers(int formId, int questionId)
    {
        var submission = new Submission
        {
            FormId = formId,
            RespondentEmail = $"test{DateTime.Now.Ticks}@example.com",
            SubmittedDate = DateTime.UtcNow,
            TotalScore = 0,
            CreatedBy = 1,
            CreatedAt = DateTime.UtcNow,
            Answers = new List<Answer>()
        };

        var answer = new Answer
        {
            QuestionId = questionId,
            Score = 15,
            CreatedBy = 1,
            CreatedAt = DateTime.UtcNow,
            AnswerValues = new List<AnswerValue>
            {
                new AnswerValue
                {
                    SelectedOptionId = 1,
                    TextValue = "Test Answer",
                    CreatedBy = 1,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };

        submission.Answers.Add(answer);
        await _submissionRepository.CreateAsync(submission);
        return submission;
    }

    #endregion
}
