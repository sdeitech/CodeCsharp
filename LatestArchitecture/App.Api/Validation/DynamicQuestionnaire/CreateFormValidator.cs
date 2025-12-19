using App.Application.Dto.DynamicQuestionnaire;
using App.Domain.Enums.DynamicQuestionnaire;
using FluentValidation;

namespace App.Api.Validation.DynamicQuestionnaire;

public class CreateFormValidator : AbstractValidator<CreateFormDto>
{
    public CreateFormValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Form title is required")
            .MaximumLength(255)
            .WithMessage("Form title cannot exceed 255 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Form description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Pages)
            .NotEmpty()
            .WithMessage("Form must have at least one page");

        RuleForEach(x => x.Pages)
            .SetValidator(new CreatePageValidator());

        // Check for duplicate page orders
        RuleFor(x => x.Pages)
            .Must(HaveUniquePageOrders)
            .WithMessage("Page orders must be unique");
    }

    private bool HaveUniquePageOrders(List<CreatePageDto> pages)
    {
        if (pages == null || !pages.Any()) return true;
        
        var pageOrders = pages.Select(p => p.PageOrder).ToList();
        return pageOrders.Count == pageOrders.Distinct().Count();
    }
}

public class CreatePageValidator : AbstractValidator<CreatePageDto>
{
    public CreatePageValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(255)
            .WithMessage("Page title cannot exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Page description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.PageOrder)
            .GreaterThan(0)
            .WithMessage("Page order must be greater than 0");

        RuleFor(x => x.Questions)
            .NotEmpty()
            .WithMessage("Page must have at least one question");

        RuleForEach(x => x.Questions)
            .SetValidator(new CreateQuestionValidator());

        // Check for duplicate question orders within a page
        RuleFor(x => x.Questions)
            .Must(HaveUniqueQuestionOrders)
            .WithMessage("Question orders must be unique within a page");
    }

    private bool HaveUniqueQuestionOrders(List<CreateQuestionDto> questions)
    {
        if (questions == null || !questions.Any()) return true;
        
        var questionOrders = questions.Select(q => q.QuestionOrder).ToList();
        return questionOrders.Count == questionOrders.Distinct().Count();
    }
}

public class CreateQuestionValidator : AbstractValidator<CreateQuestionDto>
{
    private const int MatrixQuestionTypeId = (Int32)QuestionType.Matrix;
    private const int SliderQuestionTypeId = (Int32)QuestionType.Slider;

    public CreateQuestionValidator()
    {
        RuleFor(x => x.QuestionTypeId)
            .Must(id => Enum.IsDefined(typeof(QuestionType), id))
            .WithMessage("Invalid question type ID.");

        RuleFor(x => x.QuestionText)
            .NotEmpty()
            .WithMessage("Question text is required")
            .MaximumLength(1000)
            .WithMessage("Question text cannot exceed 1000 characters");

        RuleFor(x => x.QuestionOrder)
            .GreaterThan(0)
            .WithMessage("Question order must be greater than 0");

        // Matrix Question Validation
        RuleFor(x => x.MatrixConfig)
            .NotNull()
            .WithMessage("Matrix configuration is required for Matrix questions")
            .When(x => x.QuestionTypeId == MatrixQuestionTypeId);

        RuleFor(x => x.MatrixConfig)
            .SetValidator(new CreateMatrixConfigValidator()!)
            .When(x => x.QuestionTypeId == MatrixQuestionTypeId && x.MatrixConfig != null);

        // Slider Question Validation
        RuleFor(x => x.SliderConfig)
            .NotNull()
            .WithMessage("Slider configuration is required for Slider questions")
            .When(x => x.QuestionTypeId == SliderQuestionTypeId);

        RuleFor(x => x.SliderConfig)
            .SetValidator(new CreateSliderConfigValidator()!)
            .When(x => x.QuestionTypeId == SliderQuestionTypeId && x.SliderConfig != null);

        // Matrix and Slider questions should not have Options
        RuleFor(x => x.Options)
            .Must(options => options == null || !options.Any())
            .WithMessage("Matrix and Slider questions should not have options")
            .When(x => x.QuestionTypeId == MatrixQuestionTypeId || x.QuestionTypeId == SliderQuestionTypeId);

        // Non-Matrix/Slider questions should not have Matrix/Slider config
        RuleFor(x => x.MatrixConfig)
            .Must(config => config == null)
            .WithMessage("Matrix configuration should only be provided for Matrix questions")
            .When(x => x.QuestionTypeId != MatrixQuestionTypeId);

        RuleFor(x => x.SliderConfig)
            .Must(config => config == null)
            .WithMessage("Slider configuration should only be provided for Slider questions")
            .When(x => x.QuestionTypeId != SliderQuestionTypeId);

        // Options validation for non-Matrix/Slider questions
        RuleForEach(x => x.Options)
            .SetValidator(new CreateOptionValidator())
            .When(x => x.Options != null && x.Options.Any() && 
                      x.QuestionTypeId != MatrixQuestionTypeId && 
                      x.QuestionTypeId != SliderQuestionTypeId);

        // Check for duplicate option display orders within a question
        RuleFor(x => x.Options)
            .Must(HaveUniqueDisplayOrders)
            .WithMessage("Option display orders must be unique within a question")
            .When(x => x.Options != null && x.Options.Any() && 
                      x.QuestionTypeId != MatrixQuestionTypeId && 
                      x.QuestionTypeId != SliderQuestionTypeId);
    }

    private static bool HaveUniqueDisplayOrders(List<CreateOptionDto> options)
    {
        if (options == null || !options.Any()) return true;
        
        var displayOrders = options.Select(o => o.DisplayOrder).ToList();
        return displayOrders.Count == displayOrders.Distinct().Count();
    }
}

public class CreateMatrixConfigValidator : AbstractValidator<CreateMatrixConfigDto>
{
    public CreateMatrixConfigValidator()
    {
        RuleFor(x => x.Rows)
            .NotEmpty()
            .WithMessage("Matrix questions must have at least one row");

        RuleFor(x => x.Columns)
            .NotEmpty()
            .WithMessage("Matrix questions must have at least one column");

        RuleForEach(x => x.Rows)
            .SetValidator(new CreateMatrixRowValidator());

        RuleForEach(x => x.Columns)
            .SetValidator(new CreateMatrixColumnValidator());

        // Check for unique row display orders
        RuleFor(x => x.Rows)
            .Must(HaveUniqueRowDisplayOrders)
            .WithMessage("Matrix row display orders must be unique");

        // Check for unique column display orders
        RuleFor(x => x.Columns)
            .Must(HaveUniqueColumnDisplayOrders)
            .WithMessage("Matrix column display orders must be unique");
    }

    private static bool HaveUniqueRowDisplayOrders(List<CreateMatrixRowDto> rows)
    {
        if (rows == null || !rows.Any()) return true;
        var displayOrders = rows.Select(r => r.DisplayOrder).ToList();
        return displayOrders.Count == displayOrders.Distinct().Count();
    }

    private static bool HaveUniqueColumnDisplayOrders(List<CreateMatrixColumnDto> columns)
    {
        if (columns == null || !columns.Any()) return true;
        var displayOrders = columns.Select(c => c.DisplayOrder).ToList();
        return displayOrders.Count == displayOrders.Distinct().Count();
    }
}

public class CreateMatrixRowValidator : AbstractValidator<CreateMatrixRowDto>
{
    public CreateMatrixRowValidator()
    {
        RuleFor(x => x.RowLabel)
            .NotEmpty()
            .WithMessage("Matrix row label is required")
            .MaximumLength(500)
            .WithMessage("Matrix row label cannot exceed 500 characters");

        RuleFor(x => x.DisplayOrder)
            .GreaterThan(0)
            .WithMessage("Matrix row display order must be greater than 0");
    }
}

public class CreateMatrixColumnValidator : AbstractValidator<CreateMatrixColumnDto>
{
    public CreateMatrixColumnValidator()
    {
        RuleFor(x => x.ColumnLabel)
            .NotEmpty()
            .WithMessage("Matrix column label is required")
            .MaximumLength(500)
            .WithMessage("Matrix column label cannot exceed 500 characters");

        RuleFor(x => x.DisplayOrder)
            .GreaterThan(0)
            .WithMessage("Matrix column display order must be greater than 0");

        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Matrix column score must be greater than or equal to 0");
    }
}

public class CreateSliderConfigValidator : AbstractValidator<CreateSliderConfigDto>
{
    public CreateSliderConfigValidator()
    {
        RuleFor(x => x.MinValue)
            .LessThan(x => x.MaxValue)
            .WithMessage("Slider minimum value must be less than maximum value");

        RuleFor(x => x.MaxValue)
            .GreaterThan(x => x.MinValue)
            .WithMessage("Slider maximum value must be greater than minimum value");

        RuleFor(x => x.StepValue)
            .GreaterThan(0)
            .WithMessage("Slider step value must be greater than 0");

        RuleFor(x => x.StepValue)
            .Must((config, step) => (config.MaxValue - config.MinValue) % step == 0)
            .WithMessage("Slider step value must divide evenly into the range (max - min)")
            .When(x => x.StepValue > 0);

        RuleFor(x => x.MinLabel)
            .MaximumLength(100)
            .WithMessage("Slider minimum label cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.MinLabel));

        RuleFor(x => x.MaxLabel)
            .MaximumLength(100)
            .WithMessage("Slider maximum label cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.MaxLabel));
    }
}

public class CreateOptionValidator : AbstractValidator<CreateOptionDto>
{
    public CreateOptionValidator()
    {
        RuleFor(x => x.OptionText)
            .MaximumLength(500)
            .WithMessage("Option text cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.OptionText));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(2048)
            .WithMessage("Image URL cannot exceed 2048 characters")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.DisplayOrder)
            .GreaterThan(0)
            .WithMessage("Display order must be greater than 0");

        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Score must be greater than or equal to 0");

        // Either option text or image URL must be provided
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.OptionText) || !string.IsNullOrEmpty(x.ImageUrl))
            .WithMessage("Either option text or image URL must be provided");
    }
}
