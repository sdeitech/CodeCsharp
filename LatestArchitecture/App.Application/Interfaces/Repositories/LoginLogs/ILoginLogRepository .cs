namespace App.Application.Interfaces.Repositories.LoginLogs
{
    public interface ILoginLogRepository
    {
        public void AddLoginLog(double? Longitude, double? Latitude, int? OrganizationId, int PortalId, string IPAddress, int CreatedBy, string LoginLogsStatus, int ActionId, string ScreenName);
    }
}
