using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Dto.Common;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;
using App.Common.Constant;

namespace App.Infrastructure.Repository.DynamicQuestionnaire;

public class SubmissionRepository : BaseRepository<Submission>, ISubmissionRepository
{
    public SubmissionRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory) 
        : base(context, dbConnectionFactory)
    {
    }

    public async Task<Submission> CreateAsync(Submission submission)
    {
        Add(submission);
        await _dbContext.SaveChangesAsync();
        return submission;
    }

    public async Task<Submission> UpdateAsync(Submission submission)
    {
        Update(submission);
        await _dbContext.SaveChangesAsync();
        return submission;
    }



    public async Task<Submission?> GetByIdWithDetailsAsync(int submissionId, int organizationId)
    {
        return await DbSet
            .Include(s => s.Form)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
                    .ThenInclude(q => q.QuestionType)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
                    .ThenInclude(q => q.Options)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
                    .ThenInclude(q => q.MatrixRows)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
                    .ThenInclude(q => q.MatrixColumns)
            .Include(s => s.Answers)
                .ThenInclude(a => a.AnswerValues)
                    .ThenInclude(av => av.SelectedOption)
            .FirstOrDefaultAsync(s => s.Id == submissionId && !s.IsDeleted && s.OrganizationId == organizationId);
    }

    public async Task<bool> HasPreviousSubmissionAsync(int formId, string email, int organizationId)
    {
        return await DbSet
            .AnyAsync(s => s.FormId == formId && 
                          s.RespondentEmail.ToLower() == email.ToLower() && 
                          s.OrganizationId == organizationId &&
                          !s.IsDeleted);
    }



    public async Task<IEnumerable<Submission>> GetByFormIdAsync(int formId, int organizationId)
    {
        return await DbSet
            .Include(s => s.Answers)
            .Where(s => s.FormId == formId && !s.IsDeleted && s.OrganizationId == organizationId)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Submission>> GetByFormIdWithDetailsAsync(int formId, int organizationId)
    {
        return await DbSet
            .Include(s => s.Form)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
                    .ThenInclude(q => q.QuestionType)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
                    .ThenInclude(q => q.Options)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
                    .ThenInclude(q => q.MatrixRows)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
                    .ThenInclude(q => q.MatrixColumns)
            .Include(s => s.Answers)
                .ThenInclude(a => a.AnswerValues)
                    .ThenInclude(av => av.SelectedOption)
            .Where(s => s.FormId == formId && !s.IsDeleted && s.OrganizationId == organizationId)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Submission>> GetByEmailAsync(string email, int organizationId)
    {
        return await DbSet
            .Include(s => s.Form)
            .Where(s => s.RespondentEmail.ToLower() == email.ToLower() && 
                       s.OrganizationId == organizationId && 
                       !s.IsDeleted)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync();
    }


    public async Task<PagedResult<FormResponseListDto>> GetPagedResponsesAsync(int formId, ResponseFilterDto filter, int organizationId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@FormId", formId, DbType.Int32);
        parameters.Add("@PageNumber", filter.PageNumber, DbType.Int32);
        parameters.Add("@PageSize", filter.PageSize, DbType.Int32);
        parameters.Add("@SortColumn", filter.SortColumn ?? "SubmittedDate", DbType.String);
        parameters.Add("@SortOrder", filter.SortOrder ?? "DESC", DbType.String);
        parameters.Add("@SearchTerm", filter.SearchTerm, DbType.String);
        parameters.Add("@OrganizationId", organizationId, DbType.Int32);
        parameters.Add("@RespondentEmail", filter.RespondentEmail, DbType.String);
        parameters.Add("@SubmittedDateFrom", filter.SubmittedDateFrom, DbType.DateTime);
        parameters.Add("@SubmittedDateTo", filter.SubmittedDateTo, DbType.DateTime);

        using var multi = await _dbConnection.QueryMultipleAsync(
            SqlMethod.DQ_GetPagedSubmissions, 
            parameters, 
            commandType: CommandType.StoredProcedure);

        // Read submissions data 
        var paginatedSubmissions = (await multi.ReadAsync<FormResponseListDto>()).ToList();
        var totalCount = await multi.ReadSingleAsync<int>();

        return new PagedResult<FormResponseListDto>
        {
            Items = paginatedSubmissions,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<int> UpdateRangeAsync(IEnumerable<Submission> submissions)
    {
        _dbContext.UpdateRange(submissions);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> RecalculateFormScoresAsync(int formId, int organizationId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@FormId", formId, DbType.Int32);
        parameters.Add("@OrganizationId", organizationId, DbType.Int32);
        parameters.Add("@UpdatedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            SqlMethod.DQ_RecalculateFormScores, 
            parameters, 
            commandType: CommandType.StoredProcedure);

        return parameters.Get<int>("@UpdatedCount");
    }
}
