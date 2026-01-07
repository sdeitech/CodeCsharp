using App.Application.Dto.Common;
using App.Application.Dto.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;

namespace App.Application.Interfaces.Repositories.DynamicQuestionnaire;

public interface IFormRepository : IRepository<Form>
{
    Task<Form?> GetByIdWithDetailsAsync(int formId, int organizationId);
    Task<Form?> GetByPublicKeyWithDetailsAsync(string publicKey);
    Task<bool> PublicKeyExistsAsync(string publicKey);
    Task<Form> CreateAsync(Form form);
    Task<Form> UpdateAsync(Form form);
    Task<bool> DeleteAsync(int formId, int organizationId);
    Task<IEnumerable<Form>> GetAllAsync(int organizationId);
    Task<FormPagedResult<FormResponseDto>> GetPagedFormsAsync(FormFilterDto filter, int organizationId);
}

public interface IPageRepository : IRepository<Page>
{
    Task<IEnumerable<Page>> GetByFormIdAsync(int formId, int organizationId);
    Task<Page> CreateAsync(Page page);
    Task<Page> UpdateAsync(Page page);
    Task<bool> DeleteAsync(int pageId, int organizationId);
}

public interface IQuestionRepository : IRepository<Question>
{
    Task<IEnumerable<Question>> GetByPageIdAsync(int pageId, int organizationId);
    Task<Question?> GetByIdWithMatrixColumnsAsync(int questionId, int organizationId);
    Task<Question> CreateAsync(Question question);
    Task<Question> UpdateAsync(Question question);
    Task<bool> DeleteAsync(int questionId, int organizationId);
}

public interface IOptionRepository : IRepository<Option>
{
    Task<IEnumerable<Option>> GetByQuestionIdAsync(int questionId, int organizationId);
    Task<Option> CreateAsync(Option option);
    Task<Option> UpdateAsync(Option option);
    Task<bool> DeleteAsync(int optionId, int organizationId);
}

public interface IMasterQuestionTypeRepository
{
    Task<MasterQuestionType?> GetByIdAsync(int questionTypeId);
    Task<IEnumerable<MasterQuestionType>> GetAllAsync();
    Task<MasterQuestionType?> GetByTypeNameAsync(string typeName);
}

public interface ISubmissionRepository : IRepository<Submission>
{
    Task<Submission> CreateAsync(Submission submission);
    Task<Submission> UpdateAsync(Submission submission);
    Task<Submission?> GetByIdWithDetailsAsync(int submissionId, int organizationId);
    Task<bool> HasPreviousSubmissionAsync(int formId, string email, int organizationId);
    Task<IEnumerable<Submission>> GetByFormIdAsync(int formId, int organizationId);
    Task<IEnumerable<Submission>> GetByFormIdWithDetailsAsync(int formId, int organizationId);
    Task<IEnumerable<Submission>> GetByEmailAsync(string email, int organizationId);
    Task<PagedResult<FormResponseListDto>> GetPagedResponsesAsync(int formId, ResponseFilterDto filter, int organizationId);
    Task<int> UpdateRangeAsync(IEnumerable<Submission> submissions);
    Task<int> RecalculateFormScoresAsync(int formId, int organizationId);
}

public interface IAnswerRepository : IRepository<Answer>
{
    Task<Answer> CreateAsync(Answer answer);
    Task<IEnumerable<Answer>> GetBySubmissionIdAsync(int submissionId, int organizationId);
    Task<int> CreateRangeAsync(IEnumerable<Answer> answers);
}

public interface IAnswerValueRepository : IRepository<AnswerValue>
{
    Task<AnswerValue> CreateAsync(AnswerValue answerValue);
    Task<IEnumerable<AnswerValue>> GetByAnswerIdAsync(int answerId, int organizationId);
    Task<int> CreateRangeAsync(IEnumerable<AnswerValue> answerValues);
}

public interface IRuleRepository : IRepository<Rule>
{
    Task<Rule> CreateAsync(Rule rule);
    Task<Rule> UpdateAsync(Rule rule);
    Task<bool> DeleteAsync(int ruleId, int organizationId);
    Task<Rule?> GetByIdWithDetailsAsync(int ruleId, int organizationId);
    Task<IEnumerable<Rule>> GetByFormIdAsync(int formId, int organizationId);
    Task<IEnumerable<Rule>> GetByFormIdWithDetailsAsync(int formId, int organizationId);
    Task<bool> RuleExistsAsync(int formId, int sourceQuestionId, int? triggerOptionId, string condition, int organizationId);
}
