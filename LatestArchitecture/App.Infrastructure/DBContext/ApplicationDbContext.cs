using App.Domain.Entities;
using App.Domain.Entities.AgencySubscriptions;
using App.Domain.Entities.AuditLog;
using App.Domain.Entities.AuditLogs.AuditLog;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Domain.Entities.EmailFactory;
using App.Domain.Entities.MasterData;
using App.Domain.Entities.MasterDatabase;
using App.Domain.Entities.MasterOrg;
using App.Domain.Entities.Organization;
using App.Domain.Entities.SubscriptionPlan;
using App.Infrastructure.DBContext.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace App.Infrastructure.DBContext;

public partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    //public virtual DbSet<Users> Users { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<SubscriptionPlans> SubscriptionPlans { get; set; }
    public DbSet<SubscriptionPlanModules> SubscriptionPlanModules { get; set; }
    public DbSet<VW_AgencyWiseSubscriptionModuleList> VW_AgencyWiseSubscriptionModuleList { get; set; }
    public DbSet<AgencySubscriptionDetail> AgencySubscriptionDetail { get; set; }
    public DbSet<MasterOrganization> MasterOrganization { get; set; }
    public DbSet<Modules> Modules { get; set; }

    
    // Dynamic Questionnaire DbSets
    public virtual DbSet<Form> Forms { get; set; }
    public virtual DbSet<Page> Pages { get; set; }
    public virtual DbSet<Question> Questions { get; set; }
    public virtual DbSet<Option> Options { get; set; }
    public virtual DbSet<MasterQuestionType> MasterQuestionTypes { get; set; }
    public virtual DbSet<SliderConfig> SliderConfig { get; set; }
    public virtual DbSet<MatrixRow> MatrixRow { get; set; }
    public virtual DbSet<MatrixColumn> MatrixColumn { get; set; }
    
    // Phase 4: Response Submission DbSets
    public virtual DbSet<Submission> Submissions { get; set; }
    public virtual DbSet<Answer> Answers { get; set; }
    public virtual DbSet<AnswerValue> AnswerValues { get; set; }
    
    // Phase 6: Conditional Logic Rules DbSets
    public virtual DbSet<Rule> Rules { get; set; }

    //Organization DbSets
    public virtual DbSet<Organization> Organization { get; set; }
    public virtual DbSet<MasterCountry> MasterCountry { get; set; }
    public virtual DbSet<MasterState> MasterState { get; set; }

    //Organization DbSets
    public virtual DbSet<AuditLogs> AuditLogs { get; set; }
    public virtual DbSet<AuditLogMasterTables> AuditLogMasterTables { get; set; }
    public virtual DbSet<AuditLogMasterColumn> AuditLogMasterColumn { get; set; }
    public virtual DbSet<MasterDatabase> MasterDatabase { get; set; }

    public virtual DbSet<LoginLogs> LoginLogs { get; set; }

   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Entity<SubscriptionPlans>();
        EmailProviderTypeSeed.Seed(modelBuilder);
        modelBuilder.Entity<UserEntity>().ToTable("Users");
        modelBuilder.Entity<UserEntity>().HasKey(u => u.UserID);
    }
}