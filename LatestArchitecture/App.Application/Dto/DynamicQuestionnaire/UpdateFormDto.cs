namespace App.Application.Dto.DynamicQuestionnaire;

public class UpdateFormDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public bool AllowResubmission { get; set; } = false;
    public List<UpdatePageDto> Pages { get; set; } = new List<UpdatePageDto>();
}

public class UpdatePageDto
{
    public int? Id { get; set; } // Include ID for existing pages, null for new pages
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int PageOrder { get; set; }
    public List<UpdateQuestionDto> Questions { get; set; } = new List<UpdateQuestionDto>();
}

public class UpdateQuestionDto
{
    public int? Id { get; set; } // Include ID for existing questions, null for new questions
    public int QuestionTypeId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public bool IsRequired { get; set; } = false;
    public int QuestionOrder { get; set; }
    public List<UpdateOptionDto> Options { get; set; } = new List<UpdateOptionDto>();
    
    // Matrix configuration (for questionTypeId = 5)
    public UpdateMatrixConfigDto? MatrixConfig { get; set; }
    
    // Slider configuration (for questionTypeId = 4)
    public UpdateSliderConfigDto? SliderConfig { get; set; }
}

public class UpdateOptionDto
{
    public int? Id { get; set; } // Include ID for existing options, null for new options
    public string? OptionText { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public decimal Score { get; set; } = 0;
}

public class UpdateMatrixConfigDto
{
    public List<UpdateMatrixRowDto> Rows { get; set; } = new List<UpdateMatrixRowDto>();
    public List<UpdateMatrixColumnDto> Columns { get; set; } = new List<UpdateMatrixColumnDto>();
}

public class UpdateMatrixRowDto
{
    public int? Id { get; set; } // Include ID for existing matrix rows, null for new rows
    public string RowLabel { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public class UpdateMatrixColumnDto
{
    public int? Id { get; set; } // Include ID for existing matrix columns, null for new columns
    public string ColumnLabel { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public decimal Score { get; set; } = 0;
}

public class UpdateSliderConfigDto
{
    public int? Id { get; set; } // Include ID for existing slider config, null for new config
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public int StepValue { get; set; } = 1;
    public string? MinLabel { get; set; }
    public string? MaxLabel { get; set; }
}