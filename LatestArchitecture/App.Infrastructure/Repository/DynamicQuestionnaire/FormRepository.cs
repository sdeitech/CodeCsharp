using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Application.Dto.Common;
using App.Application.Dto.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;
using App.Common.Constant;

namespace App.Infrastructure.Repository.DynamicQuestionnaire;

public class FormRepository : BaseRepository<Form>, IFormRepository
{
    public FormRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory)
        : base(context, dbConnectionFactory)
    {
    }



    public async Task<Form?> GetByIdWithDetailsAsync(int formId, int organizationId)
    {
        return await DbSet
            .Include(f => f.Pages.Where(p => !p.IsDeleted))
                .ThenInclude(p => p.Questions.Where(q => !q.IsDeleted))
                    .ThenInclude(q => q.QuestionType)
            .Include(f => f.Pages)
                .ThenInclude(p => p.Questions)
                    .ThenInclude(q => q.Options.Where(o => !o.IsDeleted))
            .Include(f => f.Pages)
                .ThenInclude(p => p.Questions)
                    .ThenInclude(q => q.SliderConfig)
            .Include(f => f.Pages)
                .ThenInclude(p => p.Questions)
                    .ThenInclude(q => q.MatrixRows)
            .Include(f => f.Pages)
                .ThenInclude(p => p.Questions)
                    .ThenInclude(q => q.MatrixColumns)
            .Where(f => !f.IsDeleted && f.OrganizationId == organizationId)
            .FirstOrDefaultAsync(f => f.Id == formId);
    }

    public async Task<Form?> GetByPublicKeyWithDetailsAsync(string publicKey)
    {
        return await DbSet
            .Include(f => f.Pages.Where(p => !p.IsDeleted))
                .ThenInclude(p => p.Questions.Where(q => !q.IsDeleted))
                    .ThenInclude(q => q.QuestionType)
            .Include(f => f.Pages)
                .ThenInclude(p => p.Questions)
                    .ThenInclude(q => q.Options.Where(o => !o.IsDeleted))
            .Include(f => f.Pages)
                .ThenInclude(p => p.Questions)
                    .ThenInclude(q => q.SliderConfig)
            .Include(f => f.Pages)
                .ThenInclude(p => p.Questions)
                    .ThenInclude(q => q.MatrixRows)
            .Include(f => f.Pages)
                .ThenInclude(p => p.Questions)
                    .ThenInclude(q => q.MatrixColumns)
            .Where(f => !f.IsDeleted && f.IsPublished)
            .FirstOrDefaultAsync(f => f.PublicKey == publicKey);
    }

    public async Task<bool> PublicKeyExistsAsync(string publicKey)
    {
        return await DbSet
            .Where(f => !f.IsDeleted)
            .AnyAsync(f => f.PublicKey == publicKey);
    }

    public async Task<Form> CreateAsync(Form form)
    {
        Add(form);
        await _dbContext.SaveChangesAsync();
        return form;
    }

    public async Task<Form> UpdateAsync(Form form)
    {
        form.UpdatedAt = DateTime.UtcNow;
        
        Update(form);
        await _dbContext.SaveChangesAsync();
        return form;
    }

    public async Task<bool> DeleteAsync(int formId, int organizationId)
    {
        var form = await DbSet
            .Where(f => f.Id == formId && f.OrganizationId == organizationId && !f.IsDeleted)
            .FirstOrDefaultAsync();
        
        if (form == null) return false;

        form.IsDeleted = true;
        form.DeletedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }



    public async Task<IEnumerable<Form>> GetAllAsync(int organizationId)
    {
        return await DbSet
            .Where(f => !f.IsDeleted && f.OrganizationId == organizationId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }



    public async Task<FormPagedResult<FormResponseDto>> GetPagedFormsAsync(FormFilterDto filter, int organizationId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@PageNumber", filter.PageNumber, DbType.Int32);
        parameters.Add("@PageSize", filter.PageSize, DbType.Int32);
        parameters.Add("@SortColumn", filter.SortColumn ?? "CreatedAt", DbType.String);
        parameters.Add("@SortOrder", filter.SortOrder ?? "DESC", DbType.String);
        parameters.Add("@SearchTerm", filter.SearchTerm, DbType.String);
        parameters.Add("@OrganizationId", organizationId, DbType.Int32);
    parameters.Add("@IsPublished", filter.IsPublished.HasValue ? (object)(filter.IsPublished.Value ? 1 : 0) : DBNull.Value, DbType.Int32);

        using var multi = await _dbConnection.QueryMultipleAsync(
            SqlMethod.DQ_GetPagedForms, 
            parameters, 
            commandType: CommandType.StoredProcedure);

        // Use FormDbDto to match database schema exactly
        var paginatedForms = (await multi.ReadAsync<FormResponseDto>()).ToList();
        var totalCount = await multi.ReadSingleAsync<int>();

        // Try read optional next result set for published/draft counts
        int? publishedCount = null;
        int? draftCount = null;
        try
        {
            var counts = await multi.ReadFirstOrDefaultAsync<dynamic>();
            if (counts != null)
            {
                publishedCount = (int?)counts.PublishedCount;
                draftCount = (int?)counts.DraftCount;
            }
        }
        catch
        {
            // ignore if result set isn't present (backwards compatible)
        }

        return new App.Application.Dto.DynamicQuestionnaire.FormPagedResult<FormResponseDto>
        {
            Items = paginatedForms,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            PublishedCount = publishedCount,
            DraftCount = draftCount
        };
    }
}
