namespace App.Application.Dto.MasterDatabase
{
    public class MasterDatabaseDto
    {
        public int DatabaseID { get; set; }
        public string DatabaseName { get; set; }
        public string Password { get; set; }
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public bool IsCentralized { get; set; }
        public int? ParentOrganizationID { get; set; }
        public string? OrganizationName { get; set; }
    }

    public class MasterDatabaseResponseDto
    {
        public int DatabaseID { get; set; }
        public string DatabaseName { get; set; }
        public string Password { get; set; }
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public bool? IsCentralized { get; set; }
        public int? ParentOrganizationID { get; set; }
        public string? OrganizationName { get; set; }
        public int TotalRecords { get; set; }
    }

        public class MasterDatabaseResponseForDropdownDto
    {
        public int DatabaseID { get; set; }
        public string DatabaseName { get; set; }
    }
}
