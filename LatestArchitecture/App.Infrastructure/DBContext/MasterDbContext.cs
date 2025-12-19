using App.Domain.Entities.AgencySubscriptions;
using App.Domain.Entities;
using App.Domain.Entities.AuditLog;
using App.Domain.Entities.AuditLogs.AuditLog;
using App.Domain.Entities.EmailFactory;
using App.Domain.Entities.MasterAdmin;
using App.Domain.Entities.MasterData;
using App.Domain.Entities.MasterDatabase;
using App.Domain.Entities.MasterSettings;
using App.Domain.Entities.MasterSystemSettings;
using App.Domain.Entities.MasterTimeZone;
using App.Domain.Entities.Organization;
using App.Domain.Entities.SubscriptionPlan;
using App.Infrastructure.DBContext.Configurations.SystemSetting;
using App.SharedConfigs.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace App.SharedConfigs.DBContext
{
    public class MasterDbContext(DbContextOptions<MasterDbContext> options) : DbContext(options)
    {
        public DbSet<IndustryConfig> IndustryConfig { get; set; }

        //Master Settings
        public DbSet<MasterSetting> MasterSetting { get; set; }
        //Master system Settings
        public DbSet<MasterSystemSetting> MasterSystemSetting { get; set; }
        //Master Time Zones
        public DbSet<MasterTimeZones> MasterTimeZones  { get; set; } 
        public DbSet<MasterSettingNames> MasterSettingNames { get; set; } 

        //Organization DbSets
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<MasterCountry> MasterCountry { get; set; }
        public virtual DbSet<MasterState> MasterState { get; set; }

        //Organization DbSets
        public virtual DbSet<AuditLogs> AuditLogs { get; set; }
        public virtual DbSet<AuditLogMasterTables> AuditLogMasterTables { get; set; }
        public virtual DbSet<AuditLogMasterColumn> AuditLogMasterColumn { get; set; }
        public virtual DbSet<MasterDatabase> MasterDatabase { get; set; }
        public DbSet<SubscriptionPlans> SubscriptionPlans { get; set; }
        public DbSet<SubscriptionPlanModules> SubscriptionPlanModules { get; set; }
        public DbSet<AgencySubscriptionDetail> AgencySubscriptionDetail { get; set; }
        public DbSet<AgencyAdmin> AgencyAdmins { get; set; }
        public  DbSet<EmailProviderConfigs> EmailProviderConfigs { get; set; }
        public  DbSet<EmailProviderType> EmailProviderTypes { get; set; }
        public  DbSet<SmtpConfig> SmtpConfigs { get; set; }
        public  DbSet<AwsSesConfig> AwsSesConfigs { get; set; }
        public  DbSet<SendGridConfig> SendGridConfigs { get; set; }
        public  DbSet<AzureConfig> AzureConfigs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<SubscriptionPlans>();
            modelBuilder.Entity<AgencyAdmin>().ToTable("AgencyAdmin");
            modelBuilder.Entity<AgencyAdmin>().HasKey(a => a.AgencyAdminId);

            modelBuilder.Entity<AgencyAdmin>().HasQueryFilter(a => !a.IsDelete);
            modelBuilder.Entity<MasterSystemSetting>();
            modelBuilder.Entity<MasterTimeZones>();

        }
    }
}
