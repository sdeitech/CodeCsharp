using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace App.Services.Test.DynamicQuestionnaire;

public class TestModeServiceTests : DynamicQuestionnaireTestBase
{
    private readonly IDynamicQuestionnaireService _dynamicQuestionnaireService;

    public TestModeServiceTests()
    {
        _dynamicQuestionnaireService = ServiceProvider.GetRequiredService<IDynamicQuestionnaireService>();
    }

    [Fact]
    public async Task SubmitTestModeAsync_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptions();
        var request = new TestModeSubmissionRequest
        {
            FormId = form.Id,
            CalculateScore = true,
            EvaluateRules = true,
            Answers = new List<TestModeAnswer>
            {
                new TestModeAnswer
                {
                    QuestionId = form.Pages.First().Questions.First().Id,
                    SelectedOptionIds = new List<int> { form.Pages.First().Questions.First().Options.First().Id }
                }
            }
        };

        // Act
        var result = await _dynamicQuestionnaireService.SubmitTestModeAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data.Should().NotBeNull();
        
        var testResult = result.Data as TestModeSubmissionResult;
        testResult.Should().NotBeNull();
        testResult!.FormId.Should().Be(form.Id);
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task EvaluateTestModeRulesAsync_ValidRequest_ReturnsRuleEvaluation()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptions();
        var request = new TestModeSubmissionRequest
        {
            FormId = form.Id,
            EvaluateRules = true,
            Answers = new List<TestModeAnswer>
            {
                new TestModeAnswer
                {
                    QuestionId = form.Pages.First().Questions.First().Id,
                    SelectedOptionIds = new List<int> { form.Pages.First().Questions.First().Options.First().Id }
                }
            }
        };

        // Act
        var result = await _dynamicQuestionnaireService.EvaluateTestModeRulesAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task CalculateTestModeScoreAsync_ValidRequest_ReturnsScoreCalculation()
    {
        // Arrange
        var form = await CreateTestFormWithQuestionsAndOptions();
        var request = new TestModeSubmissionRequest
        {
            FormId = form.Id,
            CalculateScore = true,
            Answers = new List<TestModeAnswer>
            {
                new TestModeAnswer
                {
                    QuestionId = form.Pages.First().Questions.First().Id,
                    SelectedOptionIds = new List<int> { form.Pages.First().Questions.First().Options.First().Id }
                }
            }
        };

        // Act
        var result = await _dynamicQuestionnaireService.CalculateTestModeScoreAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Data.Should().NotBeNull();
        
        var scoreResult = result.Data as TestModeScoreResult;
        scoreResult.Should().NotBeNull();
        scoreResult!.TotalScore.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task SubmitTestModeAsync_NonExistentForm_ReturnsNotFound()
    {
        // Arrange
        var request = new TestModeSubmissionRequest
        {
            FormId = 999,
            Answers = new List<TestModeAnswer>()
        };

        // Act
        var result = await _dynamicQuestionnaireService.SubmitTestModeAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    private async Task<Form> CreateTestFormWithQuestionsAndOptions()
    {
        var form = new Form
        {
            Id = 1,
            Title = "Test Form",
            Description = "Test Description",
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
            QuestionTypeId = 1,
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

        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();
        return form;
    }
}
