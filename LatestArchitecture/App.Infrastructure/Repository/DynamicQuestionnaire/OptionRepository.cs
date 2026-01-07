using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository.DynamicQuestionnaire;

public class OptionRepository : BaseRepository<Option>, IOptionRepository
{
    public OptionRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory)
        : base(context, dbConnectionFactory)
    {
    }

    public async Task<IEnumerable<Option>> GetByQuestionIdAsync(int questionId, int organizationId)
    {
        return await DbSet
            .Where(o => o.QuestionId == questionId && o.OrganizationId == organizationId && !o.IsDeleted)
            .OrderBy(o => o.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Option> CreateAsync(Option option)
    {
        Add(option);
        await _dbContext.SaveChangesAsync();
        return option;
    }

    public async Task<Option> UpdateAsync(Option option)
    {
        option.UpdatedAt = DateTime.UtcNow;
        Update(option);
        await _dbContext.SaveChangesAsync();
        return option;
    }

    public async Task<bool> DeleteAsync(int optionId, int organizationId)
    {
        var option = await DbSet
            .Where(o => o.Id == optionId && o.OrganizationId == organizationId && !o.IsDeleted)
            .FirstOrDefaultAsync();
        
        if (option == null) return false;

        option.IsDeleted = true;
        option.DeletedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
