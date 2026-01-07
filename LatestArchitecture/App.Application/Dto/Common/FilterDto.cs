using System.ComponentModel.DataAnnotations;

namespace App.Application.Dto.Common;

public class FilterDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;

    public string? SortColumn { get; set; }

    public string? SortOrder { get; set; } = "ASC"; // "ASC" or "DESC"

    public string? SearchTerm { get; set; }

}
