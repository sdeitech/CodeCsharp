namespace App.Application.Dto.DynamicQuestionnaire;

public class CreateFormDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public bool AllowResubmission { get; set; } = false;
    public List<CreatePageDto> Pages { get; set; } = new List<CreatePageDto>();
}

public class CreatePageDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int PageOrder { get; set; }
    public List<CreateQuestionDto> Questions { get; set; } = new List<CreateQuestionDto>();
}

public class CreateQuestionDto
{
    public int QuestionTypeId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = false;
    public int QuestionOrder { get; set; }
    public List<CreateOptionDto> Options { get; set; } = new List<CreateOptionDto>();
    
    // Matrix configuration (for questionTypeId = 5)
    public CreateMatrixConfigDto? MatrixConfig { get; set; }
    
    // Slider configuration (for questionTypeId = 4)
    public CreateSliderConfigDto? SliderConfig { get; set; }
}

public class CreateOptionDto
{
    public string? OptionText { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public decimal Score { get; set; } = 0;
}

public class CreateMatrixConfigDto
{
    public List<CreateMatrixRowDto> Rows { get; set; } = new List<CreateMatrixRowDto>();
    public List<CreateMatrixColumnDto> Columns { get; set; } = new List<CreateMatrixColumnDto>();
}

public class CreateMatrixRowDto
{
    public string RowLabel { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public class CreateMatrixColumnDto
{
    public string ColumnLabel { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public decimal Score { get; set; } = 0;
}

public class CreateSliderConfigDto
{
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public int StepValue { get; set; } = 1;
    public string? MinLabel { get; set; }
    public string? MaxLabel { get; set; }
}
