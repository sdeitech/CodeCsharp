using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository.DynamicQuestionnaire;

public class AnswerRepository : BaseRepository<Answer>, IAnswerRepository
{
    public AnswerRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory) 
        : base(context, dbConnectionFactory)
    {
    }

    public async Task<Answer> CreateAsync(Answer answer)
    {
        Add(answer);
        await _dbContext.SaveChangesAsync();
        return answer;
    }

    public async Task<IEnumerable<Answer>> GetBySubmissionIdAsync(int submissionId, int organizationId)
    {
        return await DbSet
            .Include(a => a.Question)
            .Include(a => a.AnswerValues)
                .ThenInclude(av => av.SelectedOption)
            .Where(a => a.SubmissionId == submissionId && a.OrganizationId == organizationId && !a.IsDeleted)
            .ToListAsync();
    }

    public async Task<int> CreateRangeAsync(IEnumerable<Answer> answers)
    {
        AddRange(answers);
        return await _dbContext.SaveChangesAsync();
    }
}
