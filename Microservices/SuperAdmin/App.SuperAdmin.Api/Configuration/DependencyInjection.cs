using App.Master.DBContext;
using App.SuperAdmin.Application.Interfaces.Repositories;
using App.SuperAdmin.Application.Interfaces.Service;
using App.SuperAdmin.Infrastructure.DBContext;
using App.SuperAdmin.Infrastructure.Repository;
using App.SuperAdmin.Infrastructure.Service;

namespace App.SuperAdmin.Api.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDependencies(this IServiceCollection services)
        {
            services.AddScoped<IMasterService, MasterService>();
            services.AddScoped<IMasterRepository, MasterRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork<MasterDbContext>>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            return services;
        }
    }
}
