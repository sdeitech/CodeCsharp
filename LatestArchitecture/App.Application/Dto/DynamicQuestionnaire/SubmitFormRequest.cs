namespace App.Application.Dto.DynamicQuestionnaire;

public class SubmitFormRequest
{
    public string RespondentEmail { get; set; } = string.Empty;
    public string? RespondentName { get; set; }
    public List<SubmissionAnswerDto> Answers { get; set; } = new();
}

public class SubmissionAnswerDto
{
    public int QuestionId { get; set; }
    public List<AnswerValueDto> Values { get; set; } = new();
}

public class AnswerValueDto
{
    // For Radio, Dropdown, Multi-select answers
    public int? SelectedOptionId { get; set; }
    
    // For Matrix answers (identifies the row)
    public int? MatrixRowId { get; set; }
    
    // For Matrix answers (identifies the selected column for a given row)  
    public int? SelectedMatrixColumnId { get; set; }
    
    // For Text-based answers
    public string? TextValue { get; set; }
    
    // For Slider answers
    public decimal? NumericValue { get; set; }
}

public class SubmissionResponseDto
{
    public int SubmissionId { get; set; }
    public string RespondentEmail { get; set; } = string.Empty;
    public string? RespondentName { get; set; }
    public DateTime SubmittedDate { get; set; }
    public decimal TotalScore { get; set; }
    public string Message { get; set; } = string.Empty;
}
