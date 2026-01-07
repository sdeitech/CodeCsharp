using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Dto.Common;
using App.Common.Models;

namespace App.Application.Interfaces.Services.DynamicQuestionnaire;

public interface IDynamicQuestionnaireService
{
    Task<JsonModel> CreateFormAsync(CreateFormDto createFormDto, int userId);
    Task<JsonModel> UpdateFormAsync(int formId, UpdateFormDto updateFormDto, int userId);
    Task<JsonModel> GetFormByIdAsync(int formId);
    Task<JsonModel> GetAllFormsAsync();
    Task<JsonModel> GetPagedFormsAsync(FormFilterDto filter);
    Task<JsonModel> GetMasterQuestionTypesAsync();
    Task<JsonModel> PublishFormAsync(int formId, int userId);
    Task<JsonModel> GetPublicFormAsync(string publicKey);
    Task<JsonModel> SubmitFormResponseAsync(string publicKey, SubmitFormRequest request);
    Task<JsonModel> DeleteFormAsync(int formId, int userId);
    
    // Phase 5: Admin Submission Review APIs
    Task<JsonModel> GetFormSubmissionsAsync(int formId, ResponseFilterDto filter);
    Task<JsonModel> GetSubmissionByIdAsync(int submissionId);
    
    // Phase 6: Conditional Logic Rules APIs
    Task<JsonModel> CreateRuleAsync(CreateRuleRequest request, int userId);
    Task<JsonModel> UpdateRuleAsync(UpdateRuleRequest request, int userId);
    Task<JsonModel> DeleteRuleAsync(int ruleId, int userId);
    Task<JsonModel> GetFormRulesAsync(int formId);
    Task<JsonModel> GetRuleByIdAsync(int ruleId);
    Task<JsonModel> EvaluateRulesAsync(RuleEvaluationRequest request);
    
    // Phase 7: Test Mode APIs for Preview
    Task<JsonModel> SubmitTestModeAsync(TestModeSubmissionRequest request);
    Task<JsonModel> EvaluateTestModeRulesAsync(TestModeSubmissionRequest request);
    Task<JsonModel> CalculateTestModeScoreAsync(TestModeSubmissionRequest request);
}
