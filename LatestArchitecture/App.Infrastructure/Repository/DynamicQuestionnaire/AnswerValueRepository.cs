using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Infrastructure.DBContext;
using App.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository.DynamicQuestionnaire;

public class AnswerValueRepository : BaseRepository<AnswerValue>, IAnswerValueRepository
{
    public AnswerValueRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory) 
        : base(context, dbConnectionFactory)
    {
    }

    public async Task<AnswerValue> CreateAsync(AnswerValue answerValue)
    {
        Add(answerValue);
        await _dbContext.SaveChangesAsync();
        return answerValue;
    }

    public async Task<IEnumerable<AnswerValue>> GetByAnswerIdAsync(int answerId, int organizationId)
    {
        return await DbSet
            .Include(av => av.SelectedOption)
            .Where(av => av.AnswerId == answerId && av.OrganizationId == organizationId && !av.IsDeleted)
            .ToListAsync();
    }

    public async Task<int> CreateRangeAsync(IEnumerable<AnswerValue> answerValues)
    {
        AddRange(answerValues);
        return await _dbContext.SaveChangesAsync();
    }
}
