using App.Application.Dto.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using AutoMapper;
namespace App.Application.Profiles;

public class DynamicQuestionnaireProfile : Profile
{
    public DynamicQuestionnaireProfile()
    {
        // Create Form Mappings
        CreateMap<CreateFormDto, Form>()
            .ForMember(dest => dest.Pages, opt => opt.MapFrom(src => src.Pages))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsPublished, opt => opt.Ignore());

        CreateMap<CreatePageDto, Page>()
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FormId, opt => opt.Ignore())
            .ForMember(dest => dest.Form, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<CreateQuestionDto, Question>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
            .ForMember(dest => dest.SliderConfig, opt => opt.MapFrom(src => src.SliderConfig))
            .ForMember(dest => dest.MatrixRows, opt => opt.MapFrom(src => src.MatrixConfig != null ? src.MatrixConfig.Rows : null))
            .ForMember(dest => dest.MatrixColumns, opt => opt.MapFrom(src => src.MatrixConfig != null ? src.MatrixConfig.Columns : null))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PageId, opt => opt.Ignore())
            .ForMember(dest => dest.Page, opt => opt.Ignore())
            .ForMember(dest => dest.QuestionType, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<CreateOptionDto, Option>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
            .ForMember(dest => dest.Question, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // Matrix and Slider Configuration Mappings
        CreateMap<CreateSliderConfigDto, SliderConfig>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
            .ForMember(dest => dest.Question, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        CreateMap<CreateMatrixRowDto, MatrixRow>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
            .ForMember(dest => dest.Question, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        CreateMap<CreateMatrixColumnDto, MatrixColumn>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
            .ForMember(dest => dest.Question, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        // Update Form Mappings
        CreateMap<UpdateFormDto, Form>()
            //.ForMember(dest => dest.Pages, opt => opt.MapFrom(src => src.Pages))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsPublished, opt => opt.Ignore())
            .ForMember(dest => dest.Pages, opt => opt.Ignore());

        CreateMap<UpdatePageDto, Page>()
            //.ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions))
            .ForMember(dest => dest.FormId, opt => opt.Ignore())
            .ForMember(dest => dest.Form, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Questions, opt => opt.Ignore()); 

        CreateMap<UpdateQuestionDto, Question>()
            // ignore nested collections and configs to preserve existing tracked entities
            .ForMember(dest => dest.SliderConfig, opt => opt.Ignore())
            .ForMember(dest => dest.MatrixRows, opt => opt.Ignore())
            .ForMember(dest => dest.MatrixColumns, opt => opt.Ignore())
            .ForMember(dest => dest.PageId, opt => opt.Ignore())
            .ForMember(dest => dest.Page, opt => opt.Ignore())
            .ForMember(dest => dest.QuestionType, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Options, opt => opt.Ignore());

        CreateMap<UpdateOptionDto, Option>()
            .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
            .ForMember(dest => dest.Question, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<UpdateSliderConfigDto, SliderConfig>()
            .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
            .ForMember(dest => dest.Question, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        CreateMap<UpdateMatrixRowDto, MatrixRow>()
            .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
            .ForMember(dest => dest.Question, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        CreateMap<UpdateMatrixColumnDto, MatrixColumn>()
            .ForMember(dest => dest.QuestionId, opt => opt.Ignore())
            .ForMember(dest => dest.Question, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        // Response Mappings
        CreateMap<Form, FormResponseDto>()
            .ForMember(dest => dest.FormId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Pages, opt => opt.MapFrom(src => src.Pages));

        CreateMap<Page, PageResponseDto>()
            .ForMember(dest => dest.PageId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));

        CreateMap<Question, QuestionResponseDto>()
            .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.QuestionTypeName, opt => opt.MapFrom(src => src.QuestionType.TypeName))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
            .ForMember(dest => dest.SliderConfig, opt => opt.MapFrom(src => src.SliderConfig))
            .ForMember(dest => dest.MatrixRows, opt => opt.MapFrom(src => src.MatrixRows))
            .ForMember(dest => dest.MatrixColumns, opt => opt.MapFrom(src => src.MatrixColumns));

        CreateMap<Option, OptionResponseDto>()
            .ForMember(dest => dest.OptionId, opt => opt.MapFrom(src => src.Id));

        CreateMap<SliderConfig, SliderConfigResponseDto>()
            .ForMember(dest => dest.SliderConfigId, opt => opt.MapFrom(src => src.Id));

        CreateMap<MatrixRow, MatrixRowResponseDto>()
            .ForMember(dest => dest.MatrixRowId, opt => opt.MapFrom(src => src.Id));

        CreateMap<MatrixColumn, MatrixColumnResponseDto>()
            .ForMember(dest => dest.MatrixColumnId, opt => opt.MapFrom(src => src.Id));

        CreateMap<MasterQuestionType, MasterQuestionTypeDto>()
            .ForMember(dest => dest.QuestionTypeId, opt => opt.MapFrom(src => src.Id));

        // Public Form Mappings
        CreateMap<Form, PublicFormResponseDto>()
            .ForMember(dest => dest.FormId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Pages, opt => opt.MapFrom(src => src.Pages.OrderBy(p => p.PageOrder)));

        CreateMap<Page, PublicPageDto>()
            .ForMember(dest => dest.PageId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions.OrderBy(q => q.QuestionOrder)));

        CreateMap<Question, PublicQuestionDto>()
            .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.QuestionType.TypeName))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options.OrderBy(o => o.DisplayOrder)))
            .ForMember(dest => dest.MatrixRows, opt => opt.MapFrom(src => src.MatrixRows.OrderBy(r => r.DisplayOrder)))
            .ForMember(dest => dest.MatrixColumns, opt => opt.MapFrom(src => src.MatrixColumns.OrderBy(c => c.DisplayOrder)));

        CreateMap<Option, PublicOptionDto>()
            .ForMember(dest => dest.OptionId, opt => opt.MapFrom(src => src.Id));

        CreateMap<SliderConfig, PublicSliderConfigDto>();

        CreateMap<MatrixRow, PublicMatrixRowDto>()
            .ForMember(dest => dest.MatrixRowId, opt => opt.MapFrom(src => src.Id));

        CreateMap<MatrixColumn, PublicMatrixColumnDto>()
            .ForMember(dest => dest.MatrixColumnId, opt => opt.MapFrom(src => src.Id));

        // Phase 4: Submission Mappings
        CreateMap<Submission, SubmissionResponseDto>();

        CreateMap<SubmitFormRequest, Submission>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FormId, opt => opt.Ignore())
            .ForMember(dest => dest.Form, opt => opt.Ignore())
            .ForMember(dest => dest.SubmittedDate, opt => opt.Ignore())
            .ForMember(dest => dest.TotalScore, opt => opt.Ignore())
            .ForMember(dest => dest.Answers, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<SubmissionAnswerDto, Answer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SubmissionId, opt => opt.Ignore())
            .ForMember(dest => dest.Submission, opt => opt.Ignore())
            .ForMember(dest => dest.Question, opt => opt.Ignore())
            .ForMember(dest => dest.Score, opt => opt.Ignore())
            .ForMember(dest => dest.AnswerValues, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<AnswerValueDto, AnswerValue>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AnswerId, opt => opt.Ignore())
            .ForMember(dest => dest.Answer, opt => opt.Ignore())
            .ForMember(dest => dest.SelectedOption, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // Phase 6: Rule Mappings
        CreateMap<CreateRuleRequest, Rule>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Form, opt => opt.Ignore())
            .ForMember(dest => dest.SourceQuestion, opt => opt.Ignore())
            .ForMember(dest => dest.TriggerOption, opt => opt.Ignore())
            .ForMember(dest => dest.TargetQuestion, opt => opt.Ignore())
            .ForMember(dest => dest.TargetPage, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<Rule, RuleResponseDto>()
            .ForMember(dest => dest.RuleId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SourceQuestionText, opt => opt.MapFrom(src => src.SourceQuestion != null ? src.SourceQuestion.QuestionText : string.Empty))
            .ForMember(dest => dest.TriggerOptionText, opt => opt.MapFrom(src => src.TriggerOption != null ? src.TriggerOption.OptionText : null))
            .ForMember(dest => dest.TargetQuestionText, opt => opt.MapFrom(src => src.TargetQuestion != null ? src.TargetQuestion.QuestionText : null))
            .ForMember(dest => dest.TargetPageTitle, opt => opt.MapFrom(src => src.TargetPage != null ? src.TargetPage.Title : null));

        CreateMap<UpdateRuleRequest, Rule>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FormId, opt => opt.Ignore())
            .ForMember(dest => dest.SourceQuestionId, opt => opt.Ignore())
            .ForMember(dest => dest.Form, opt => opt.Ignore())
            .ForMember(dest => dest.SourceQuestion, opt => opt.Ignore())
            .ForMember(dest => dest.TriggerOption, opt => opt.Ignore())
            .ForMember(dest => dest.TargetQuestion, opt => opt.Ignore())
            .ForMember(dest => dest.TargetPage, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}
