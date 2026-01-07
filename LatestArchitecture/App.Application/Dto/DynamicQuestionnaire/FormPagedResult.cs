using App.Application.Dto.Common;

namespace App.Application.Dto.DynamicQuestionnaire;

public class FormPagedResult<T> : PagedResult<T>
{
    // Published/Draft counts are specific to forms
    public int? PublishedCount { get; set; }
    public int? DraftCount { get; set; }
}
