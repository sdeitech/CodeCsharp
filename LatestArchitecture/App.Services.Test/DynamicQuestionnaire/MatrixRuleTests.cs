using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Application.Constant.DynamicQuestionnaire;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Common.Enums;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Domain.Enums;
using App.Domain.Enums.DynamicQuestionnaire;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace App.Services.Test.DynamicQuestionnaire;

public class MatrixRuleTests : DynamicQuestionnaireTestBase
{
    private readonly IFormRepository _formRepository;
    private readonly IMasterQuestionTypeRepository _masterQuestionTypeRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAnswerRepository _answerRepository;
    private readonly IAnswerValueRepository _answerValueRepository;
    private readonly IRuleRepository _ruleRepository;
    private const int TestUserId = 1;

    public MatrixRuleTests()
    {
        _formRepository = ServiceProvider.GetRequiredService<IFormRepository>();
        _masterQuestionTypeRepository = ServiceProvider.GetRequiredService<IMasterQuestionTypeRepository>();
        _submissionRepository = ServiceProvider.GetRequiredService<ISubmissionRepository>();
        _answerRepository = ServiceProvider.GetRequiredService<IAnswerRepository>();
        _answerValueRepository = ServiceProvider.GetRequiredService<IAnswerValueRepository>();
        _ruleRepository = ServiceProvider.GetRequiredService<IRuleRepository>();
    }

    [Fact]
    public async Task CreateRule_WithMatrixRowHasSelection_ShouldSucceed()
    {
        // Arrange
        var form = CreateTestForm();
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        var page = CreateTestPage(form.Id);
        DbContext.Pages.Add(page);
        await DbContext.SaveChangesAsync();

        var (question, matrixRows, matrixColumns) = CreateTestFormWithMatrixQuestion(page.Id);

        // Act
        var rule = new Rule
        {
            FormId = form.Id,
            SourceQuestionId = question.Id,
            Condition = RuleConstants.RowHasSelection,
            MatrixRowId = matrixRows.First().Id,
            ActionType = "Show",
            TargetQuestionId = question.Id,
            CreatedBy = TestUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _ruleRepository.CreateAsync(rule);

        // Assert
        rule.Id.Should().BeGreaterThan(0);
        rule.FormId.Should().Be(form.Id);
        rule.SourceQuestionId.Should().Be(question.Id);
        rule.Condition.Should().Be(RuleConstants.RowHasSelection);
        rule.MatrixRowId.Should().Be(matrixRows.First().Id);
    }

    [Fact]
    public async Task CreateRule_WithMatrixRowHasColumn_ShouldSucceed()
    {
        // Arrange
        var form = CreateTestForm();
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        var page = CreateTestPage(form.Id);
        DbContext.Pages.Add(page);
        await DbContext.SaveChangesAsync();

        var (question, matrixRows, matrixColumns) = CreateTestFormWithMatrixQuestion(page.Id);

        // Act
        var rule = new Rule
        {
            FormId = form.Id,
            SourceQuestionId = question.Id,
            Condition = RuleConstants.RowHasColumn,
            MatrixRowId = matrixRows.First().Id,
            MatrixColumnId = matrixColumns.First().Id,
            ActionType = "Show",
            TargetQuestionId = question.Id,
            CreatedBy = TestUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _ruleRepository.CreateAsync(rule);

        // Assert
        rule.Id.Should().BeGreaterThan(0);
        rule.FormId.Should().Be(form.Id);
        rule.SourceQuestionId.Should().Be(question.Id);
        rule.Condition.Should().Be(RuleConstants.RowHasColumn);
        rule.MatrixRowId.Should().Be(matrixRows.First().Id);
        rule.MatrixColumnId.Should().Be(matrixColumns.First().Id);
    }

    [Fact]
    public async Task CreateRule_WithMatrixColumnSelected_ShouldSucceed()
    {
        // Arrange
        var form = CreateTestForm();
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        var page = CreateTestPage(form.Id);
        DbContext.Pages.Add(page);
        await DbContext.SaveChangesAsync();

        var (question, matrixRows, matrixColumns) = CreateTestFormWithMatrixQuestion(page.Id);

        // Act
        var rule = new Rule
        {
            FormId = form.Id,
            SourceQuestionId = question.Id,
            Condition = RuleConstants.ColumnSelected,
            MatrixColumnId = matrixColumns.First().Id,
            ActionType = "Show",
            TargetQuestionId = question.Id,
            CreatedBy = TestUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _ruleRepository.CreateAsync(rule);

        // Assert
        rule.Id.Should().BeGreaterThan(0);
        rule.FormId.Should().Be(form.Id);
        rule.SourceQuestionId.Should().Be(question.Id);
        rule.Condition.Should().Be(RuleConstants.ColumnSelected);
        rule.MatrixColumnId.Should().Be(matrixColumns.First().Id);
    }

    [Fact]
    public async Task CreateRule_WithMatrixScoreGreaterThan_ShouldSucceed()
    {
        // Arrange
        var form = CreateTestForm();
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        var page = CreateTestPage(form.Id);
        DbContext.Pages.Add(page);
        await DbContext.SaveChangesAsync();

        var (question, matrixRows, matrixColumns) = CreateTestFormWithMatrixQuestion(page.Id);

        // Act
        var rule = new Rule
        {
            FormId = form.Id,
            SourceQuestionId = question.Id,
            Condition = RuleConstants.ScoreGreaterThan,
            ScoreValue = 5.0m,
            ActionType = "Show",
            TargetQuestionId = question.Id,
            CreatedBy = TestUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _ruleRepository.CreateAsync(rule);

        // Assert
        rule.Id.Should().BeGreaterThan(0);
        rule.FormId.Should().Be(form.Id);
        rule.SourceQuestionId.Should().Be(question.Id);
        rule.Condition.Should().Be(RuleConstants.ScoreGreaterThan);
        rule.ScoreValue.Should().Be(5.0m);
    }

    [Fact]
    public async Task CreateRule_WithMatrixScoreLessThan_ShouldSucceed()
    {
        // Arrange
        var form = CreateTestForm();
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        var page = CreateTestPage(form.Id);
        DbContext.Pages.Add(page);
        await DbContext.SaveChangesAsync();

        var (question, matrixRows, matrixColumns) = CreateTestFormWithMatrixQuestion(page.Id);

        // Act
        var rule = new Rule
        {
            FormId = form.Id,
            SourceQuestionId = question.Id,
            Condition = RuleConstants.ScoreLessThan,
            ScoreValue = 3.0m,
            ActionType = "Hide",
            TargetQuestionId = question.Id,
            CreatedBy = TestUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _ruleRepository.CreateAsync(rule);

        // Assert
        rule.Id.Should().BeGreaterThan(0);
        rule.FormId.Should().Be(form.Id);
        rule.SourceQuestionId.Should().Be(question.Id);
        rule.Condition.Should().Be(RuleConstants.ScoreLessThan);
        rule.ScoreValue.Should().Be(3.0m);
    }

    [Fact]
    public async Task CreateRule_WithMatrixScoreEqualTo_ShouldSucceed()
    {
        // Arrange
        var form = CreateTestForm();
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        var page = CreateTestPage(form.Id);
        DbContext.Pages.Add(page);
        await DbContext.SaveChangesAsync();

        var (question, matrixRows, matrixColumns) = CreateTestFormWithMatrixQuestion(page.Id);

        // Act
        var rule = new Rule
        {
            FormId = form.Id,
            SourceQuestionId = question.Id,
            Condition = RuleConstants.ScoreEqualTo,
            ScoreValue = 4.0m,
            ActionType = "Show",
            TargetQuestionId = question.Id,
            CreatedBy = TestUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _ruleRepository.CreateAsync(rule);

        // Assert
        rule.Id.Should().BeGreaterThan(0);
        rule.FormId.Should().Be(form.Id);
        rule.SourceQuestionId.Should().Be(question.Id);
        rule.Condition.Should().Be(RuleConstants.ScoreEqualTo);
        rule.ScoreValue.Should().Be(4.0m);
    }

    [Fact]
    public async Task CreateRule_WithMatrixScoreInRange_ShouldSucceed()
    {
        // Arrange
        var form = CreateTestForm();
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        var page = CreateTestPage(form.Id);
        DbContext.Pages.Add(page);
        await DbContext.SaveChangesAsync();

        var (question, matrixRows, matrixColumns) = CreateTestFormWithMatrixQuestion(page.Id);

        // Act
        var rule = new Rule
        {
            FormId = form.Id,
            SourceQuestionId = question.Id,
            Condition = RuleConstants.ScoreInRange,
            MinValue = 2.0m,
            MaxValue = 6.0m,
            ActionType = "Show",
            TargetQuestionId = question.Id,
            CreatedBy = TestUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _ruleRepository.CreateAsync(rule);

        // Assert
        rule.Id.Should().BeGreaterThan(0);
        rule.FormId.Should().Be(form.Id);
        rule.SourceQuestionId.Should().Be(question.Id);
        rule.Condition.Should().Be(RuleConstants.ScoreInRange);
        rule.MinValue.Should().Be(2.0m);
        rule.MaxValue.Should().Be(6.0m);
    }

    private (Question question, List<MatrixRow> matrixRows, List<MatrixColumn> matrixColumns) CreateTestFormWithMatrixQuestion(int pageId)
    {
        var question = CreateTestQuestion(pageId, QuestionType.Matrix);
        DbContext.Questions.Add(question);
        DbContext.SaveChanges();

        var matrixRows = new List<MatrixRow>
        {
            CreateTestMatrixRow(question.Id, 1, "Row 1"),
            CreateTestMatrixRow(question.Id, 2, "Row 2"),
            CreateTestMatrixRow(question.Id, 3, "Row 3")
        };

        var matrixColumns = new List<MatrixColumn>
        {
            CreateTestMatrixColumn(question.Id, 1, "Column 1"),
            CreateTestMatrixColumn(question.Id, 2, "Column 2"),
            CreateTestMatrixColumn(question.Id, 3, "Column 3")
        };

        DbContext.MatrixRow.AddRange(matrixRows);
        DbContext.MatrixColumn.AddRange(matrixColumns);
        DbContext.SaveChanges();

        return (question, matrixRows, matrixColumns);
    }
}
