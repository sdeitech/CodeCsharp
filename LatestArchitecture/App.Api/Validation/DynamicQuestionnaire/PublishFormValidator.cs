using App.Application.Dto.DynamicQuestionnaire;
using FluentValidation;

namespace App.Api.Validation.DynamicQuestionnaire;

public class PublishFormValidator : AbstractValidator<PublishFormRequest>
{
    public PublishFormValidator()
    {
        RuleFor(x => x.FormId)
            .GreaterThan(0)
            .WithMessage("Form ID must be greater than 0");
    }
}
