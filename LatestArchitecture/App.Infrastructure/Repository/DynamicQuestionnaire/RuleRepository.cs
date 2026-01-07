using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository.DynamicQuestionnaire;

/// <summary>
/// Repository for managing conditional logic rules
/// </summary>
public class RuleRepository : BaseRepository<Rule>, IRuleRepository
{
    public RuleRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory) 
        : base(context, dbConnectionFactory)
    {
    }

    public async Task<Rule> CreateAsync(Rule rule)
    {
        rule.CreatedAt = DateTime.UtcNow;
        DbSet.Add(rule);
        await _dbContext.SaveChangesAsync();
        return rule;
    }

    public async Task<Rule> UpdateAsync(Rule rule)
    {
        rule.UpdatedAt = DateTime.UtcNow;
        DbSet.Update(rule);
        await _dbContext.SaveChangesAsync();
        return rule;
    }

    public async Task<bool> DeleteAsync(int ruleId, int organizationId)
    {
        var rule = await DbSet
            .Where(r => r.Id == ruleId && r.OrganizationId == organizationId && !r.IsDeleted)
            .FirstOrDefaultAsync();
        
        if (rule == null)
            return false;

        rule.IsDeleted = true;
        rule.DeletedAt = DateTime.UtcNow;
        DbSet.Update(rule);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Rule?> GetByIdWithDetailsAsync(int ruleId, int organizationId)
    {
        return await DbSet
            .Include(r => r.SourceQuestion)
            .Include(r => r.TriggerOption)
            .Include(r => r.TargetQuestion)
            .Include(r => r.TargetPage)
            .Where(r => !r.IsDeleted && r.Id == ruleId && r.OrganizationId == organizationId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Rule>> GetByFormIdAsync(int formId, int organizationId)
    {
        return await DbSet
            .Where(r => !r.IsDeleted && r.FormId == formId && r.OrganizationId == organizationId)
            .OrderBy(r => r.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rule>> GetByFormIdWithDetailsAsync(int formId, int organizationId)
    {
        return await DbSet
            .Include(r => r.SourceQuestion)
            .Include(r => r.TriggerOption)
            .Include(r => r.TargetQuestion)
            .Include(r => r.TargetPage)
            .Where(r => !r.IsDeleted && r.FormId == formId && r.OrganizationId == organizationId)
            .OrderBy(r => r.Id)
            .ToListAsync();
    }

    public async Task<bool> RuleExistsAsync(int formId, int sourceQuestionId, int? triggerOptionId, string condition, int organizationId)
    {
        return await DbSet
            .AnyAsync(r => !r.IsDeleted && 
                          r.FormId == formId && 
                          r.SourceQuestionId == sourceQuestionId && 
                          r.TriggerOptionId == triggerOptionId && 
                          r.Condition == condition &&
                          r.OrganizationId == organizationId);
    }
}
