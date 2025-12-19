namespace App.Domain.Enums.DynamicQuestionnaire
{
    public enum QuestionType
    {
        /* These values correspond to the IDs stored in the MasterQuestionType table */
        Multi = 1,
        Radio = 2,
        Dropdown = 3,
        Slider = 4,
        Matrix = 5,
        Text = 6,
        TextArea = 7,
        Date = 8
    }
}
