using App.Master.DBContext;
using Microsoft.EntityFrameworkCore;

namespace App.SuperAdmin.Api.Configuration
{
    public static class DbContextInitializer
    {
        public static void AddConfiguredDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            var masterConnectionString = configuration.GetConnectionString("MasterConnection");
            services.AddDbContext<MasterDbContext>(options =>
                options.UseSqlServer(masterConnectionString));

        }
    }
}
