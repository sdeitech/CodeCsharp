using App.Master.DBContext;
using App.UserManagement.Application.Implementation.Service;
using App.UserManagement.Application.Interfaces;
using App.UserManagement.Application.Interfaces.Repositories;
using App.UserManagement.Application.Interfaces.Services;
using App.UserManagement.Infrastructure.DBContext;
using App.UserManagement.Infrastructure.Repository;

namespace App.UserManagement.Api.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddSingleton<Func<IServiceProvider, string>>(sp =>
            {
                var masterDb = sp.GetRequiredService<MasterDbContext>();
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.HttpContext ?? throw new Exception("No active HTTP context found.");

                // Get industry name from header
                var industryName = httpContext.Request.Headers["Industry-Name"].FirstOrDefault();
                var targetConnectionString = masterDb.IndustryConfig
                    .Where(c => c.Name == industryName)
                    .Select(c => c.ConnectionString)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(targetConnectionString))
                {
                    throw new Exception($"AppDb connection string not found for industry '{industryName}'.");
                }

                return targetConnectionString;
            });

            return services;
        }
    }
}
