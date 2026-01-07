namespace App.Application.Dto.MasterDatabase
{
    public class MasterDatabaseCountsResponseDto
    {
        public int ActiveDatabase { get; set; }
        public int InActiveDatabase { get; set; }
        public int CentralizedDatabase { get; set; }
        public int MultiTenantDatabase { get; set; }
    }
}
