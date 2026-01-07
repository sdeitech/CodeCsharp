namespace App.Application.Dto.DynamicQuestionnaire;

public class PublicFormResponseDto
{
    public int FormId { get; set; }
    public string PublicKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public bool AllowResubmission { get; set; }
    public List<PublicPageDto> Pages { get; set; } = new List<PublicPageDto>();
}

public class PublicPageDto
{
    public int PageId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int PageOrder { get; set; }
    public List<PublicQuestionDto> Questions { get; set; } = new List<PublicQuestionDto>();
}

public class PublicQuestionDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int QuestionOrder { get; set; }
    public string QuestionType { get; set; } = string.Empty;
    public List<PublicOptionDto>? Options { get; set; }
    public PublicSliderConfigDto? SliderConfig { get; set; }
    public List<PublicMatrixRowDto>? MatrixRows { get; set; }
    public List<PublicMatrixColumnDto>? MatrixColumns { get; set; }
}

public class PublicOptionDto
{
    public int OptionId { get; set; }
    public string? OptionText { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class PublicSliderConfigDto
{
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public int StepValue { get; set; }
    public string? MinLabel { get; set; }
    public string? MaxLabel { get; set; }
}

public class PublicMatrixRowDto
{
    public int MatrixRowId { get; set; }
    public string RowLabel { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public class PublicMatrixColumnDto
{
    public int MatrixColumnId { get; set; }
    public string ColumnLabel { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
