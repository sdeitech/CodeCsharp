namespace App.Application.Dto.DynamicQuestionnaire;

public class FormResponseDto
{
    public int FormId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public bool IsPublished { get; set; }
    public bool AllowResubmission { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedDate { get; set; }
    public string? PublicKey { get; set; }
    public string? PublicURL { get; set; }
    public List<PageResponseDto> Pages { get; set; } = new List<PageResponseDto>();
}

public class PageResponseDto
{
    public int PageId { get; set; }
    public int FormId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int PageOrder { get; set; }
    public List<QuestionResponseDto> Questions { get; set; } = new List<QuestionResponseDto>();
}

public class QuestionResponseDto
{
    public int QuestionId { get; set; }
    public int PageId { get; set; }
    public int QuestionTypeId { get; set; }
    public string QuestionTypeName { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int QuestionOrder { get; set; }
    public List<OptionResponseDto> Options { get; set; } = new List<OptionResponseDto>();
    
    // Slider configuration (for questionTypeId = 4)
    public SliderConfigResponseDto? SliderConfig { get; set; }
    
    // Matrix configuration (for questionTypeId = 5)
    public List<MatrixRowResponseDto> MatrixRows { get; set; } = new List<MatrixRowResponseDto>();
    public List<MatrixColumnResponseDto> MatrixColumns { get; set; } = new List<MatrixColumnResponseDto>();
}

public class OptionResponseDto
{
    public int OptionId { get; set; }
    public int QuestionId { get; set; }
    public string? OptionText { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public decimal Score { get; set; }
}

public class SliderConfigResponseDto
{
    public int SliderConfigId { get; set; }
    public int QuestionId { get; set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public int StepValue { get; set; }
    public string? MinLabel { get; set; }
    public string? MaxLabel { get; set; }
}

public class MatrixRowResponseDto
{
    public int MatrixRowId { get; set; }
    public int QuestionId { get; set; }
    public string RowLabel { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public class MatrixColumnResponseDto
{
    public int MatrixColumnId { get; set; }
    public int QuestionId { get; set; }
    public string ColumnLabel { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public decimal Score { get; set; }
}

public class MasterQuestionTypeDto
{
    public int QuestionTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
