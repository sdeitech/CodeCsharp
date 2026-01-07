namespace App.Application.Dto.AuditLog
{
    public class AuditLogResponseDto
    {
        public int LogID { get; private set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public int? OrganizationID { get; set; }
        public int ActionID { get; set; }
        public int ActionName { get; set; }
        public int ColumnID { get; set; }
        public int ColumnName { get; set; }
        public string ScreenName { get; set; }
        public int TableID { get; set; }
        public int TableName { get; set; }
        public string IPAddress { get; set; }
        public int StatusID { get; set; }
        public int StatusName { get; set; }
        public int PortalID { get; set; }
        public int PortalName { get; set; }
        public int? ChangedBy { get; set; }
        public int? ChangedName { get; set; }
        public DateTime LogDate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
