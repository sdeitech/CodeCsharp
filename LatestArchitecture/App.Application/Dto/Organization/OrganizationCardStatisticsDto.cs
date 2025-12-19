namespace App.Application.Dto.Organization
{
    public class OrganizationCardStatisticsDto
    {
        public int TotalOrganizations { get; set; }
        public int ActiveOrganizations { get; set; }
        public int InactiveOrganizations { get; set; }
        public int ExpiredSubscriptions { get; set; }
        public int UpcomingExpireSubscriptions { get; set; }
        public int ActiveSubscriptions { get; set; }
    }
}
