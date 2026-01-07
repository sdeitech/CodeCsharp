using App.Application.Dto.DynamicQuestionnaire;
using FluentValidation;

namespace App.Api.Validation.DynamicQuestionnaire;

public class SubmitFormValidator : AbstractValidator<SubmitFormRequest>
{
    public SubmitFormValidator()
    {
        RuleFor(x => x.RespondentEmail)
            .NotEmpty()
            .WithMessage("Respondent email is required")
            .EmailAddress()
            .WithMessage("Please provide a valid email address")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.RespondentName)
            .MaximumLength(255)
            .WithMessage("Respondent name cannot exceed 255 characters");

        RuleFor(x => x.Answers)
            .NotNull()
            .WithMessage("Answers are required")
            .Must(answers => answers.Count > 0)
            .WithMessage("At least one answer is required");

        RuleForEach(x => x.Answers).SetValidator(new SubmissionAnswerValidator());
    }
}

public class SubmissionAnswerValidator : AbstractValidator<SubmissionAnswerDto>
{
    public SubmissionAnswerValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0)
            .WithMessage("Question ID must be greater than 0");

        RuleFor(x => x.Values)
            .NotNull()
            .WithMessage("Answer values are required")
            .Must(values => values.Count > 0)
            .WithMessage("At least one answer value is required");

        RuleForEach(x => x.Values).SetValidator(new AnswerValueValidator());
    }
}

public class AnswerValueValidator : AbstractValidator<AnswerValueDto>
{
    public AnswerValueValidator()
    {
        RuleFor(x => x)
            .Must(HaveAtLeastOneValue)
            .WithMessage("Answer value must have at least one field filled");

        RuleFor(x => x.TextValue)
            .MaximumLength(4000)
            .WithMessage("Text value cannot exceed 4000 characters")
            .When(x => !string.IsNullOrEmpty(x.TextValue));

        RuleFor(x => x.SelectedOptionId)
            .GreaterThan(0)
            .WithMessage("Selected option ID must be greater than 0")
            .When(x => x.SelectedOptionId.HasValue);

        RuleFor(x => x.MatrixRowId)
            .GreaterThan(0)
            .WithMessage("Matrix row ID must be greater than 0")
            .When(x => x.MatrixRowId.HasValue);

        RuleFor(x => x.SelectedMatrixColumnId)
            .GreaterThan(0)
            .WithMessage("Selected matrix column ID must be greater than 0")
            .When(x => x.SelectedMatrixColumnId.HasValue);
    }

    private static bool HaveAtLeastOneValue(AnswerValueDto value)
    {
        return value.SelectedOptionId.HasValue ||
               value.MatrixRowId.HasValue ||
               value.SelectedMatrixColumnId.HasValue ||
               !string.IsNullOrWhiteSpace(value.TextValue) ||
               value.NumericValue.HasValue;
    }
}
