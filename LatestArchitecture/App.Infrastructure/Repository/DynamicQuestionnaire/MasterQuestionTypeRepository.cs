using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository.DynamicQuestionnaire;

public class MasterQuestionTypeRepository : IMasterQuestionTypeRepository
{
    private readonly ApplicationDbContext _context;

    public MasterQuestionTypeRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory)
    {
        _context = context;
    }

    public async Task<MasterQuestionType?> GetByIdAsync(int questionTypeId)
    {
        return await _context.MasterQuestionTypes
            .FirstOrDefaultAsync(qt => qt.Id == questionTypeId && qt.IsActive);
    }

    public async Task<IEnumerable<MasterQuestionType>> GetAllAsync()
    {
        return await _context.MasterQuestionTypes
            .Where(qt => qt.IsActive)
            .OrderBy(qt => qt.TypeName)
            .ToListAsync();
    }

    public async Task<MasterQuestionType?> GetByTypeNameAsync(string typeName)
    {
        return await _context.MasterQuestionTypes
            .FirstOrDefaultAsync(qt => qt.TypeName == typeName && qt.IsActive);
    }
}
