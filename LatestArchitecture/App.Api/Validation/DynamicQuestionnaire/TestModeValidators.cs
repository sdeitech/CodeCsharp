using FluentValidation;
using App.Application.Dto.DynamicQuestionnaire;

namespace App.Api.Validation.DynamicQuestionnaire;

/// <summary>
/// Validator for test mode submission requests
/// </summary>
public class TestModeSubmissionValidator : AbstractValidator<TestModeSubmissionRequest>
{
    public TestModeSubmissionValidator()
    {
        RuleFor(x => x.FormId)
            .GreaterThan(0)
            .WithMessage("FormId must be greater than 0");

        RuleFor(x => x.Answers)
            .NotNull()
            .WithMessage("Answers cannot be null");

        RuleForEach(x => x.Answers)
            .SetValidator(new TestModeAnswerValidator());
    }
}

/// <summary>
/// Validator for test mode answers
/// </summary>
public class TestModeAnswerValidator : AbstractValidator<TestModeAnswer>
{
    public TestModeAnswerValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0)
            .WithMessage("QuestionId must be greater than 0");

        RuleFor(x => x.SelectedOptionIds)
            .NotNull()
            .WithMessage("SelectedOptionIds cannot be null");

        RuleFor(x => x.MatrixAnswers)
            .NotNull()
            .WithMessage("MatrixAnswers cannot be null");

        RuleForEach(x => x.MatrixAnswers)
            .SetValidator(new TestModeMatrixAnswerValidator());
    }
}

/// <summary>
/// Validator for test mode matrix answers
/// </summary>
public class TestModeMatrixAnswerValidator : AbstractValidator<TestModeMatrixAnswer>
{
    public TestModeMatrixAnswerValidator()
    {
        RuleFor(x => x.MatrixRowId)
            .GreaterThan(0)
            .WithMessage("MatrixRowId must be greater than 0");

        RuleFor(x => x.SelectedColumnId)
            .GreaterThan(0)
            .WithMessage("SelectedColumnId must be greater than 0");
    }
}
