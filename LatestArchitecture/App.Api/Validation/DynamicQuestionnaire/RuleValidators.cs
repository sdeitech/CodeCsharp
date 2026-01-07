using App.Application.Dto.DynamicQuestionnaire;
using FluentValidation;
using App.Application.Constant.DynamicQuestionnaire;

namespace App.Api.Validation.DynamicQuestionnaire;

/// <summary>
/// Validator for creating conditional logic rules
/// </summary>
public class CreateRuleValidator : AbstractValidator<CreateRuleRequest>
{
    public CreateRuleValidator()
    {
        // Basic field validations
        RuleFor(x => x.FormId)
            .GreaterThan(0)
            .WithMessage("FormId must be greater than 0");

        RuleFor(x => x.SourceQuestionId)
            .GreaterThan(0)
            .WithMessage("SourceQuestionId must be greater than 0");

        RuleFor(x => x.Condition)
            .NotEmpty()
            .WithMessage("Condition is required")
            .Must(BeValidCondition)
            .WithMessage($"Condition must be one of: {string.Join(", ", RuleConstants.ValidConditions)}");

        RuleFor(x => x.ActionType)
            .NotEmpty()
            .WithMessage("ActionType is required")
            .Must(BeValidActionType)
            .WithMessage($"ActionType must be one of: {string.Join(", ", RuleConstants.ValidActionTypes)}");

        // Choice-based condition validations (Radio, Multi, Dropdown)
        RuleFor(x => x.TriggerOptionId)
            .GreaterThan(0)
            .When(x => RuleConstants.ChoiceBasedConditions.Contains(x.Condition))
            .WithMessage("TriggerOptionId is required for IsSelected and IsNotSelected conditions");

        RuleFor(x => x.TriggerOptionId)
            .Empty()
            .When(x => RuleConstants.ValueBasedConditions.Contains(x.Condition))
            .WithMessage("TriggerOptionId must be null for value-based conditions (IsGreaterThan, IsLessThan, etc.)");

        // Value-based condition validations (Slider questions)
        RuleFor(x => x.MinValue)
            .NotNull()
            .When(x => x.Condition == RuleConstants.IsGreaterThan || 
                      x.Condition == RuleConstants.IsLessThan || 
                      x.Condition == RuleConstants.IsEqualTo || 
                      x.Condition == RuleConstants.IsNotEqualTo)
            .WithMessage("MinValue is required for IsGreaterThan, IsLessThan, IsEqualTo, and IsNotEqualTo conditions");

        RuleFor(x => x.MinValue)
            .NotNull()
            .When(x => x.Condition == RuleConstants.IsInRange)
            .WithMessage("MinValue is required for IsInRange condition");

        RuleFor(x => x.MaxValue)
            .NotNull()
            .When(x => x.Condition == RuleConstants.IsInRange)
            .WithMessage("MaxValue is required for IsInRange condition");

        RuleFor(x => x.MaxValue)
            .GreaterThan(x => x.MinValue)
            .When(x => x.Condition == RuleConstants.IsInRange && x.MinValue.HasValue && x.MaxValue.HasValue)
            .WithMessage("MaxValue must be greater than MinValue for IsInRange condition");

        RuleFor(x => x.MinValue)
            .Empty()
            .When(x => RuleConstants.ChoiceBasedConditions.Contains(x.Condition))
            .WithMessage("MinValue must be null for choice-based conditions (IsSelected, IsNotSelected)");

        RuleFor(x => x.MaxValue)
            .Empty()
            .When(x => x.Condition != RuleConstants.IsInRange)
            .WithMessage("MaxValue should only be specified for IsInRange condition");

        // Action-specific validations
        RuleFor(x => x.TargetQuestionId)
            .GreaterThan(0)
            .When(x => RuleConstants.ActionsRequiringTargetQuestion.Contains(x.ActionType))
            .WithMessage("TargetQuestionId is required for HideQuestion and ShowQuestion actions");

        RuleFor(x => x.TargetQuestionId)
            .Empty()
            .When(x => !RuleConstants.ActionsRequiringTargetQuestion.Contains(x.ActionType))
            .WithMessage("TargetQuestionId should only be specified for HideQuestion and ShowQuestion actions");

        RuleFor(x => x.TargetPageId)
            .GreaterThan(0)
            .When(x => RuleConstants.ActionsRequiringTargetPage.Contains(x.ActionType))
            .WithMessage("TargetPageId is required for SkipToPage action");

        RuleFor(x => x.TargetPageId)
            .Empty()
            .When(x => !RuleConstants.ActionsRequiringTargetPage.Contains(x.ActionType))
            .WithMessage("TargetPageId should only be specified for SkipToPage action");

        // Business logic validations
        RuleFor(x => x)
            .Must(x => x.TargetQuestionId == null || x.SourceQuestionId != x.TargetQuestionId.Value)
            .When(x => x.TargetQuestionId.HasValue)
            .WithMessage("Source question cannot be the same as target question");

        // Value range validations for common slider ranges
        RuleFor(x => x.MinValue)
            .GreaterThanOrEqualTo(-999999)
            .LessThanOrEqualTo(999999)
            .When(x => x.MinValue.HasValue)
            .WithMessage("MinValue must be between -999999 and 999999");

        RuleFor(x => x.MaxValue)
            .GreaterThanOrEqualTo(-999999)
            .LessThanOrEqualTo(999999)
            .When(x => x.MaxValue.HasValue)
            .WithMessage("MaxValue must be between -999999 and 999999");

        // Precision validation for decimal values
        RuleFor(x => x.MinValue)
            .Must(HaveReasonablePrecision)
            .When(x => x.MinValue.HasValue)
            .WithMessage("MinValue should not have more than 4 decimal places");

        RuleFor(x => x.MaxValue)
            .Must(HaveReasonablePrecision)
            .When(x => x.MaxValue.HasValue)
            .WithMessage("MaxValue should not have more than 4 decimal places");

        // Custom validation for range logic
        RuleFor(x => x)
            .Must(HaveValidRangeLogic)
            .WithMessage("For IsInRange condition, MinValue must be less than MaxValue");
    }

    private static bool BeValidCondition(string condition)
    {
        return RuleConstants.ValidConditions.Contains(condition);
    }

    private static bool BeValidActionType(string actionType)
    {
        return RuleConstants.ValidActionTypes.Contains(actionType);
    }

    private static bool HaveReasonablePrecision(decimal? value)
    {
        if (!value.HasValue) return true;
        
        var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(value.Value)[3])[2];
        return decimalPlaces <= 4;
    }

    private static bool HaveValidRangeLogic(CreateRuleRequest request)
    {
        if (request.Condition != RuleConstants.IsInRange) return true;
        if (!request.MinValue.HasValue || !request.MaxValue.HasValue) return true;
        
        return request.MinValue.Value < request.MaxValue.Value;
    }
}
/// <summary>
/// Validator for updating conditional logic rules
/// </summary>
public class UpdateRuleValidator : AbstractValidator<UpdateRuleRequest>
{
    public UpdateRuleValidator()
    {
        // Basic field validations
        RuleFor(x => x.RuleId)
            .GreaterThan(0)
            .WithMessage("RuleId must be greater than 0");

        RuleFor(x => x.Condition)
            .NotEmpty()
            .WithMessage("Condition is required")
            .Must(BeValidCondition)
            .WithMessage($"Condition must be one of: {string.Join(", ", RuleConstants.ValidConditions)}");

        RuleFor(x => x.ActionType)
            .NotEmpty()
            .WithMessage("ActionType is required")
            .Must(BeValidActionType)
            .WithMessage($"ActionType must be one of: {string.Join(", ", RuleConstants.ValidActionTypes)}");

        // Choice-based condition validations (Radio, Multi, Dropdown)
        RuleFor(x => x.TriggerOptionId)
            .GreaterThan(0)
            .When(x => RuleConstants.ChoiceBasedConditions.Contains(x.Condition))
            .WithMessage("TriggerOptionId is required for IsSelected and IsNotSelected conditions");

        RuleFor(x => x.TriggerOptionId)
            .Empty()
            .When(x => RuleConstants.ValueBasedConditions.Contains(x.Condition))
            .WithMessage("TriggerOptionId must be null for value-based conditions (IsGreaterThan, IsLessThan, etc.)");

        // Value-based condition validations (Slider questions)
        RuleFor(x => x.MinValue)
            .NotNull()
            .When(x => x.Condition == RuleConstants.IsGreaterThan || 
                      x.Condition == RuleConstants.IsLessThan || 
                      x.Condition == RuleConstants.IsEqualTo || 
                      x.Condition == RuleConstants.IsNotEqualTo)
            .WithMessage("MinValue is required for IsGreaterThan, IsLessThan, IsEqualTo, and IsNotEqualTo conditions");

        RuleFor(x => x.MinValue)
            .NotNull()
            .When(x => x.Condition == RuleConstants.IsInRange)
            .WithMessage("MinValue is required for IsInRange condition");

        RuleFor(x => x.MaxValue)
            .NotNull()
            .When(x => x.Condition == RuleConstants.IsInRange)
            .WithMessage("MaxValue is required for IsInRange condition");

        RuleFor(x => x.MaxValue)
            .GreaterThan(x => x.MinValue)
            .When(x => x.Condition == RuleConstants.IsInRange && x.MinValue.HasValue && x.MaxValue.HasValue)
            .WithMessage("MaxValue must be greater than MinValue for IsInRange condition");

        RuleFor(x => x.MinValue)
            .Empty()
            .When(x => RuleConstants.ChoiceBasedConditions.Contains(x.Condition))
            .WithMessage("MinValue must be null for choice-based conditions (IsSelected, IsNotSelected)");

        RuleFor(x => x.MaxValue)
            .Empty()
            .When(x => x.Condition != RuleConstants.IsInRange)
            .WithMessage("MaxValue should only be specified for IsInRange condition");

        // Action-specific validations
        RuleFor(x => x.TargetQuestionId)
            .GreaterThan(0)
            .When(x => RuleConstants.ActionsRequiringTargetQuestion.Contains(x.ActionType))
            .WithMessage("TargetQuestionId is required for HideQuestion and ShowQuestion actions");

        RuleFor(x => x.TargetQuestionId)
            .Empty()
            .When(x => !RuleConstants.ActionsRequiringTargetQuestion.Contains(x.ActionType))
            .WithMessage("TargetQuestionId should only be specified for HideQuestion and ShowQuestion actions");

        RuleFor(x => x.TargetPageId)
            .GreaterThan(0)
            .When(x => RuleConstants.ActionsRequiringTargetPage.Contains(x.ActionType))
            .WithMessage("TargetPageId is required for SkipToPage action");

        RuleFor(x => x.TargetPageId)
            .Empty()
            .When(x => !RuleConstants.ActionsRequiringTargetPage.Contains(x.ActionType))
            .WithMessage("TargetPageId should only be specified for SkipToPage action");

        // Value range validations for common slider ranges
        RuleFor(x => x.MinValue)
            .GreaterThanOrEqualTo(-999999)
            .LessThanOrEqualTo(999999)
            .When(x => x.MinValue.HasValue)
            .WithMessage("MinValue must be between -999999 and 999999");

        RuleFor(x => x.MaxValue)
            .GreaterThanOrEqualTo(-999999)
            .LessThanOrEqualTo(999999)
            .When(x => x.MaxValue.HasValue)
            .WithMessage("MaxValue must be between -999999 and 999999");

        // Precision validation for decimal values
        RuleFor(x => x.MinValue)
            .Must(HaveReasonablePrecision)
            .When(x => x.MinValue.HasValue)
            .WithMessage("MinValue should not have more than 4 decimal places");

        RuleFor(x => x.MaxValue)
            .Must(HaveReasonablePrecision)
            .When(x => x.MaxValue.HasValue)
            .WithMessage("MaxValue should not have more than 4 decimal places");

        // Custom validation for range logic
        RuleFor(x => x)
            .Must(HaveValidRangeLogic)
            .WithMessage("For IsInRange condition, MinValue must be less than MaxValue");
    }

    private static bool BeValidCondition(string condition)
    {
        return RuleConstants.ValidConditions.Contains(condition);
    }

    private static bool BeValidActionType(string actionType)
    {
        return RuleConstants.ValidActionTypes.Contains(actionType);
    }

    private static bool HaveReasonablePrecision(decimal? value)
    {
        if (!value.HasValue) return true;
        
        var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(value.Value)[3])[2];
        return decimalPlaces <= 4;
    }

    private static bool HaveValidRangeLogic(UpdateRuleRequest request)
    {
        if (request.Condition != RuleConstants.IsInRange) return true;
        if (!request.MinValue.HasValue || !request.MaxValue.HasValue) return true;
        
        return request.MinValue.Value < request.MaxValue.Value;
    }
}
