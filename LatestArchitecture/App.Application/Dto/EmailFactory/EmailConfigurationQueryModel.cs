namespace App.Api.Models
{
    public class EmailConfigurationQueryModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? SortColumn { get; set; } = "OrganizationName";
        public string? SortDirection { get; set; } = "ASC";
    }
}
