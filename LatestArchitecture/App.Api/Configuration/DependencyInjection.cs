using App.Application.Interfaces;
using App.Application.Interfaces.Repositories;
using App.Application.Interfaces.Repositories.AgencySubscriptions;
using App.Application.Interfaces.Repositories.AuditLogs;
using App.Application.Interfaces.Repositories.AuthenticationModule;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Application.Interfaces.Repositories.LoginLogs;
using App.Application.Interfaces.Repositories.MasterData;
using App.Application.Interfaces.Repositories.MasterDatabase;
using App.Application.Interfaces.Repositories.Organization;
using App.Application.Interfaces.Repositories.SubscriptionPlan;
using App.Application.Interfaces.Services;
using App.Application.Interfaces.Services.AgencySubscriptions;
using App.Application.Interfaces.Services.AuditLog;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Application.Interfaces.Services.Email;
using App.Application.Interfaces.Services.Images;
using App.Application.Interfaces.Services.MasterData;
using App.Application.Interfaces.Services.MasterDatabase;
using App.Application.Interfaces.Services.Organization;
using App.Application.Interfaces.Services.SubscriptionPlan;
using App.Application.Service;
using App.Application.Service.AuditLog;
using App.Application.Service.AuthenticationModule;
using App.Application.Service.DynamicQuestionnaire;
using App.Application.Service.EmailProviders;
using App.Application.Service.Images;
using App.Application.Service.MasterData;
using App.Application.Service.MasterDatabase;
using App.Application.Service.Organization;
using App.Application.SubscriptionPlansService;
using App.Infrastructure.DBContext;
using App.Infrastructure.Repository;
using App.Infrastructure.Repository.AgencySubscriptions;
using App.Infrastructure.Repository.AuditLog;
using App.Infrastructure.Repository.AuthenticationModule;
using App.Infrastructure.Repository.DynamicQuestionnaire;
using App.Infrastructure.Repository.LoginLogs;
using App.Infrastructure.Repository.MasterData;
using App.Infrastructure.Repository.MasterDatabase;
using App.Infrastructure.Repository.Organization;
using App.Infrastructure.Repository.SubscriptionPlan;
using App.Application.Interfaces.Services.MasterSettings;
using App.Application.Service.MasterSettings;
using App.Application.Interfaces.Repositories.MasterSettings;
using App.Infrastructure.Repository.MasterSettings;
using App.Application.Interfaces.Services.MasterSystemSettingsService;
using App.Application.Interfaces.Repositories.MasterSystemSettings;
using App.Application.Service.MasterSystemSettings;
using App.Infrastructure.Repository.MasterSystemSettings;
using App.SharedConfigs.DBContext;
using App.Application.Interfaces.Services.AwsServices;
using App.Application.Service.AwsServices;
using App.Application.Interfaces.Services.AzureBlobStorageService;
using App.Application.Service.AzureBlobStorageService;
using App.Infrastructure.Repository.MasterAdmin;
using App.Application.Service.MasterAdmin;
using App.Application.Interfaces.Services.MasterTimeZones;
using App.Application.Service.MasterTimeZones;
using App.Application.Interfaces.Repositories.MasterTimeZone;
using App.Infrastructure.Repository.MasterTimeZone;
using App.Infrastructure.Repository.EmailFactory;
using App.Application.Interfaces.Repositories.EmailFactory;
using App.Application.Service.EmailConfiguration;
using App.Application.Interfaces.Services.EmailFactory;
using App.Application.Interfaces.Services.SuperAdminDashboard;
using App.Application.Service.SuperAdminDashboard;
using App.Application.Interfaces.Repositories.SuperAdminDashboard;
using App.Infrastructure.Repository.SuperAdminDashboard;

namespace App.Api.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDependencies(this IServiceCollection services)
        {

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();

            // Register SuperAdminTokenService
            services.AddScoped<App.Application.Interfaces.Services.SuperAdmin.ISuperAdminTokenService, App.Application.Service.SuperAdmin.SuperAdminTokenService>();
            services.AddScoped<IUserRepository, UserRepository>();

            ////AWS services (uncomment this for aws s3 bucket access)
            //services.AddScoped<IAwsServices, AwsServices>();

            ////Azure Blob services
            //services.AddScoped<IAzureBlobStorageService, AzureBlobStorageService>();

            //Authentication

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<IBruteForceProtectionService, BruteForceProtectionService>();
            services.AddScoped<IBruteForceProtectionRepository, BruteForceProtectionRepository>();

            // Super Admin Authentication
            services.AddScoped<App.Application.Interfaces.Services.SuperAdmin.ISuperAdminAuthenticationService, App.Application.Service.SuperAdmin.SuperAdminAuthenticationService>();
            services.AddScoped<App.Application.Interfaces.Repositories.SuperAdmin.ISuperAdminAuthenticationRepository, App.Infrastructure.Repository.SuperAdmin.SuperAdminAuthenticationRepository>();

           
            // Master Settings Dependencies
            #region Master Settings Dependencies
            services.AddScoped<IMasterSettingsService, MasterSettingsService>();
            services.AddScoped<IMasterSettingsRepository, MasterSettingsRepository>();
            #endregion

            // Master System Settings Dependencies
            #region Master System Settings Dependencies
            services.AddScoped<IMasterSystemSettingsService, MasterSystemSettingsService>();
            services.AddScoped<IMasterSystemSettingsRepository, MasterSystemSettingsRepository>();
            #endregion

            // Master Time Zones Dependencies
            #region Master Time Zones Dependencies
            services.AddScoped<IMasterTimeZonesService, MasterTimeZonesService>();
            services.AddScoped<IMasterTimeZonesRepository, MasterTimeZonesRepository>();
            services.AddScoped<DateTimeConversionFilterService>();
            #endregion



            // Master Dashboard Dependencies
            #region Master Dashboard Dependencies
            services.AddScoped<ISuperAdminDashboardService, SuperAdminDashboardService>();
            services.AddScoped<ISuperAdminDashboardRepository, SuperAdminDashboardRepository>();
         
            #endregion

            // Dynamic Questionnaire Dependencies
            #region Dynamic Questionnaire Dependencies
            services.AddScoped<IDynamicQuestionnaireService, DynamicQuestionnaireService>();
            services.AddScoped<IFormRepository, FormRepository>();
            services.AddScoped<IPageRepository, PageRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IOptionRepository, OptionRepository>();
            services.AddScoped<IMasterQuestionTypeRepository, MasterQuestionTypeRepository>();

            services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<IAnswerValueRepository, AnswerValueRepository>();

            // Phase 6: Rule Repository
            services.AddScoped<IRuleRepository, RuleRepository>();

            // Phase 7: Scoring Engine Service
            services.AddScoped<IScoringEngineService, ScoringEngineService>();

            // Phase 8: Export Service
            services.AddScoped<IExportService, ExportService>();
            #endregion

            //Organization Dependencies
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            //Audit Log Dependencies
            services.AddTransient<IAuditLogService, AuditLogService>();
            services.AddTransient<ILoginLogRepository, LoginLogRepository>();
            services.AddTransient<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
            services.AddScoped<IUnitOfWork, UnitOfWork<MasterDbContext>>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

            //Subscription Management
            services.AddScoped<ISubscriptionPlansService, SubscriptionPlansService>();
            services.AddScoped<ISubscriptionPlansRepository, SubscriptionPlansRepository>();
            services.AddScoped<IAgencySubscriptionPlanService, AgencySubscriptionPlanService>();
            services.AddScoped<IAgencySubscriptionPlanRepository, AgencySubscriptionPlanRepository>();

            // MasterCountry Dependencies
            services.AddScoped<IMasterDataService, MasterDataService>();
            services.AddScoped<IMasterDataRepository, MasterDataRepository>();

            // MasterDatabase Dependencies
            services.AddScoped<IMasterDatabaseService, MasterDatabaseService>();
            services.AddScoped<IMasterDatabaseRepository, MasterDatabaseRepository>();
          


            // Email Provider Dependencies

            services.AddScoped<IEmailProviderTypeRepository, EmailProviderTypeRepository>();
            services.AddScoped<IEmailProviderTypeService, EmailProviderTypeService>();
            services.AddScoped<SmtpEmailProvider>();
            services.AddScoped<AwsSesEmailProvider>();
            services.AddScoped<SendGridEmailProvider>();
            services.AddScoped<EmailProviderFactory>();
            // Email Configuration Dependencies
            services.AddScoped<IEmailProviderConfigRepository, EmailProviderConfigRepository>();
            services.AddScoped<IEmailConfigurationService, EmailConfigurationService>();
            //services.AddScoped<AzureEmailProvider>();

            //services.AddSingleton<Func<IServiceProvider, string>>(sp =>
            //{
            //    var configDb = sp.GetRequiredService<MasterDbContext>();
            //    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            //    var httpContext = httpContextAccessor.HttpContext ?? throw new Exception("No active HTTP context found.");

            //    // Get industry name from header
            //    var industryName = httpContext.Request.Headers["Industry-Name"].FirstOrDefault();
            //    var targetConnectionString = configDb.IndustryConfig
            //        .Where(c => c.Name == industryName)
            //        .Select(c => c.ConnectionString)
            //        .FirstOrDefault();

            //    if (string.IsNullOrEmpty(targetConnectionString))
            //    {
            //        throw new Exception($"AppDb connection string not found for industry '{industryName}'.");
            //    }

            //    return targetConnectionString;
            //});

            return services;
        }
    }
}
