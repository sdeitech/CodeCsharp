using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository.DynamicQuestionnaire;

public class QuestionRepository : BaseRepository<Question>, IQuestionRepository
{
    public QuestionRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory)
        : base(context, dbConnectionFactory)
    {
    }

    public async Task<IEnumerable<Question>> GetByPageIdAsync(int pageId, int organizationId)
    {
        return await DbSet
            .Include(q => q.QuestionType)
            .Include(q => q.Options.Where(o => !o.IsDeleted))
            .Include(q => q.SliderConfig)
            .Include(q => q.MatrixRows)
            .Include(q => q.MatrixColumns)
            .Where(q => q.PageId == pageId && q.OrganizationId == organizationId && !q.IsDeleted)
            .OrderBy(q => q.QuestionOrder)
            .ToListAsync();
    }

    public async Task<Question?> GetByIdWithMatrixColumnsAsync(int questionId, int organizationId)
    {
        return await DbSet
            .Include(q => q.MatrixColumns)
            .Where(q => q.Id == questionId && q.OrganizationId == organizationId && !q.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<Question> CreateAsync(Question question)
    {
        Add(question);
        await _dbContext.SaveChangesAsync();
        return question;
    }

    public async Task<Question> UpdateAsync(Question question)
    {
        question.UpdatedAt = DateTime.UtcNow;
        Update(question);
        await _dbContext.SaveChangesAsync();
        return question;
    }

    public async Task<bool> DeleteAsync(int questionId, int organizationId)
    {
        var question = await DbSet
            .Where(q => q.Id == questionId && q.OrganizationId == organizationId && !q.IsDeleted)
            .FirstOrDefaultAsync();
        
        if (question == null) return false;

        question.IsDeleted = true;
        question.DeletedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
