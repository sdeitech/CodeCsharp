using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.LoginLogs;
using App.Infrastructure.DBContext;


namespace App.Infrastructure.Repository.LoginLogs
{
    public class LoginLogRepository : DbConnectionRepositoryBase, ILoginLogRepository
    {
        public LoginLogRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory)
        : base(context, dbConnectionFactory)
        {
            _dbContext = context;
        }
        private readonly ApplicationDbContext _dbContext;



        public void AddLoginLog(double? Longitude, double? Latitude, int? OrganizationId, int PortalId,
            string IPAddress, int CreatedBy, string LoginLogsStatus, int ActionId, string ScreenName)
        {
            var loginLogs = new App.Domain.Entities.LoginLogs
            {
                Longitude = Longitude,
                Latitude = Latitude,
                OrganizationId = OrganizationId,
                PortalId = PortalId,
                LoginLogsStatus = LoginLogsStatus,
                IPAddress = IPAddress,
                UserId = CreatedBy,
                CreatedBy = CreatedBy,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,
                ActionId = ActionId,
                ScreenName = ScreenName
            };

            _dbContext.LoginLogs.Add(loginLogs);
            _dbContext.SaveChanges();
        }
    }
}

