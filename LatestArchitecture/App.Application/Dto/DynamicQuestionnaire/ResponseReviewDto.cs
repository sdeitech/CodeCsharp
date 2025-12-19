namespace App.Application.Dto.DynamicQuestionnaire;

using App.Application.Dto.Common;

public class FormResponseListDto
{
    public int SubmissionId { get; set; }
    public string RespondentEmail { get; set; } = string.Empty;
    public string? RespondentName { get; set; }
    public DateTime SubmittedDate { get; set; }
    public decimal TotalScore { get; set; }
    public string FormTitle { get; set; } = string.Empty;
}

public class ResponseDetailDto
{
    public int SubmissionId { get; set; }
    public int FormId { get; set; }
    public string FormTitle { get; set; } = string.Empty;
    public string RespondentEmail { get; set; } = string.Empty;
    public string? RespondentName { get; set; }
    public DateTime SubmittedDate { get; set; }
    public decimal TotalScore { get; set; }
    public List<ResponseAnswerDto> Answers { get; set; } = new();
}

public class ResponseAnswerDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public decimal Score { get; set; }
    public List<ResponseAnswerValueDto> Values { get; set; } = new();
}

public class ResponseAnswerValueDto
{
    // For Radio, Dropdown, Multi-select answers
    public int? SelectedOptionId { get; set; }
    public string? SelectedOptionText { get; set; }
    
    // For Matrix answers
    public int? MatrixRowId { get; set; }
    public string? MatrixRowLabel { get; set; }
    public int? SelectedMatrixColumnId { get; set; }
    public string? SelectedMatrixColumnLabel { get; set; }
    
    // For Text-based answers
    public string? TextValue { get; set; }
    
    // For Slider answers
    public decimal? NumericValue { get; set; }
}

public class ResponseFilterDto : FilterDto
{
    public string? RespondentEmail { get; set; }
    public DateTime? SubmittedDateFrom { get; set; }
    public DateTime? SubmittedDateTo { get; set; }
    // Sorting/paging/search come from base FilterDto (SortColumn, SortOrder, PageNumber, PageSize, SearchTerm)
}
