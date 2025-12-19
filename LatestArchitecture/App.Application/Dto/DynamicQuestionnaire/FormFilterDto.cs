using App.Application.Dto.Common;

namespace App.Application.Dto.DynamicQuestionnaire;

/// <summary>
/// Filter DTO for form listing endpoints. Inherits common paging/sorting/search fields from <see cref="FilterDto"/>.
/// Extend this class with form-specific filters as needed (for example: IsPublished, CreatedBy, etc.).
/// </summary>
public class FormFilterDto : FilterDto
{
    // Optional filter: true = published, false = drafts, null/undefined = all
    public bool? IsPublished { get; set; }
}
