using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository.DynamicQuestionnaire;

public class PageRepository : BaseRepository<Page>, IPageRepository
{
    public PageRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory)
        : base(context, dbConnectionFactory)
    {
    }

    public async Task<IEnumerable<Page>> GetByFormIdAsync(int formId, int organizationId)
    {
        return await DbSet
            .Where(p => p.FormId == formId && p.OrganizationId == organizationId && !p.IsDeleted)
            .OrderBy(p => p.PageOrder)
            .ToListAsync();
    }

    public async Task<Page> CreateAsync(Page page)
    {
        Add(page);
        await _dbContext.SaveChangesAsync();
        return page;
    }

    public async Task<Page> UpdateAsync(Page page)
    {
        page.UpdatedAt = DateTime.UtcNow;
        Update(page);
        await _dbContext.SaveChangesAsync();
        return page;
    }

    public async Task<bool> DeleteAsync(int pageId, int organizationId)
    {
        var page = await DbSet
            .Where(p => p.Id == pageId && p.OrganizationId == organizationId && !p.IsDeleted)
            .FirstOrDefaultAsync();
        
        if (page == null) return false;

        page.IsDeleted = true;
        page.DeletedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }
}