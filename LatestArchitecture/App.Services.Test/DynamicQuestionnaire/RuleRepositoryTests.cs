using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using FluentAssertions;
using App.Application.Constant.DynamicQuestionnaire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace App.Services.Test.DynamicQuestionnaire;

/// <summary>
/// Tests for Rule Repository functionality
/// </summary>
public class RuleRepositoryTests : DynamicQuestionnaireTestBase
{
    private readonly IRuleRepository _repository;

    public RuleRepositoryTests()
    {
        _repository = ServiceProvider.GetRequiredService<IRuleRepository>();
    }

    #region Create Tests

    [Fact]
    public async Task CreateAsync_WithValidRule_ShouldCreateRuleSuccessfully()
    {
        // Arrange
        var form = await CreateTestForm();
        var sourceQuestion = form.Pages.First().Questions.First();
        var targetQuestion = form.Pages.First().Questions.Last();
        var triggerOption = sourceQuestion.Options.First();

        var rule = new Rule
        {
            FormId = form.Id,
            SourceQuestionId = sourceQuestion.Id,
            TriggerOptionId = triggerOption.Id,
            Condition = RuleConstants.IsSelected,
            ActionType = RuleConstants.HideQuestion,
            TargetQuestionId = targetQuestion.Id,
            CreatedBy = 1
        };

        // Act
        var result = await _repository.CreateAsync(rule);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.FormId.Should().Be(form.Id);
        result.SourceQuestionId.Should().Be(sourceQuestion.Id);
        result.TriggerOptionId.Should().Be(triggerOption.Id);
    result.Condition.Should().Be(RuleConstants.IsSelected);
    result.ActionType.Should().Be(RuleConstants.HideQuestion);
        result.TargetQuestionId.Should().Be(targetQuestion.Id);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_WithSliderRangeRule_ShouldCreateWithRangeValues()
    {
        // Arrange
        var form = await CreateTestForm();
        var sourceQuestion = form.Pages.First().Questions.First();
        var targetQuestion = form.Pages.First().Questions.Last();

        var rule = new Rule
        {
            FormId = form.Id,
            SourceQuestionId = sourceQuestion.Id,
            Condition = RuleConstants.IsInRange,
            ActionType = RuleConstants.HideQuestion,
            TargetQuestionId = targetQuestion.Id,
            MinValue = 5,
            MaxValue = 10,
            CreatedBy = 1
        };

        // Act
        var result = await _repository.CreateAsync(rule);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.MinValue.Should().Be(5);
        result.MaxValue.Should().Be(10);
    result.Condition.Should().Be(RuleConstants.IsInRange);
    }

    #endregion

    #region Read Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnRule()
    {
        // Arrange
        var form = await CreateTestForm();
        var rule = await CreateTestRule(form);

        // Act
        var result = await _repository.GetByIdAsync(rule.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(rule.Id);
        result.FormId.Should().Be(form.Id);
        result.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidId = 999;

        // Act
        var result = await _repository.GetByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithDeletedRule_ShouldReturnNull()
    {
        // Arrange
        var form = await CreateTestForm();
        var rule = await CreateTestRule(form);
        await _repository.DeleteAsync(rule.Id, 1);

        // Act
        var result = await _repository.GetByIdAsync(rule.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_WithValidId_ShouldReturnRuleWithNavigationProperties()
    {
        // Arrange
        var form = await CreateTestForm();
        var rule = await CreateTestRule(form);

        // Act
        var result = await _repository.GetByIdWithDetailsAsync(rule.Id, 1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(rule.Id);
        result.Form.Should().NotBeNull();
        result.SourceQuestion.Should().NotBeNull();
        result.TargetQuestion.Should().NotBeNull();
        result.TriggerOption.Should().NotBeNull();
        result.SourceQuestion!.QuestionType.Should().NotBeNull();
        result.TargetQuestion!.QuestionType.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByFormIdAsync_WithValidFormId_ShouldReturnAllFormRules()
    {
        // Arrange
        var form = await CreateTestForm();
        var rule1 = await CreateTestRule(form);
    var rule2 = await CreateTestRule(form, actionType: RuleConstants.TerminateForm);

        // Act
        var result = await _repository.GetByFormIdAsync(form.Id, 1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Id == rule1.Id);
        result.Should().Contain(r => r.Id == rule2.Id);
        result.All(r => r.FormId == form.Id).Should().BeTrue();
        result.All(r => !r.IsDeleted).Should().BeTrue();
    }

    [Fact]
    public async Task GetByFormIdAsync_WithNoRules_ShouldReturnEmptyList()
    {
        // Arrange
        var form = await CreateTestForm();

        // Act
        var result = await _repository.GetByFormIdAsync(form.Id, 1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByFormIdWithDetailsAsync_WithValidFormId_ShouldReturnRulesWithDetails()
    {
        // Arrange
        var form = await CreateTestForm();
        await CreateTestRule(form);
    await CreateTestRule(form, actionType: RuleConstants.TerminateForm);

        // Act
        var result = await _repository.GetByFormIdWithDetailsAsync(form.Id, 1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        foreach (var rule in result)
        {
            rule.SourceQuestion.Should().NotBeNull();
            rule.SourceQuestion!.QuestionType.Should().NotBeNull();
            if (rule.TargetQuestionId.HasValue)
            {
                rule.TargetQuestion.Should().NotBeNull();
                rule.TargetQuestion!.QuestionType.Should().NotBeNull();
            }
            if (rule.TriggerOptionId.HasValue)
            {
                rule.TriggerOption.Should().NotBeNull();
            }
        }
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task UpdateAsync_WithValidRule_ShouldUpdateRuleSuccessfully()
    {
        // Arrange
        var form = await CreateTestForm();
        var rule = await CreateTestRule(form);
        var originalCreatedAt = rule.CreatedAt;

        // Modify rule properties
    rule.Condition = RuleConstants.IsNotSelected;
    rule.ActionType = RuleConstants.SkipToPage;
        rule.TargetQuestionId = null;
        rule.TargetPageId = form.Pages.First().Id;

        // Act
        var result = await _repository.UpdateAsync(rule);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(rule.Id);
    result.Condition.Should().Be(RuleConstants.IsNotSelected);
    result.ActionType.Should().Be(RuleConstants.SkipToPage);
        result.TargetQuestionId.Should().BeNull();
        result.TargetPageId.Should().Be(form.Pages.First().Id);
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.CreatedAt.Should().Be(originalCreatedAt); // Should not change
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldSoftDeleteRule()
    {
        // Arrange
        var form = await CreateTestForm();
        var rule = await CreateTestRule(form);

        // Act
        var result = await _repository.DeleteAsync(rule.Id, 1);

        // Assert
        result.Should().BeTrue();

        // Verify soft delete
        var deletedRule = await DbContext.Set<Rule>().FindAsync(rule.Id);
        deletedRule.Should().NotBeNull();
        deletedRule!.IsDeleted.Should().BeTrue();
        deletedRule.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verify rule is not returned in normal queries
        var activeRule = await _repository.GetByIdAsync(rule.Id);
        activeRule.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var invalidId = 999;

        // Act
        var result = await _repository.DeleteAsync(invalidId, 1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WithAlreadyDeletedRule_ShouldReturnFalse()
    {
        // Arrange
        var form = await CreateTestForm();
        var rule = await CreateTestRule(form);
        await _repository.DeleteAsync(rule.Id, 1); // Delete once

        // Act
        var result = await _repository.DeleteAsync(rule.Id, 1); // Try to delete again

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Validation Tests

    [Fact]
    public async Task RuleExistsAsync_WithExistingRule_ShouldReturnTrue()
    {
        // Arrange
        var form = await CreateTestForm();
        var rule = await CreateTestRule(form);

        // Act
        var result = await _repository.RuleExistsAsync(
            rule.FormId,
            rule.SourceQuestionId,
            rule.TriggerOptionId,
            rule.Condition,
            1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task RuleExistsAsync_WithNonExistingRule_ShouldReturnFalse()
    {
        // Arrange
        var form = await CreateTestForm();
        var sourceQuestion = form.Pages.First().Questions.First();
        var triggerOption = sourceQuestion.Options.First();

        // Act
        var result = await _repository.RuleExistsAsync(
            form.Id,
            sourceQuestion.Id,
            triggerOption.Id,
            "NonExistentCondition",
            1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RuleExistsAsync_WithDeletedRule_ShouldReturnFalse()
    {
        // Arrange
        var form = await CreateTestForm();
        var rule = await CreateTestRule(form);
        await _repository.DeleteAsync(rule.Id, 1);

        // Act
        var result = await _repository.RuleExistsAsync(
            rule.FormId,
            rule.SourceQuestionId,
            rule.TriggerOptionId,
            rule.Condition,
            1);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Repository Base Interface Tests

    [Fact]
    public async Task GetAll_ShouldReturnActiveRulesQueryable()
    {
        // Arrange
        var form = await CreateTestForm();
        var activeRule = await CreateTestRule(form);
    var deletedRule = await CreateTestRule(form, actionType: RuleConstants.TerminateForm);
        await _repository.DeleteAsync(deletedRule.Id, 1);

        // Act
        var result = await _repository.GetAll().ToListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain(r => r.Id == activeRule.Id);
        result.Should().NotContain(r => r.Id == deletedRule.Id);
    }

    [Fact]
    public async Task GetAllNoTracking_ShouldReturnActiveRulesWithoutTracking()
    {
        // Arrange
        var form = await CreateTestForm();
        var rule = await CreateTestRule(form);

        // Act
        var result = await _repository.GetAllNoTracking().ToListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(rule.Id);

        // Verify no tracking
        var trackedEntities = DbContext.ChangeTracker.Entries<Rule>().Count();
        trackedEntities.Should().Be(1); // Only the one we created, not the ones from GetAllNoTracking
    }

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var form = await CreateTestForm();
        var rule = await CreateTestRule(form);

        // Act
        var result = await _repository.ExistsAsync(rule.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var invalidId = 999;

        // Act
        var result = await _repository.ExistsAsync(invalidId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_WithDeletedRule_ShouldReturnFalse()
    {
        // Arrange
        var form = await CreateTestForm();
        var rule = await CreateTestRule(form);
        await _repository.DeleteAsync(rule.Id, 1);

        // Act
        var result = await _repository.ExistsAsync(rule.Id);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Helper Methods

    private async Task<Form> CreateTestForm()
    {
        var radioQuestionType = await DbContext.MasterQuestionTypes
            .FirstAsync(qt => qt.TypeName == "Radio");

        var form = new Form
        {
            Title = "Test Form for Rules",
            Description = "Test Description",
            CreatedBy = 1,
            Pages = new List<Page>
            {
                new()
                {
                    Title = "Page 1",
                    PageOrder = 1,
                    CreatedBy = 1,
                    Questions = new List<Question>
                    {
                        new()
                        {
                            QuestionText = "Source Question",
                            QuestionTypeId = radioQuestionType.Id,
                            QuestionOrder = 1,
                            IsRequired = true,
                            CreatedBy = 1,
                            Options = new List<Option>
                            {
                                new() { OptionText = "Option 1", DisplayOrder = 1, CreatedBy = 1 },
                                new() { OptionText = "Option 2", DisplayOrder = 2, CreatedBy = 1 }
                            }
                        },
                        new()
                        {
                            QuestionText = "Target Question",
                            QuestionTypeId = radioQuestionType.Id,
                            QuestionOrder = 2,
                            IsRequired = false,
                            CreatedBy = 1,
                            Options = new List<Option>
                            {
                                new() { OptionText = "Yes", DisplayOrder = 1, CreatedBy = 1 },
                                new() { OptionText = "No", DisplayOrder = 2, CreatedBy = 1 }
                            }
                        }
                    }
                }
            }
        };

        DbContext.Set<Form>().Add(form);
        await DbContext.SaveChangesAsync();
        return form;
    }

    private async Task<Rule> CreateTestRule(Form form, string actionType = RuleConstants.HideQuestion)
    {
        var sourceQuestion = form.Pages.First().Questions.First();
        var targetQuestion = form.Pages.First().Questions.Last();
        var triggerOption = sourceQuestion.Options.First();

        var rule = new Rule
        {
            FormId = form.Id,
            SourceQuestionId = sourceQuestion.Id,
            TriggerOptionId = triggerOption.Id,
            Condition = RuleConstants.IsSelected,
            ActionType = actionType,
            TargetQuestionId = actionType == RuleConstants.HideQuestion ? targetQuestion.Id : null,
            TargetPageId = actionType == RuleConstants.SkipToPage ? form.Pages.First().Id : null,
            CreatedBy = 1
        };

        return await _repository.CreateAsync(rule);
    }

    #endregion
}
