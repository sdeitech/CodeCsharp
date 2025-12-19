using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Application.Service.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Domain.Enums;
using App.Domain.Enums.DynamicQuestionnaire;
using App.Infrastructure.DBContext;
using App.Infrastructure.Repository.DynamicQuestionnaire;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using App.Application.Dto.Common;
using System.Data;
using DomainRule = App.Domain.Entities.DynamicQuestionnaire.Rule;

namespace App.Services.Test.DynamicQuestionnaire;

// Test-friendly repository implementations that don't use DbConnectionRepositoryBase
public class TestFormRepository : IFormRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Form> _dbSet;

    public TestFormRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Form>();
    }

    public void Add(Form entity) => _dbSet.Add(entity);
    public void AddRange(IEnumerable<Form> entities) => _dbSet.AddRange(entities);
    public IQueryable<Form> GetAll() => _dbSet.Where(f => !f.IsDeleted);
    public IQueryable<Form> GetAllNoTracking() => _dbSet.Where(f => !f.IsDeleted).AsNoTracking();
    
    public async Task<Form?> GetByIdAsync(int id)
    {
        return await _dbSet.Where(f => !f.IsDeleted).FirstOrDefaultAsync(f => f.Id == id);
    }

    public void Update(Form entity) => _dbSet.Update(entity);
    public async Task<bool> ExistsAsync(int id) => await _dbSet.Where(f => !f.IsDeleted).AnyAsync(f => f.Id == id);
    public void Remove(Form entity, bool hardDelete = false)
    {
        if (hardDelete) _dbSet.Remove(entity);
        else { entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; }
    }
    public void RemoveRange(IEnumerable<Form> entities, bool hardDelete = false)
    {
        if (hardDelete) _dbSet.RemoveRange(entities);
        else foreach (var entity in entities) { entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; }
    }

    public async Task<Form?> GetByIdWithDetailsAsync(int formId)
    {
        return await _dbSet
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
            .Where(f => !f.IsDeleted)
            .FirstOrDefaultAsync(f => f.Id == formId);
    }

    public async Task<Form?> GetByIdWithDetailsAsync(int formId, int organizationId)
    {
        return await _dbSet
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

    public async Task<Form> CreateAsync(Form form)
    {
        Add(form);
        await _context.SaveChangesAsync();
        return form;
    }

    public async Task<Form> UpdateAsync(Form form)
    {
        form.UpdatedAt = DateTime.UtcNow;
        Update(form);
        await _context.SaveChangesAsync();
        return form;
    }

    public async Task<bool> DeleteAsync(int formId)
    {
        var form = await GetByIdAsync(formId);
        if (form == null) return false;
        form.IsDeleted = true;
        form.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int formId, int organizationId)
    {
        var form = await _dbSet
            .Where(f => !f.IsDeleted && f.Id == formId && f.OrganizationId == organizationId)
            .FirstOrDefaultAsync();
        if (form == null) return false;
        form.IsDeleted = true;
        form.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Form>> GetAllAsync()
    {
        return await _dbSet
            .Where(f => !f.IsDeleted)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Form>> GetAllAsync(int organizationId)
    {
        return await _dbSet
            .Where(f => !f.IsDeleted && f.OrganizationId == organizationId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<FormPagedResult<FormResponseDto>> GetPagedFormsAsync(FormFilterDto filter)
    {
        // Simple in-memory implementation for testing
        var query = _dbSet.Where(f => !f.IsDeleted);
        
        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            query = query.Where(f => f.Title.Contains(filter.SearchTerm) || (f.Description != null && f.Description.Contains(filter.SearchTerm)));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(f => new FormResponseDto
            {
                FormId = f.Id,
                Title = f.Title,
                Description = f.Description,
                Instructions = f.Instructions,
                AllowResubmission = f.AllowResubmission,
                CreatedAt = f.CreatedAt ?? DateTime.UtcNow
            })
            .ToListAsync();

    return new App.Application.Dto.DynamicQuestionnaire.FormPagedResult<FormResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<App.Application.Dto.DynamicQuestionnaire.FormPagedResult<FormResponseDto>> GetPagedFormsAsync(FormFilterDto filter, int organizationId)
    {
        // Simple in-memory implementation for testing with organization filtering
        var query = _dbSet.Where(f => !f.IsDeleted && f.OrganizationId == organizationId);
        
        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            query = query.Where(f => f.Title.Contains(filter.SearchTerm) || (f.Description != null && f.Description.Contains(filter.SearchTerm)));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(f => new FormResponseDto
            {
                FormId = f.Id,
                Title = f.Title,
                Description = f.Description,
                Instructions = f.Instructions,
                AllowResubmission = f.AllowResubmission,
                CreatedAt = f.CreatedAt ?? DateTime.UtcNow
            })
            .ToListAsync();

    return new App.Application.Dto.DynamicQuestionnaire.FormPagedResult<FormResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<Form?> GetByPublicKeyWithDetailsAsync(string publicKey)
    {
        return await _dbSet
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
        return await _dbSet
            .Where(f => !f.IsDeleted)
            .AnyAsync(f => f.PublicKey == publicKey);
    }

    public void Dispose() => _context?.Dispose();
}

public class TestMasterQuestionTypeRepository : IMasterQuestionTypeRepository
{
    private readonly ApplicationDbContext _context;

    public TestMasterQuestionTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MasterQuestionType?> GetByIdAsync(int questionTypeId)
    {
        return await _context.MasterQuestionTypes
            .Where(qt => qt.IsActive)
            .FirstOrDefaultAsync(qt => qt.Id == questionTypeId);
    }

    public async Task<IEnumerable<MasterQuestionType>> GetAllAsync()
    {
        return await _context.MasterQuestionTypes
            .Where(qt => qt.IsActive)
            .ToListAsync();
    }

    public async Task<MasterQuestionType?> GetByTypeNameAsync(string typeName)
    {
        return await _context.MasterQuestionTypes
            .Where(qt => qt.IsActive)
            .FirstOrDefaultAsync(qt => qt.TypeName == typeName);
    }
}

// Test repositories for submission-related entities
public class TestSubmissionRepository : ISubmissionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Submission> _dbSet;

    public TestSubmissionRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Submission>();
    }

    public void Add(Submission entity) => _dbSet.Add(entity);
    public void AddRange(IEnumerable<Submission> entities) => _dbSet.AddRange(entities);
    public IQueryable<Submission> GetAll() => _dbSet.Where(s => !s.IsDeleted);
    public IQueryable<Submission> GetAllNoTracking() => _dbSet.Where(s => !s.IsDeleted).AsNoTracking();
    
    public async Task<Submission?> GetByIdAsync(int id)
    {
        return await _dbSet.Where(s => !s.IsDeleted).FirstOrDefaultAsync(s => s.Id == id);
    }

    public void Update(Submission entity) => _dbSet.Update(entity);
    public async Task<bool> ExistsAsync(int id) => await _dbSet.Where(s => !s.IsDeleted).AnyAsync(s => s.Id == id);
    public void Remove(Submission entity, bool hardDelete = false)
    {
        if (hardDelete) _dbSet.Remove(entity);
        else { entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; }
    }
    public void RemoveRange(IEnumerable<Submission> entities, bool hardDelete = false)
    {
        if (hardDelete) _dbSet.RemoveRange(entities);
        else foreach (var entity in entities) { entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; }
    }

    public async Task<Submission> CreateAsync(Submission submission)
    {
        Add(submission);
        await _context.SaveChangesAsync();
        return submission;
    }

    public async Task<Submission> UpdateAsync(Submission submission)
    {
        Update(submission);
        await _context.SaveChangesAsync();
        return submission;
    }

    public async Task<Submission?> GetByIdWithDetailsAsync(int submissionId)
    {
        return await _dbSet
            .Include(s => s.Form)
            .Include(s => s.Answers)
                .ThenInclude(a => a.AnswerValues)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
            .Where(s => !s.IsDeleted)
            .FirstOrDefaultAsync(s => s.Id == submissionId);
    }

    public async Task<Submission?> GetByIdWithDetailsAsync(int submissionId, int organizationId)
    {
        return await _dbSet
            .Include(s => s.Form)
            .Include(s => s.Answers)
                .ThenInclude(a => a.AnswerValues)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
            .Where(s => !s.IsDeleted && s.OrganizationId == organizationId)
            .FirstOrDefaultAsync(s => s.Id == submissionId);
    }

    public async Task<bool> HasPreviousSubmissionAsync(int formId, string email)
    {
        return await _dbSet
            .Where(s => !s.IsDeleted)
            .AnyAsync(s => s.FormId == formId && s.RespondentEmail == email);
    }

    public async Task<bool> HasPreviousSubmissionAsync(int formId, string email, int organizationId)
    {
        return await _dbSet
            .Where(s => !s.IsDeleted && s.OrganizationId == organizationId)
            .AnyAsync(s => s.FormId == formId && s.RespondentEmail == email);
    }

    public async Task<IEnumerable<Submission>> GetByFormIdAsync(int formId)
    {
        return await _dbSet
            .Where(s => !s.IsDeleted && s.FormId == formId)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Submission>> GetByFormIdAsync(int formId, int organizationId)
    {
        return await _dbSet
            .Where(s => !s.IsDeleted && s.FormId == formId && s.OrganizationId == organizationId)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Submission>> GetByFormIdWithDetailsAsync(int formId, int organizationId)
    {
        return await _dbSet
            .Include(s => s.Form)
            .Include(s => s.Answers)
                .ThenInclude(a => a.AnswerValues)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
            .Where(s => !s.IsDeleted && s.FormId == formId && s.OrganizationId == organizationId)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Submission>> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Where(s => !s.IsDeleted && s.RespondentEmail == email)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Submission>> GetByEmailAsync(string email, int organizationId)
    {
        return await _dbSet
            .Where(s => !s.IsDeleted && s.RespondentEmail == email && s.OrganizationId == organizationId)
            .OrderByDescending(s => s.SubmittedDate)
            .ToListAsync();
    }

    public async Task<PagedResult<FormResponseListDto>> GetPagedResponsesAsync(int formId, ResponseFilterDto filter)
    {
        var query = _dbSet
            .Include(s => s.Form)
            .Where(s => s.FormId == formId && !s.IsDeleted);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filter.RespondentEmail))
        {
            query = query.Where(s => s.RespondentEmail.ToLower().Contains(filter.RespondentEmail.ToLower()));
        }

        if (filter.SubmittedDateFrom.HasValue)
        {
            query = query.Where(s => s.SubmittedDate >= filter.SubmittedDateFrom.Value);
        }

        if (filter.SubmittedDateTo.HasValue)
        {
            query = query.Where(s => s.SubmittedDate <= filter.SubmittedDateTo.Value);
        }

        // Apply sorting - use SortColumn from FilterDto
        var sortColumn = filter.SortColumn?.ToLower() ?? "submitteddate";
        var sortOrder = filter.SortOrder?.ToLower() ?? "desc";

        query = sortColumn switch
        {
            "respondentemail" => sortOrder == "asc" 
                ? query.OrderBy(s => s.RespondentEmail)
                : query.OrderByDescending(s => s.RespondentEmail),
            "totalscore" => sortOrder == "asc"
                ? query.OrderBy(s => s.TotalScore)
                : query.OrderByDescending(s => s.TotalScore),
            _ => sortOrder == "asc"
                ? query.OrderBy(s => s.SubmittedDate)
                : query.OrderByDescending(s => s.SubmittedDate)
        };

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var submissions = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(s => new FormResponseListDto
            {
                SubmissionId = s.Id,
                RespondentEmail = s.RespondentEmail,
                RespondentName = s.RespondentName,
                SubmittedDate = s.SubmittedDate,
                TotalScore = s.TotalScore,
                FormTitle = s.Form.Title
            })
            .ToListAsync();

        return new PagedResult<FormResponseListDto>
        {
            Items = submissions,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<PagedResult<FormResponseListDto>> GetPagedResponsesAsync(int formId, ResponseFilterDto filter, int organizationId)
    {
        var query = _dbSet
            .Include(s => s.Form)
            .Where(s => s.FormId == formId && !s.IsDeleted && s.OrganizationId == organizationId);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filter.RespondentEmail))
        {
            query = query.Where(s => s.RespondentEmail.ToLower().Contains(filter.RespondentEmail.ToLower()));
        }

        if (filter.SubmittedDateFrom.HasValue)
        {
            query = query.Where(s => s.SubmittedDate >= filter.SubmittedDateFrom.Value);
        }

        if (filter.SubmittedDateTo.HasValue)
        {
            query = query.Where(s => s.SubmittedDate <= filter.SubmittedDateTo.Value);
        }

        // Apply sorting - use SortColumn from FilterDto
        var sortColumn = filter.SortColumn?.ToLower() ?? "submitteddate";
        var sortOrder = filter.SortOrder?.ToLower() ?? "desc";

        query = sortColumn switch
        {
            "respondentemail" => sortOrder == "asc" 
                ? query.OrderBy(s => s.RespondentEmail)
                : query.OrderByDescending(s => s.RespondentEmail),
            "totalscore" => sortOrder == "asc"
                ? query.OrderBy(s => s.TotalScore)
                : query.OrderByDescending(s => s.TotalScore),
            _ => sortOrder == "asc"
                ? query.OrderBy(s => s.SubmittedDate)
                : query.OrderByDescending(s => s.SubmittedDate)
        };

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var submissions = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(s => new FormResponseListDto
            {
                SubmissionId = s.Id,
                RespondentEmail = s.RespondentEmail,
                RespondentName = s.RespondentName,
                SubmittedDate = s.SubmittedDate,
                TotalScore = s.TotalScore,
                FormTitle = s.Form.Title
            })
            .ToListAsync();

        return new PagedResult<FormResponseListDto>
        {
            Items = submissions,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<PagedResult<FormResponseListDto>> GetPagedResponsesWithStoredProcedureAsync(int formId, ResponseFilterDto filter, int organizationId)
    {
        // For tests, we'll use the same implementation as the regular method
        // since we don't have stored procedures in test database
        return await GetPagedResponsesAsync(formId, filter, organizationId);
    }

    public async Task<int> UpdateRangeAsync(IEnumerable<Submission> submissions)
    {
        foreach (var submission in submissions)
        {
            submission.UpdatedAt = DateTime.UtcNow;
            Update(submission);
        }
        return await _context.SaveChangesAsync();
    }

    public async Task<int> RecalculateFormScoresAsync(int formId, int organizationId)
    {
        // For tests, simulate the stored procedure by implementing the scoring logic
        var submissions = await GetByFormIdWithDetailsAsync(formId, organizationId);
        
        foreach (var submission in submissions)
        {
            submission.TotalScore = CalculateScoreForSubmission(submission);
            submission.UpdatedAt = DateTime.UtcNow;
            Update(submission);
        }
        
        var updatedCount = submissions.Count();
        if (updatedCount > 0)
        {
            await _context.SaveChangesAsync();
        }
        
        return updatedCount;
    }

    private static decimal CalculateScoreForSubmission(Submission submission)
    {
        decimal totalScore = 0;

        foreach (var answer in submission.Answers)
        {
            var question = answer.Question;
            if (question is null)
            {
                answer.Score = 0;
                continue;
            }

            decimal answerScore = 0;

            // Calculate score based on question type
            foreach (var answerValue in answer.AnswerValues)
            {
                switch (question.QuestionType?.TypeName)
                {
                    case "Slider":
                        // For slider questions, use numeric value directly as score
                        if (answerValue.NumericValue.HasValue)
                        {
                            answerScore += answerValue.NumericValue.Value;
                        }
                        break;

                    case "Radio":
                    case "Dropdown":
                        // For radio/dropdown, use the selected option's score
                        if (answerValue.SelectedOptionId.HasValue)
                        {
                            var selectedOption = question.Options?.FirstOrDefault(o => o.Id == answerValue.SelectedOptionId.Value);
                            if (selectedOption is not null)
                            {
                                answerScore += selectedOption.Score;
                            }
                        }
                        break;

                    case "Multi":
                        // For multi-select, sum all selected option scores
                        if (answerValue.SelectedOptionId.HasValue)
                        {
                            var selectedOption = question.Options?.FirstOrDefault(o => o.Id == answerValue.SelectedOptionId.Value);
                            if (selectedOption is not null)
                            {
                                answerScore += selectedOption.Score;
                            }
                        }
                        break;

                    case "Matrix":
                        // For matrix questions, use the selected column's score
                        if (answerValue.SelectedMatrixColumnId.HasValue)
                        {
                            var selectedColumn = question.MatrixColumns?.FirstOrDefault(c => c.Id == answerValue.SelectedMatrixColumnId.Value);
                            if (selectedColumn is not null)
                            {
                                answerScore += selectedColumn.Score;
                            }
                        }
                        break;

                    case "Text":
                    case "TextArea":
                    case "Date":
                    default:
                        // For text questions or unknown types, score remains 0
                        break;
                }
            }

            // Update the answer's score
            answer.Score = answerScore;
            totalScore += answerScore;
        }

        return totalScore;
    }

    public void Dispose() => _context?.Dispose();
}

public class TestAnswerRepository : IAnswerRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Answer> _dbSet;

    public TestAnswerRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Answer>();
    }

    public void Add(Answer entity) => _dbSet.Add(entity);
    public void AddRange(IEnumerable<Answer> entities) => _dbSet.AddRange(entities);
    public IQueryable<Answer> GetAll() => _dbSet.Where(a => !a.IsDeleted);
    public IQueryable<Answer> GetAllNoTracking() => _dbSet.Where(a => !a.IsDeleted).AsNoTracking();
    
    public async Task<Answer?> GetByIdAsync(int id)
    {
        return await _dbSet.Where(a => !a.IsDeleted).FirstOrDefaultAsync(a => a.Id == id);
    }

    public void Update(Answer entity) => _dbSet.Update(entity);
    public async Task<bool> ExistsAsync(int id) => await _dbSet.Where(a => !a.IsDeleted).AnyAsync(a => a.Id == id);
    public void Remove(Answer entity, bool hardDelete = false)
    {
        if (hardDelete) _dbSet.Remove(entity);
        else { entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; }
    }
    public void RemoveRange(IEnumerable<Answer> entities, bool hardDelete = false)
    {
        if (hardDelete) _dbSet.RemoveRange(entities);
        else foreach (var entity in entities) { entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; }
    }

    public async Task<Answer> CreateAsync(Answer answer)
    {
        Add(answer);
        await _context.SaveChangesAsync();
        return answer;
    }

    public async Task<IEnumerable<Answer>> GetBySubmissionIdAsync(int submissionId)
    {
        return await _dbSet
            .Include(a => a.AnswerValues)
            .Include(a => a.Question)
            .Where(a => !a.IsDeleted && a.SubmissionId == submissionId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Answer>> GetBySubmissionIdAsync(int submissionId, int organizationId)
    {
        return await _dbSet
            .Include(a => a.AnswerValues)
            .Include(a => a.Question)
            .Include(a => a.Submission)
            .Where(a => !a.IsDeleted && a.SubmissionId == submissionId && a.Submission.OrganizationId == organizationId)
            .ToListAsync();
    }

    public async Task<int> CreateRangeAsync(IEnumerable<Answer> answers)
    {
        AddRange(answers);
        return await _context.SaveChangesAsync();
    }

    public void Dispose() => _context?.Dispose();
}

public class TestAnswerValueRepository : IAnswerValueRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<AnswerValue> _dbSet;

    public TestAnswerValueRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<AnswerValue>();
    }

    public void Add(AnswerValue entity) => _dbSet.Add(entity);
    public void AddRange(IEnumerable<AnswerValue> entities) => _dbSet.AddRange(entities);
    public IQueryable<AnswerValue> GetAll() => _dbSet.Where(av => !av.IsDeleted);
    public IQueryable<AnswerValue> GetAllNoTracking() => _dbSet.Where(av => !av.IsDeleted).AsNoTracking();
    
    public async Task<AnswerValue?> GetByIdAsync(int id)
    {
        return await _dbSet.Where(av => !av.IsDeleted).FirstOrDefaultAsync(av => av.Id == id);
    }

    public void Update(AnswerValue entity) => _dbSet.Update(entity);
    public async Task<bool> ExistsAsync(int id) => await _dbSet.Where(av => !av.IsDeleted).AnyAsync(av => av.Id == id);
    public void Remove(AnswerValue entity, bool hardDelete = false)
    {
        if (hardDelete) _dbSet.Remove(entity);
        else { entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; }
    }
    public void RemoveRange(IEnumerable<AnswerValue> entities, bool hardDelete = false)
    {
        if (hardDelete) _dbSet.RemoveRange(entities);
        else foreach (var entity in entities) { entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; }
    }

    public async Task<AnswerValue> CreateAsync(AnswerValue answerValue)
    {
        Add(answerValue);
        await _context.SaveChangesAsync();
        return answerValue;
    }

    public async Task<IEnumerable<AnswerValue>> GetByAnswerIdAsync(int answerId)
    {
        return await _dbSet
            .Where(av => !av.IsDeleted && av.AnswerId == answerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<AnswerValue>> GetByAnswerIdAsync(int answerId, int organizationId)
    {
        return await _dbSet
            .Include(av => av.Answer)
                .ThenInclude(a => a.Submission)
            .Where(av => !av.IsDeleted && av.AnswerId == answerId && av.Answer.Submission.OrganizationId == organizationId)
            .ToListAsync();
    }

    public async Task<int> CreateRangeAsync(IEnumerable<AnswerValue> answerValues)
    {
        AddRange(answerValues);
        return await _context.SaveChangesAsync();
    }

    public void Dispose() => _context?.Dispose();
}

public class TestRuleRepository : IRuleRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<DomainRule> _dbSet;

    public TestRuleRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<DomainRule>();
    }

    public void Add(DomainRule entity) => _dbSet.Add(entity);
    public void AddRange(IEnumerable<DomainRule> entities) => _dbSet.AddRange(entities);
    public IQueryable<DomainRule> GetAll() => _dbSet.Where(r => !r.IsDeleted);
    public IQueryable<DomainRule> GetAllNoTracking() => _dbSet.Where(r => !r.IsDeleted).AsNoTracking();
    
    public async Task<DomainRule?> GetByIdAsync(int id)
    {
        return await _dbSet.Where(r => !r.IsDeleted).FirstOrDefaultAsync(r => r.Id == id);
    }

    public void Update(DomainRule entity) => _dbSet.Update(entity);
    public async Task<bool> ExistsAsync(int id) => await _dbSet.Where(r => !r.IsDeleted).AnyAsync(r => r.Id == id);
    public void Remove(DomainRule entity, bool hardDelete = false)
    {
        if (hardDelete) _dbSet.Remove(entity);
        else { entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; }
    }
    public void RemoveRange(IEnumerable<DomainRule> entities, bool hardDelete = false)
    {
        if (hardDelete) _dbSet.RemoveRange(entities);
        else foreach (var entity in entities) { entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; }
    }

    public async Task<DomainRule> CreateAsync(DomainRule rule)
    {
        rule.CreatedAt = DateTime.UtcNow;
        Add(rule);
        await _context.SaveChangesAsync();
        return rule;
    }

    public async Task<DomainRule> UpdateAsync(DomainRule rule)
    {
        rule.UpdatedAt = DateTime.UtcNow;
        Update(rule);
        await _context.SaveChangesAsync();
        return rule;
    }

    public async Task<bool> DeleteAsync(int ruleId)
    {
        var rule = await GetByIdAsync(ruleId);
        if (rule == null) return false;
        rule.IsDeleted = true;
        rule.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int ruleId, int organizationId)
    {
        var rule = await _dbSet
            .Include(r => r.Form)
            .Where(r => !r.IsDeleted && r.Id == ruleId && r.Form.OrganizationId == organizationId)
            .FirstOrDefaultAsync();
        if (rule == null) return false;
        rule.IsDeleted = true;
        rule.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DomainRule?> GetByIdWithDetailsAsync(int ruleId)
    {
        return await _dbSet
            .Include(r => r.Form)
            .Include(r => r.SourceQuestion)
                .ThenInclude(q => q!.QuestionType)
            .Include(r => r.TriggerOption)
            .Include(r => r.TargetQuestion)
                .ThenInclude(q => q!.QuestionType)
            .Include(r => r.TargetPage)
            .Where(r => !r.IsDeleted)
            .FirstOrDefaultAsync(r => r.Id == ruleId);
    }

    public async Task<DomainRule?> GetByIdWithDetailsAsync(int ruleId, int organizationId)
    {
        return await _dbSet
            .Include(r => r.Form)
            .Include(r => r.SourceQuestion)
                .ThenInclude(q => q!.QuestionType)
            .Include(r => r.TriggerOption)
            .Include(r => r.TargetQuestion)
                .ThenInclude(q => q!.QuestionType)
            .Include(r => r.TargetPage)
            .Where(r => !r.IsDeleted && r.Form.OrganizationId == organizationId)
            .FirstOrDefaultAsync(r => r.Id == ruleId);
    }

    public async Task<IEnumerable<DomainRule>> GetByFormIdAsync(int formId)
    {
        return await _dbSet
            .Where(r => !r.IsDeleted && r.FormId == formId)
            .ToListAsync();
    }

    public async Task<IEnumerable<DomainRule>> GetByFormIdAsync(int formId, int organizationId)
    {
        return await _dbSet
            .Include(r => r.Form)
            .Where(r => !r.IsDeleted && r.FormId == formId && r.Form.OrganizationId == organizationId)
            .ToListAsync();
    }

    public async Task<IEnumerable<DomainRule>> GetByFormIdWithDetailsAsync(int formId)
    {
        return await _dbSet
            .Include(r => r.SourceQuestion)
                .ThenInclude(q => q!.QuestionType)
            .Include(r => r.TriggerOption)
            .Include(r => r.TargetQuestion)
                .ThenInclude(q => q!.QuestionType)
            .Include(r => r.TargetPage)
            .Where(r => !r.IsDeleted && r.FormId == formId)
            .ToListAsync();
    }

    public async Task<IEnumerable<DomainRule>> GetByFormIdWithDetailsAsync(int formId, int organizationId)
    {
        return await _dbSet
            .Include(r => r.Form)
            .Include(r => r.SourceQuestion)
                .ThenInclude(q => q!.QuestionType)
            .Include(r => r.TriggerOption)
            .Include(r => r.TargetQuestion)
                .ThenInclude(q => q!.QuestionType)
            .Include(r => r.TargetPage)
            .Where(r => !r.IsDeleted && r.FormId == formId && r.Form.OrganizationId == organizationId)
            .ToListAsync();
    }

    public async Task<bool> RuleExistsAsync(int formId, int sourceQuestionId, int? triggerOptionId, string condition)
    {
        return await _dbSet
            .Where(r => !r.IsDeleted)
            .AnyAsync(r => r.FormId == formId && 
                          r.SourceQuestionId == sourceQuestionId && 
                          r.TriggerOptionId == triggerOptionId && 
                          r.Condition == condition);
    }

    public async Task<bool> RuleExistsAsync(int formId, int sourceQuestionId, int? triggerOptionId, string condition, int organizationId)
    {
        return await _dbSet
            .Include(r => r.Form)
            .Where(r => !r.IsDeleted && r.Form.OrganizationId == organizationId)
            .AnyAsync(r => r.FormId == formId && 
                          r.SourceQuestionId == sourceQuestionId && 
                          r.TriggerOptionId == triggerOptionId && 
                          r.Condition == condition);
    }

    public void Dispose() => _context?.Dispose();
}

public class TestQuestionRepository : IQuestionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Question> _dbSet;

    public TestQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Question>();
    }

    public void Add(Question entity) => _dbSet.Add(entity);
    public void AddRange(IEnumerable<Question> entities) => _dbSet.AddRange(entities);
    public IQueryable<Question> GetAll() => _dbSet.Where(q => !q.IsDeleted);
    public IQueryable<Question> GetAllNoTracking() => _dbSet.Where(q => !q.IsDeleted).AsNoTracking();

    public async Task<Question?> GetByIdAsync(int id)
    {
        return await _dbSet.Where(q => !q.IsDeleted).FirstOrDefaultAsync(q => q.Id == id);
    }

    public void Update(Question entity) => _dbSet.Update(entity);
    public async Task<bool> ExistsAsync(int id) => await _dbSet.Where(q => !q.IsDeleted).AnyAsync(q => q.Id == id);
    public void Remove(Question entity, bool hardDelete = false)
    {
        if (hardDelete) _dbSet.Remove(entity);
        else { entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; }
    }
    public void RemoveRange(IEnumerable<Question> entities, bool hardDelete = false)
    {
        if (hardDelete) _dbSet.RemoveRange(entities);
        else foreach (var entity in entities) { entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow; }
    }

    public async Task<Question> CreateAsync(Question question)
    {
        Add(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task<IEnumerable<Question>> GetByPageIdAsync(int pageId)
    {
        return await _dbSet
            .Include(q => q.Options)
            .Include(q => q.SliderConfig)
            .Include(q => q.MatrixRows)
            .Include(q => q.MatrixColumns)
            .Where(q => !q.IsDeleted && q.PageId == pageId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetByPageIdAsync(int pageId, int organizationId)
    {
        return await _dbSet
            .Include(q => q.Options)
            .Include(q => q.SliderConfig)
            .Include(q => q.MatrixRows)
            .Include(q => q.MatrixColumns)
            .Include(q => q.Page)
                .ThenInclude(p => p.Form)
            .Where(q => !q.IsDeleted && q.PageId == pageId && q.Page.Form.OrganizationId == organizationId)
            .ToListAsync();
    }

    public async Task<Question?> GetByIdWithMatrixColumnsAsync(int questionId)
    {
        return await _dbSet
            .Include(q => q.MatrixColumns)
            .Where(q => !q.IsDeleted)
            .FirstOrDefaultAsync(q => q.Id == questionId);
    }

    public async Task<Question?> GetByIdWithMatrixColumnsAsync(int questionId, int organizationId)
    {
        return await _dbSet
            .Include(q => q.MatrixColumns)
            .Include(q => q.Page)
                .ThenInclude(p => p.Form)
            .Where(q => !q.IsDeleted && q.Page.Form.OrganizationId == organizationId)
            .FirstOrDefaultAsync(q => q.Id == questionId);
    }

    public async Task<Question> UpdateAsync(Question question)
    {
        question.UpdatedAt = DateTime.UtcNow;
        Update(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task<bool> DeleteAsync(int questionId)
    {
        var question = await GetByIdAsync(questionId);
        if (question == null) return false;
        question.IsDeleted = true;
        question.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int questionId, int organizationId)
    {
        var question = await _dbSet
            .Include(q => q.Page)
                .ThenInclude(p => p.Form)
            .Where(q => !q.IsDeleted && q.Id == questionId && q.Page.Form.OrganizationId == organizationId)
            .FirstOrDefaultAsync();
        if (question == null) return false;
        question.IsDeleted = true;
        question.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public void Dispose() => _context?.Dispose();
}

public abstract class DynamicQuestionnaireTestBase : IDisposable
{
    protected readonly ApplicationDbContext DbContext;
    protected readonly IMapper Mapper;
    protected readonly ServiceProvider ServiceProvider;
    protected readonly Mock<IDbConnectionFactory> MockDbConnectionFactory;

    protected DynamicQuestionnaireTestBase()
    {
        var services = new ServiceCollection();
        
        // Add DbContext with InMemory database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

        // Add AutoMapper
        services.AddAutoMapper(typeof(App.Application.Profiles.MappingProfile));

        // Add logging
        services.AddLogging();

        // Setup mock connection factory for in-memory testing
        MockDbConnectionFactory = new Mock<IDbConnectionFactory>();
        var mockConnection = new Mock<IDbConnection>();
        mockConnection.Setup(x => x.ConnectionString).Returns("InMemoryDb");
        MockDbConnectionFactory.Setup(x => x.CreateConnection(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(mockConnection.Object);
        
        // Register test-friendly services and repositories
        services.AddSingleton(MockDbConnectionFactory.Object);
        services.AddScoped<IFormRepository, TestFormRepository>();
        services.AddScoped<IMasterQuestionTypeRepository, TestMasterQuestionTypeRepository>();
        services.AddScoped<IQuestionRepository, TestQuestionRepository>();
        services.AddScoped<ISubmissionRepository, TestSubmissionRepository>();
        services.AddScoped<IAnswerRepository, TestAnswerRepository>();
        services.AddScoped<IAnswerValueRepository, TestAnswerValueRepository>();
        services.AddScoped<IRuleRepository, TestRuleRepository>();
        services.AddScoped<IDynamicQuestionnaireService, DynamicQuestionnaireService>();
        services.AddScoped<IScoringEngineService, ScoringEngineService>();
        services.AddScoped<IExportService, ExportService>();

        ServiceProvider = services.BuildServiceProvider();
        DbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Mapper = ServiceProvider.GetRequiredService<IMapper>();

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        // Seed MasterQuestionTypes
        if (!DbContext.MasterQuestionTypes.Any())
        {
            var questionTypes = new List<MasterQuestionType>
            {
                new() { TypeName = "Multi", IsActive = true },
                new() { TypeName = "Radio", IsActive = true },
                new() { TypeName = "Dropdown", IsActive = true },
                new() { TypeName = "Slider", IsActive = true },
                new() { TypeName = "Matrix", IsActive = true },
                new() { TypeName = "Text", IsActive = true },
                new() { TypeName = "TextArea", IsActive = true },
                new() { TypeName = "Date", IsActive = true }
            };

            DbContext.MasterQuestionTypes.AddRange(questionTypes);
            DbContext.SaveChanges();
        }
    }

    protected Form CreateTestForm(string title = "Test Form")
    {
        return new Form
        {
            Title = title,
            Description = "Test Description",
            Instructions = "Test Instructions",
            AllowResubmission = false,
            OrganizationId = 1, // Add organization ID for multi-tenant support
            CreatedBy = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    protected Page CreateTestPage(int formId, int pageOrder = 1)
    {
        return new Page
        {
            FormId = formId,
            Title = "Test Page",
            Description = "Test Page Description",
            PageOrder = pageOrder,
            CreatedBy = 1
        };
    }

    protected Question CreateTestQuestion(int pageId, QuestionType questionType = QuestionType.Radio, int questionOrder = 1)
    {
        return new Question
        {
            PageId = pageId,
            QuestionTypeId = (int)questionType,
            QuestionText = "Test Question",
            IsRequired = true,
            QuestionOrder = questionOrder,
            CreatedBy = 1
        };
    }

    protected Option CreateTestOption(int questionId, int displayOrder = 1, string text = "Test Option")
    {
        return new Option
        {
            QuestionId = questionId,
            OptionText = text,
            DisplayOrder = displayOrder,
            Score = 1.0m,
            CreatedBy = 1
        };
    }

    protected SliderConfig CreateTestSliderConfig(int questionId)
    {
        return new SliderConfig
        {
            QuestionId = questionId,
            MinValue = 0,
            MaxValue = 100,
            StepValue = 1,
            MinLabel = "Min",
            MaxLabel = "Max",
            CreatedBy = 1
        };
    }

    protected MatrixRow CreateTestMatrixRow(int questionId, int displayOrder = 1, string label = "Test Row")
    {
        return new MatrixRow
        {
            QuestionId = questionId,
            RowLabel = label,
            DisplayOrder = displayOrder,
            CreatedBy = 1
        };
    }

    protected MatrixColumn CreateTestMatrixColumn(int questionId, int displayOrder = 1, string label = "Test Column")
    {
        return new MatrixColumn
        {
            QuestionId = questionId,
            ColumnLabel = label,
            DisplayOrder = displayOrder,
            CreatedBy = 1
        };
    }

    protected CreateFormDto CreateTestFormDto()
    {
        return new CreateFormDto
        {
            Title = "Test Form",
            Description = "Test Description",
            Instructions = "Test Instructions",
            AllowResubmission = false,
            Pages = new List<CreatePageDto>
            {
                new CreatePageDto
                {
                    Title = "Test Page",
                    Description = "Test Page Description",
                    PageOrder = 1,
                    Questions = new List<CreateQuestionDto>
                    {
                        new CreateQuestionDto
                        {
                            QuestionText = "Test Question",
                            IsRequired = true,
                            QuestionOrder = 1,
                            QuestionTypeId = (int)QuestionType.Radio,
                            Options = new List<CreateOptionDto>
                            {
                                new CreateOptionDto
                                {
                                    OptionText = "Test Option",
                                    DisplayOrder = 1,
                                    Score = 1.0m
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    public void Dispose()
    {
        DbContext?.Dispose();
        ServiceProvider?.Dispose();
    }
}