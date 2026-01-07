namespace App.Application.Constant.DynamicQuestionnaire;

/// <summary>
/// Shared constants for rule validation and evaluation (dynamic questionnaire)
/// </summary>
public static class RuleConstants
{
    // Condition types
    public const string IsSelected = "IsSelected";
    public const string IsNotSelected = "IsNotSelected";
    public const string IsGreaterThan = "IsGreaterThan";
    public const string IsLessThan = "IsLessThan";
    public const string IsEqualTo = "IsEqualTo";
    public const string IsNotEqualTo = "IsNotEqualTo";
    public const string IsInRange = "IsInRange";
    
    // Matrix-specific condition types
    public const string RowHasSelection = "RowHasSelection";
    public const string RowHasColumn = "RowHasColumn";
    public const string ColumnSelected = "ColumnSelected";
    public const string ScoreGreaterThan = "ScoreGreaterThan";
    public const string ScoreLessThan = "ScoreLessThan";
    public const string ScoreEqualTo = "ScoreEqualTo";
    public const string ScoreInRange = "ScoreInRange";

    // Action types
    public const string HideQuestion = "HideQuestion";
    public const string ShowQuestion = "ShowQuestion";
    public const string SkipToPage = "SkipToPage";
    public const string TerminateForm = "TerminateForm";

    // Question types
    public const string Multi = "Multi";
    public const string Radio = "Radio";
    public const string Dropdown = "Dropdown";
    public const string Slider = "Slider";
    public const string Text = "Text";
    public const string TextArea = "TextArea";
    public const string Date = "Date";
    public const string Matrix = "Matrix";

    public static readonly string[] ValidConditions = 
    {
        IsSelected, IsNotSelected, IsGreaterThan, IsLessThan, IsEqualTo, IsNotEqualTo, IsInRange,
        RowHasSelection, RowHasColumn, ColumnSelected, ScoreGreaterThan, ScoreLessThan, ScoreEqualTo, ScoreInRange
    };

    public static readonly string[] ValidActionTypes = 
    {
        HideQuestion, ShowQuestion, SkipToPage, TerminateForm
    };

    public static readonly string[] ChoiceBasedQuestionTypes = 
    {
        Multi, Radio, Dropdown
    };

    public static readonly string[] ValueBasedQuestionTypes = 
    {
        Slider
    };

    public static readonly string[] MatrixQuestionTypes = 
    {
        Matrix
    };

    public static readonly string[] ChoiceBasedConditions = 
    {
        IsSelected, IsNotSelected
    };

    public static readonly string[] ValueBasedConditions = 
    {
        IsGreaterThan, IsLessThan, IsEqualTo, IsNotEqualTo, IsInRange
    };

    public static readonly string[] MatrixConditions = 
    {
        RowHasSelection, RowHasColumn, ColumnSelected, ScoreGreaterThan, ScoreLessThan, ScoreEqualTo, ScoreInRange
    };

    public static readonly string[] ActionsRequiringTargetQuestion = 
    {
        HideQuestion, ShowQuestion
    };

    public static readonly string[] ActionsRequiringTargetPage = 
    {
        SkipToPage
    };
}
