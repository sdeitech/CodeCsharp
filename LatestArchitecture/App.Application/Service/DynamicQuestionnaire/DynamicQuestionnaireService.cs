using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Dto.Common;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Common.Models;
using App.Domain.Entities.DynamicQuestionnaire;
using AutoMapper;
using Microsoft.Extensions.Logging;
using App.Common.Constant;
using App.Application.Constant.DynamicQuestionnaire;
using System.Net;
using Microsoft.AspNetCore.Http;


namespace App.Application.Service.DynamicQuestionnaire;

public class DynamicQuestionnaireService : IDynamicQuestionnaireService
{
    private readonly IFormRepository _formRepository;
    private readonly IMasterQuestionTypeRepository _masterQuestionTypeRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAnswerRepository _answerRepository;
    private readonly IAnswerValueRepository _answerValueRepository;
    private readonly IRuleRepository _ruleRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DynamicQuestionnaireService> _logger;
    private readonly IScoringEngineService _scoringEngineService;
    private readonly ICurrentUserClaimService _currentUserClaimService;

    public DynamicQuestionnaireService(
        IFormRepository formRepository,
        IMasterQuestionTypeRepository masterQuestionTypeRepository,
        ISubmissionRepository submissionRepository,
        IAnswerRepository answerRepository,
        IAnswerValueRepository answerValueRepository,
        IRuleRepository ruleRepository,
        IQuestionRepository questionRepository,
        IMapper mapper,
        IScoringEngineService scoringEngineService,
        ICurrentUserClaimService currentUserClaimService,
        ILogger<DynamicQuestionnaireService> logger)
    {
        _formRepository = formRepository;
        _masterQuestionTypeRepository = masterQuestionTypeRepository;
        _submissionRepository = submissionRepository;
        _answerRepository = answerRepository;
        _answerValueRepository = answerValueRepository;
        _ruleRepository = ruleRepository;
        _questionRepository = questionRepository;
        _mapper = mapper;
        _scoringEngineService = scoringEngineService;
        _currentUserClaimService = currentUserClaimService;
        _logger = logger;
    }

    public async Task<JsonModel> CreateFormAsync(CreateFormDto createFormDto, int userId)
    {
        // Add basic validation - throw immediately for invalid input
        if (string.IsNullOrWhiteSpace(createFormDto?.Title))
        {
            throw new ArgumentException(StatusMessage.FormTitleCannotBeNullOrWhitespace, nameof(createFormDto));
        }

        // Get organization ID from current user claims
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            throw new UnauthorizedAccessException("User must belong to an organization to create forms");
        }

        _logger.LogInformation("Creating new form with title: {Title} for organization: {OrganizationId}", createFormDto.Title, organizationId.Value);

        // Map DTO to entity
        var form = _mapper.Map<Form>(createFormDto);
        form.CreatedBy = userId;
        form.CreatedAt = DateTime.UtcNow;
        form.OrganizationId = organizationId.Value;

        // Set audit fields for nested entities
        foreach (var page in form.Pages)
        {
            page.CreatedBy = userId;
            page.OrganizationId = organizationId.Value;

            page.Questions.ToList().ForEach(question =>
            {
                question.CreatedBy = userId;
                question.OrganizationId = organizationId.Value;
                question.Options.ToList().ForEach(option => {
                    option.CreatedBy = userId;
                    option.OrganizationId = organizationId.Value;
                });
                question.MatrixRows.ToList().ForEach(row => {
                    row.CreatedBy = userId;
                    row.OrganizationId = organizationId.Value;
                });
                question.MatrixColumns.ToList().ForEach(col => {
                    col.CreatedBy = userId;
                    col.OrganizationId = organizationId.Value;
                });

                if (question.SliderConfig is not null)
                {
                    question.SliderConfig.CreatedBy = userId;
                    question.SliderConfig.OrganizationId = organizationId.Value;
                }
            });
        }

        // Create the form
        var createdForm = await _formRepository.CreateAsync(form);

        // Get the created form with details for response
        var formWithDetails = await _formRepository.GetByIdWithDetailsAsync(createdForm.Id, organizationId.Value);
        var response = _mapper.Map<FormResponseDto>(formWithDetails);

        _logger.LogInformation("Successfully created form with ID: {FormId}", createdForm.Id);

        return new JsonModel(response, StatusMessage.FormCreatedSuccessfully, (int)HttpStatusCode.Created);
    }

    public async Task<JsonModel> UpdateFormAsync(int formId, UpdateFormDto updateFormDto, int userId)
    {
        // Add basic validation - throw immediately for invalid input
        if (string.IsNullOrWhiteSpace(updateFormDto?.Title))
        {
            throw new ArgumentException(StatusMessage.FormTitleCannotBeNullOrWhitespace, nameof(updateFormDto));
        }

        // Get organization ID from current user claims
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel(responseData: null!, "User must belong to an organization", StatusCodes.Status401Unauthorized);
        }

        _logger.LogInformation("Updating form with ID: {FormId} for organization: {OrganizationId}", formId, organizationId.Value);

        // Get existing form (this will be tracked by EF Core) with organization filtering
        var existingForm = await _formRepository.GetByIdWithDetailsAsync(formId, organizationId.Value);
        if (existingForm is null)
        {
            return new JsonModel(responseData: null!, StatusMessage.FormNotFound, StatusCodes.Status404NotFound);
        }

        // Update basic form properties
        //existingForm.Title = updateFormDto.Title;
        //existingForm.Description = updateFormDto.Description;
        //existingForm.Instructions = updateFormDto.Instructions;
        //existingForm.AllowResubmission = updateFormDto.AllowResubmission;

        _mapper.Map(updateFormDto, existingForm); //Source to Destination mapping

        existingForm.UpdatedBy = userId;
        existingForm.UpdatedAt = DateTime.UtcNow;

        // Handle page updates/additions/deletions
        await UpdatePagesAsync(existingForm, updateFormDto.Pages, userId, organizationId.Value);

        // Update the form (existing entity is already tracked)
        var updatedForm = await _formRepository.UpdateAsync(existingForm);

        // Get the updated form with details for response
        var formWithDetails = await _formRepository.GetByIdWithDetailsAsync(updatedForm.Id, organizationId.Value);
        var response = _mapper.Map<FormResponseDto>(formWithDetails);

        _logger.LogInformation("Successfully updated form with ID: {FormId}", formId);

        return new JsonModel(response, StatusMessage.FormUpdatedSuccessfully, StatusCodes.Status200OK);
    }

    private Task UpdatePagesAsync(Form existingForm, List<UpdatePageDto> updatePages, int userId, int organizationId)
    {
        // Get existing pages for comparison
        var existingPages = existingForm.Pages.ToList();
        var updatedPageIds = updatePages.Where(p => p.Id.HasValue).Select(p => p.Id!.Value).ToList();

        // Remove pages that are no longer in the update request
        var pagesToRemove = existingPages.Where(p => !updatedPageIds.Contains(p.Id)).ToList();
        foreach (var pageToRemove in pagesToRemove)
        {
            existingForm.Pages.Remove(pageToRemove);
        }

        // Process each page in the update request
        foreach (var updatePage in updatePages)
        {
            if (updatePage.Id.HasValue)
            {
                // Update existing page
                var existingPage = existingPages.FirstOrDefault(p => p.Id == updatePage.Id.Value);
                if (existingPage is not null)
                {
                    _mapper.Map(updatePage, existingPage);
                    existingPage.UpdatedBy = userId;
                    existingPage.UpdatedAt = DateTime.UtcNow;

                    UpdateQuestionsAsync(existingPage, updatePage.Questions, userId, organizationId);
                }
            }
            else
            {
                // Add new page
                var newPage = _mapper.Map<Page>(updatePage);
                newPage.FormId = existingForm.Id;
                newPage.OrganizationId = organizationId;
                SetAuditFieldsForNewEntity(newPage, userId);
                SetAuditFieldsForNewPageChildren(newPage, userId, organizationId);
                existingForm.Pages.Add(newPage);
            }
        }

        return Task.CompletedTask;
    }

    private void UpdateQuestionsAsync(Page existingPage, List<UpdateQuestionDto> updateQuestions, int userId, int organizationId)
    {
        var existingQuestions = existingPage.Questions.ToList();
        var updatedQuestionIds = updateQuestions.Where(q => q.Id.HasValue).Select(q => q.Id!.Value).ToList();

        // Remove questions that are no longer in the update request
        var questionsToRemove = existingQuestions.Where(q => !updatedQuestionIds.Contains(q.Id)).ToList();
        foreach (var questionToRemove in questionsToRemove)
        {
            existingPage.Questions.Remove(questionToRemove);
        }

        // Process each question in the update request
        foreach (var updateQuestion in updateQuestions)
        {
            if (updateQuestion.Id.HasValue)
            {
                // Update existing question
                var existingQuestion = existingQuestions.FirstOrDefault(q => q.Id == updateQuestion.Id.Value);
                if (existingQuestion is not null)
                {
                    // Manually update basic question properties to avoid AutoMapper issues with collections
                    _mapper.Map(updateQuestion, existingQuestion);

                    UpdateOptionsAsync(existingQuestion, updateQuestion.Options, userId, organizationId);

                    if (updateQuestion.MatrixConfig is not null)
                        UpdateMatrixConfigAsync(existingQuestion, updateQuestion.MatrixConfig, userId, organizationId);

                    if (updateQuestion.SliderConfig is not null)
                        UpdateSliderConfigAsync(existingQuestion, updateQuestion.SliderConfig, userId, organizationId);
                    else
                        existingQuestion.SliderConfig = null;

                }
            }
            else
            {
                // Add new question
                var newQuestion = _mapper.Map<Question>(updateQuestion);
                newQuestion.PageId = existingPage.Id;
                if (updateQuestion.MatrixConfig is not null)
                {
                    newQuestion.MatrixRows = _mapper.Map<List<MatrixRow>>(updateQuestion.MatrixConfig.Rows);
                    newQuestion.MatrixColumns = _mapper.Map<List<MatrixColumn>>(updateQuestion.MatrixConfig.Columns);
                }
                if (updateQuestion.SliderConfig is not null)
                {
                    newQuestion.SliderConfig = _mapper.Map<SliderConfig>(updateQuestion.SliderConfig);
                }
                SetAuditFieldsForNewEntity(newQuestion, userId);
                SetAuditFieldsForNewQuestionChildren(newQuestion, userId, organizationId);
                existingPage.Questions.Add(newQuestion);
            }
        }
    }

    private void UpdateOptionsAsync(Question existingQuestion, List<UpdateOptionDto> updateOptions, int userId, int organizationId)
    {
        var existingOptions = existingQuestion.Options.ToList();
        var updatedOptionIds = updateOptions.Where(o => o.Id.HasValue).Select(o => o.Id!.Value).ToList();

        // Remove options that are no longer in the update request
        var optionsToRemove = existingOptions.Where(o => !updatedOptionIds.Contains(o.Id)).ToList();
        foreach (var optionToRemove in optionsToRemove)
        {
            existingQuestion.Options.Remove(optionToRemove);
        }

        // Process each option in the update request
        foreach (var updateOption in updateOptions)
        {
            if (updateOption.Id.HasValue)
            {
                // Update existing option
                var existingOption = existingOptions.FirstOrDefault(o => o.Id == updateOption.Id.Value);
                if (existingOption is not null)
                {
                    //existingOption.OptionText = updateOption.OptionText;
                    //existingOption.ImageUrl = updateOption.ImageUrl;
                    //existingOption.DisplayOrder = updateOption.DisplayOrder;
                    //existingOption.Score = updateOption.Score;

                    _mapper.Map(updateOption, existingOption);

                    existingOption.UpdatedBy = userId;
                    existingOption.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                // Add new option
                var newOption = _mapper.Map<Option>(updateOption);
                newOption.QuestionId = existingQuestion.Id;
                newOption.OrganizationId = organizationId;
                SetAuditFieldsForNewEntity(newOption, userId);
                existingQuestion.Options.Add(newOption);
            }
        }
    }

    private void UpdateMatrixConfigAsync(Question existingQuestion, UpdateMatrixConfigDto updateMatrixConfig, int userId, int organizationId)
    {
        // Handle matrix rows
        var existingRows = existingQuestion.MatrixRows.ToList();
        var updatedRowIds = updateMatrixConfig.Rows.Where(r => r.Id.HasValue).Select(r => r.Id!.Value).ToList();

        var rowsToRemove = existingRows.Where(r => !updatedRowIds.Contains(r.Id)).ToList();
        foreach (var rowToRemove in rowsToRemove)
        {
            existingQuestion.MatrixRows.Remove(rowToRemove);
        }

        foreach (var updateRow in updateMatrixConfig.Rows)
        {
            if (updateRow.Id.HasValue)
            {
                var existingRow = existingRows.FirstOrDefault(r => r.Id == updateRow.Id.Value);
                if (existingRow is not null)
                {
                    existingRow.RowLabel = updateRow.RowLabel;
                    existingRow.DisplayOrder = updateRow.DisplayOrder;
                    existingRow.UpdatedBy = userId;
                    existingRow.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                var newRow = _mapper.Map<MatrixRow>(updateRow);
                newRow.QuestionId = existingQuestion.Id;
                newRow.OrganizationId = organizationId;
                SetAuditFieldsForNewEntity(newRow, userId);
                existingQuestion.MatrixRows.Add(newRow);
            }
        }

        // Handle matrix columns
        var existingColumns = existingQuestion.MatrixColumns.ToList();
        var updatedColumnIds = updateMatrixConfig.Columns.Where(c => c.Id.HasValue).Select(c => c.Id!.Value).ToList();

        var columnsToRemove = existingColumns.Where(c => !updatedColumnIds.Contains(c.Id)).ToList();
        foreach (var columnToRemove in columnsToRemove)
        {
            existingQuestion.MatrixColumns.Remove(columnToRemove);
        }

        foreach (var updateColumn in updateMatrixConfig.Columns)
        {
            if (updateColumn.Id.HasValue)
            {
                var existingColumn = existingColumns.FirstOrDefault(c => c.Id == updateColumn.Id.Value);
                if (existingColumn is not null)
                {
                    existingColumn.ColumnLabel = updateColumn.ColumnLabel;
                    existingColumn.DisplayOrder = updateColumn.DisplayOrder;
                    existingColumn.Score = updateColumn.Score;
                    existingColumn.UpdatedBy = userId;
                    existingColumn.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                var newColumn = _mapper.Map<MatrixColumn>(updateColumn);
                newColumn.QuestionId = existingQuestion.Id;
                newColumn.OrganizationId = organizationId;
                SetAuditFieldsForNewEntity(newColumn, userId);
                existingQuestion.MatrixColumns.Add(newColumn);
            }
        }
    }

    private void UpdateSliderConfigAsync(Question existingQuestion, UpdateSliderConfigDto updateSliderConfig, int userId, int organizationId)
    {
        if (updateSliderConfig.Id.HasValue)
        {
            // Update existing slider config
            //existingQuestion.SliderConfig.MinValue = updateSliderConfig.MinValue;
            //existingQuestion.SliderConfig.MaxValue = updateSliderConfig.MaxValue;
            //existingQuestion.SliderConfig.StepValue = updateSliderConfig.StepValue;
            //existingQuestion.SliderConfig.MinLabel = updateSliderConfig.MinLabel;
            //existingQuestion.SliderConfig.MaxLabel = updateSliderConfig.MaxLabel;

            if (existingQuestion.SliderConfig is not null)
            {
                _mapper.Map(updateSliderConfig, existingQuestion.SliderConfig);

                existingQuestion.SliderConfig.UpdatedBy = userId;
                existingQuestion.SliderConfig.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var newSliderConfig = _mapper.Map<SliderConfig>(updateSliderConfig);
                newSliderConfig.QuestionId = existingQuestion.Id;
                newSliderConfig.OrganizationId = organizationId;
                SetAuditFieldsForNewEntity(newSliderConfig, userId);
                existingQuestion.SliderConfig = newSliderConfig;
            }
        }
        else
        {
            // Create new slider config
            var newSliderConfig = _mapper.Map<SliderConfig>(updateSliderConfig);
            newSliderConfig.QuestionId = existingQuestion.Id;
            newSliderConfig.OrganizationId = organizationId;
            SetAuditFieldsForNewEntity(newSliderConfig, userId);
            existingQuestion.SliderConfig = newSliderConfig;
        }
    }

    private static void SetAuditFieldsForNewEntity<T>(T entity, int userId) where T : class
    {
        var entityType = typeof(T);
        var createdByProperty = entityType.GetProperty("CreatedBy");
        var createdAtProperty = entityType.GetProperty("CreatedAt");
        var updatedByProperty = entityType.GetProperty("UpdatedBy");
        var updatedAtProperty = entityType.GetProperty("UpdatedAt");

        createdByProperty?.SetValue(entity, userId);
        createdAtProperty?.SetValue(entity, DateTime.UtcNow);
        updatedByProperty?.SetValue(entity, userId);
        updatedAtProperty?.SetValue(entity, DateTime.UtcNow);
    }

    private static void SetAuditFieldsForNewPageChildren(Page page, int userId, int organizationId)
    {
        foreach (var question in page.Questions)
        {
            question.OrganizationId = organizationId;
            SetAuditFieldsForNewEntity(question, userId);
            SetAuditFieldsForNewQuestionChildren(question, userId, organizationId);
        }
    }

    private static void SetAuditFieldsForNewQuestionChildren(Question question, int userId, int organizationId)
    {
        foreach (var option in question.Options)
        {
            option.OrganizationId = organizationId;
            SetAuditFieldsForNewEntity(option, userId);
        }

        foreach (var matrixRow in question.MatrixRows)
        {
            matrixRow.OrganizationId = organizationId;
            SetAuditFieldsForNewEntity(matrixRow, userId);
        }

        foreach (var matrixColumn in question.MatrixColumns)
        {
            matrixColumn.OrganizationId = organizationId;
            SetAuditFieldsForNewEntity(matrixColumn, userId);
        }

        if (question.SliderConfig is not null)
        {
            question.SliderConfig.OrganizationId = organizationId;
            SetAuditFieldsForNewEntity(question.SliderConfig, userId);
        }
    }

    public async Task<JsonModel> GetFormByIdAsync(int formId)
    {
        _logger.LogInformation("Getting form by ID: {FormId}", formId);

        // Get organization ID from current user claims
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel(responseData: null!, "User must belong to an organization", StatusCodes.Status401Unauthorized);
        }

        var form = await _formRepository.GetByIdWithDetailsAsync(formId, organizationId.Value);
        if (form is null)
        {
            return new JsonModel(responseData: null!, StatusMessage.FormNotFound, StatusCodes.Status404NotFound);
        }

        var response = _mapper.Map<FormResponseDto>(form);
        return new JsonModel(response, StatusMessage.FormRetrievedSuccessfully, StatusCodes.Status200OK);
    }

    public async Task<JsonModel> GetAllFormsAsync()
    {
        _logger.LogInformation("Getting all forms");

        // Get organization ID from current user claims
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel(responseData: null!, "User must belong to an organization", StatusCodes.Status401Unauthorized);
        }

        var forms = await _formRepository.GetAllAsync(organizationId.Value);
        var response = _mapper.Map<List<FormResponseDto>>(forms);

        return new JsonModel(response, StatusMessage.FormRetrievedSuccessfully, StatusCodes.Status200OK);
    }

    public async Task<JsonModel> GetPagedFormsAsync(FormFilterDto filter)
    {
        _logger.LogInformation("Getting paged forms with stored procedure - Page: {Page}, PageSize: {PageSize}, SearchTerm: {SearchTerm}, SortColumn: {SortColumn}, SortOrder: {SortOrder}",
            filter.PageNumber, filter.PageSize, filter.SearchTerm, filter.SortColumn, filter.SortOrder);

        // Get organization ID from current user claims
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel(responseData: null!, "User must belong to an organization", StatusCodes.Status401Unauthorized);
        }

        // Call the repository method that uses stored procedure with organization filtering
        var pagedResult = await _formRepository.GetPagedFormsAsync(filter, organizationId.Value);

        // Include published/draft counts in the response data for UI cards
        var responseData = new
        {
            Items = pagedResult.Items,
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            PublishedCount = pagedResult.PublishedCount ?? 0,
            DraftCount = pagedResult.DraftCount ?? 0
        };

        return new JsonModel(responseData, StatusMessage.FormRetrievedSuccessfullyUsingStoredProcedure, StatusCodes.Status200OK);
    }

    public async Task<JsonModel> PublishFormAsync(int formId, int userId)
    {
        _logger.LogInformation("Publishing form with ID: {FormId}", formId);

        // Get organization ID from current user claims
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel(responseData: null!, "User must belong to an organization", StatusCodes.Status401Unauthorized);
        }

        var form = await _formRepository.GetByIdAsync(formId);
        if (form is null || form.OrganizationId != organizationId.Value)
        {
            return new JsonModel(responseData: null!, StatusMessage.FormNotFound, StatusCodes.Status404NotFound);
        }

        if (form.IsPublished)
        {
            return new JsonModel(responseData: null!, StatusMessage.FormIsAlreadyPublished, StatusCodes.Status400BadRequest);
        }

        // Generate unique public key
        var publicKey = GeneratePublicKey();

        // Ensure the public key is unique
        int attempts = Number.Zero;
        while (attempts < Number.Ten) // Prevent infinite loop
        {
            var exists = await _formRepository.PublicKeyExistsAsync(publicKey);

            if (!exists)
                break;

            publicKey = GeneratePublicKey();
            attempts++;
        }

        // Update form
        form.IsPublished = true;
        form.PublishedDate = DateTime.UtcNow;
        form.PublicKey = publicKey;
        form.PublicURL = $"/public/form/{publicKey}";
        form.UpdatedBy = userId;
        form.UpdatedAt = DateTime.UtcNow;

        await _formRepository.UpdateAsync(form);

        var response = new { PublicKey = publicKey, PublicURL = form.PublicURL };
        return new JsonModel(response, StatusMessage.FormPublishedSuccessfully, StatusCodes.Status200OK);
    }

    public async Task<JsonModel> GetPublicFormAsync(string publicKey)
    {
        _logger.LogInformation("Getting public form with key: {PublicKey}", publicKey);

        var form = await _formRepository.GetByPublicKeyWithDetailsAsync(publicKey);
        if (form is null)
        {
            return new JsonModel(responseData: null!, "Form not found or not published", StatusCodes.Status404NotFound);
        }

        var response = _mapper.Map<PublicFormResponseDto>(form);
        return new JsonModel(response, StatusMessage.FormRetrievedSuccessfully, StatusCodes.Status200OK);
    }

    private static string GeneratePublicKey()
    {
        return Guid.NewGuid().ToString("N")[..16].ToUpperInvariant();
    }

    public async Task<JsonModel> GetMasterQuestionTypesAsync()
    {
        _logger.LogInformation("Getting master question types");

        var questionTypes = await _masterQuestionTypeRepository.GetAllAsync();
        var response = _mapper.Map<List<MasterQuestionTypeDto>>(questionTypes);

        return new JsonModel(response, StatusMessage.QuestionTypesRetrievedSuccessfully, StatusCodes.Status200OK);
    }

    public async Task<JsonModel> DeleteFormAsync(int formId, int userId)
    {
        _logger.LogInformation("Deleting form: {FormId}", formId);

        // Get organization ID from current user claims
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel
            {
                Data = null!,
                Message = "User must belong to an organization",
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        var form = await _formRepository.GetByIdAsync(formId);
        if (form is null || form.OrganizationId != organizationId.Value)
        {
            return new JsonModel
            {
                Data = null!,
                Message = StatusMessage.FormNotFound,
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        // Optionally record who deleted the form
        form.UpdatedBy = userId;
        form.UpdatedAt = DateTime.UtcNow;
        form.DeletedBy = userId;

        var success = await _formRepository.DeleteAsync(formId, organizationId.Value);
        if (!success)
        {
            return new JsonModel
            {
                Data = null!,
                Message = StatusMessage.FormNotFound,
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        _logger.LogInformation("Successfully deleted form with ID: {FormId}", formId);

        return new JsonModel
        {
            Data = null!,
            Message = StatusMessage.FormDeletedSuccessfully,
            StatusCode = StatusCodes.Status200OK
        };
    }

    public async Task<JsonModel> SubmitFormResponseAsync(string publicKey, SubmitFormRequest request)
    {
        _logger.LogInformation("Submitting form response for publicKey: {PublicKey}, email: {Email}",
            publicKey, request.RespondentEmail);

        // Get the published form
        var form = await _formRepository.GetByPublicKeyWithDetailsAsync(publicKey);
        if (form is null || !form.IsPublished)
        {
            return new JsonModel(responseData: null!, StatusMessage.FormNotFound_or_NotPublished, StatusCodes.Status404NotFound);
        }

        // Check if resubmission is allowed
        if (!form.AllowResubmission)
        {
            var hasExistingSubmission = await _submissionRepository.HasPreviousSubmissionAsync(form.Id, request.RespondentEmail, form.OrganizationId);
            if (hasExistingSubmission)
            {
                return new JsonModel(responseData: null!, StatusMessage.MultipleSubmissionsAreNotAllowedForThisForm, StatusCodes.Status400BadRequest);
            }
        }

        // Validate required questions are answered
        var validationResult = ValidateRequiredQuestions(form, request);
        if (!validationResult.IsValid)
        {
            return new JsonModel(responseData: null!, validationResult.Message, StatusCodes.Status400BadRequest);
        }

        // Create submission
        var submission = new Submission
        {
            FormId = form.Id,
            RespondentEmail = request.RespondentEmail.Trim(),
            RespondentName = request.RespondentName?.Trim(),
            SubmittedDate = DateTime.UtcNow,
            CreatedBy = Number.Zero, // Anonymous submission
            CreatedAt = DateTime.UtcNow,
            OrganizationId = form.OrganizationId, // Set organization ID from the form
            TotalScore = Number.Zero // Will be calculated later if needed
        };

        var createdSubmission = await _submissionRepository.CreateAsync(submission);

        // Build all answers and values in-memory then create in bulk to avoid N round-trips
        var now = DateTime.UtcNow;
        var answersToCreate = new List<Answer>(request.Answers.Count);
        foreach (var answerDto in request.Answers)
        {
            var question = form.Pages.SelectMany(p => p.Questions).FirstOrDefault(q => q.Id == answerDto.QuestionId);
            if (question is null)
            {
                _logger.LogWarning("Question {QuestionId} not found in form {FormId}", answerDto.QuestionId, form.Id);
                continue;
            }

            answersToCreate.Add(new Answer
            {
                SubmissionId = createdSubmission.Id,
                QuestionId = answerDto.QuestionId,
                CreatedBy = Number.Zero,
                CreatedAt = now,
                OrganizationId = form.OrganizationId,
                Score = Number.Zero
            });
        }

        if (answersToCreate.Count > 0)
        {
            await _answerRepository.CreateRangeAsync(answersToCreate);

            // After bulk insert, ids are populated; now create all answer values in bulk
            var answerValuesToCreate = new List<AnswerValue>();
            foreach (var (answerDto, idx) in request.Answers.Select((a, i) => (a, i)))
            {
                if (idx >= answersToCreate.Count) break; // safety
                var createdAnswer = answersToCreate[idx];
                foreach (var valueDto in answerDto.Values)
                {
                    answerValuesToCreate.Add(new AnswerValue
                    {
                        AnswerId = createdAnswer.Id,
                        SelectedOptionId = valueDto.SelectedOptionId,
                        MatrixRowId = valueDto.MatrixRowId,
                        SelectedMatrixColumnId = valueDto.SelectedMatrixColumnId,
                        TextValue = valueDto.TextValue?.Trim(),
                        NumericValue = valueDto.NumericValue,
                        CreatedBy = Number.Zero,
                        CreatedAt = now,
                        OrganizationId = form.OrganizationId
                    });
                }
            }

            if (answerValuesToCreate.Count > 0)
            {
                await _answerValueRepository.CreateRangeAsync(answerValuesToCreate);
            }
        }

        // Attempt to calculate score immediately after submission
        decimal calculatedTotal = Number.Zero;
        try
        {
            var scoreResult = await _scoringEngineService.CalculateSubmissionScoreAsync(createdSubmission.Id);
            if (scoreResult?.Data is not null)
            {
                // scoreResult.Data contains { SubmissionId, TotalScore }
                var data = scoreResult.Data as dynamic;
                calculatedTotal = (decimal)(data?.TotalScore ?? Number.Zero);
            }
        }
        catch (Exception ex)
        {
            // Log and continue - submission succeeded but scoring failed
            _logger.LogError(ex, "Failed to calculate score for submission {SubmissionId}", createdSubmission.Id);
        }

        var response = new SubmissionResponseDto
        {
            SubmissionId = createdSubmission.Id,
            RespondentEmail = createdSubmission.RespondentEmail,
            RespondentName = createdSubmission.RespondentName,
            SubmittedDate = createdSubmission.SubmittedDate,
            TotalScore = calculatedTotal,
            Message = StatusMessage.FormSubmittedSuccessfully
        };

        return new JsonModel(response, StatusMessage.FormSubmittedSuccessfully, StatusCodes.Status200OK);
    }

    private static (bool IsValid, string Message) ValidateRequiredQuestions(Form form, SubmitFormRequest request)
    {
        var allQuestions = form.Pages.SelectMany(p => p.Questions).ToList();
        var requiredQuestions = allQuestions.Where(q => q.IsRequired).ToList();
        var answeredQuestionIds = request.Answers.Select(a => a.QuestionId).ToHashSet();

        var missingRequiredQuestions = requiredQuestions
            .Where(q => !answeredQuestionIds.Contains(q.Id))
            .ToList();

        if (missingRequiredQuestions.Any())
        {
            var missingQuestionTexts = string.Join(", ", missingRequiredQuestions.Select(q => q.QuestionText));
            return (false, $"The following required questions must be answered: {missingQuestionTexts}");
        }

        // Validate that required questions have actual values
        foreach (var requiredQuestion in requiredQuestions)
        {
            var answer = request.Answers.FirstOrDefault(a => a.QuestionId == requiredQuestion.Id);
            if (answer is null || !answer.Values.Any())
            {
                return (false, $"Question '{requiredQuestion.QuestionText}' is required and must have an answer");
            }

            // Check if any answer value has actual content
            bool hasValidAnswer = answer.Values.Any(v =>
                v.SelectedOptionId.HasValue ||
                v.MatrixRowId.HasValue ||
                v.SelectedMatrixColumnId.HasValue ||
                !string.IsNullOrWhiteSpace(v.TextValue) ||
                v.NumericValue.HasValue);

            if (!hasValidAnswer)
            {
                return (false, $"Question '{requiredQuestion.QuestionText}' is required and must have a valid answer");
            }
        }

        return (true, string.Empty);
    }

    // Phase 5: Admin Submission Review APIs
    public async Task<JsonModel> GetFormSubmissionsAsync(int formId, ResponseFilterDto filter)
    {
        _logger.LogInformation("Getting submissions for form: {FormId}", formId);

        // Get organization ID from current user claims
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel
            {
                Data = null!,
                Message = "User must belong to an organization",
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        // Check if form exists and belongs to the user's organization
        var form = await _formRepository.GetByIdAsync(formId);
        if (form is null || form.OrganizationId != organizationId.Value)
        {
            return new JsonModel
            {
                Data = null!,
                Message = StatusMessage.FormNotFound,
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        // Get paginated responses with organization filtering using stored procedure
        var result = await _submissionRepository.GetPagedResponsesAsync(formId, filter, organizationId.Value);

        return new JsonModel
        {
            Data = result,
            Message = StatusMessage.FormResponsesRetrievedSuccessfully,
            StatusCode = StatusCodes.Status200OK
        };

    }

    public async Task<JsonModel> GetSubmissionByIdAsync(int submissionId)
    {
        _logger.LogInformation("Getting submission details for: {SubmissionId}", submissionId);

        // Get organization ID from current user claims
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel
            {
                Data = null!,
                Message = "User must belong to an organization",
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        // Get submission with full details and organization filtering
        var submission = await _submissionRepository.GetByIdWithDetailsAsync(submissionId, organizationId.Value);
        if (submission is null)
        {
            return new JsonModel
            {
                Data = null!,
                Message = StatusMessage.ResponseNotFound,
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        // Map to response detail DTO
        var responseDetail = new ResponseDetailDto
        {
            SubmissionId = submission.Id,
            FormId = submission.FormId,
            FormTitle = submission.Form.Title,
            RespondentEmail = submission.RespondentEmail,
            RespondentName = submission.RespondentName,
            SubmittedDate = submission.SubmittedDate,
            TotalScore = submission.TotalScore,
            Answers = submission.Answers?.Where(answer => answer.Question is not null).Select(answer => new ResponseAnswerDto
            {
                QuestionId = answer.QuestionId,
                QuestionText = answer.Question.QuestionText ?? string.Empty,
                QuestionType = answer.Question.QuestionType?.TypeName ?? "Unknown",
                IsRequired = answer.Question.IsRequired,
                Score = answer.Score,
                Values = answer.AnswerValues?.Select(av => new ResponseAnswerValueDto
                {
                    SelectedOptionId = av.SelectedOptionId,
                    SelectedOptionText = av.SelectedOption?.OptionText,
                    MatrixRowId = av.MatrixRowId,
                    MatrixRowLabel = av.MatrixRowId.HasValue ?
                        answer.Question.MatrixRows?.FirstOrDefault(mr => mr.Id == av.MatrixRowId)?.RowLabel : null,
                    SelectedMatrixColumnId = av.SelectedMatrixColumnId,
                    SelectedMatrixColumnLabel = av.SelectedMatrixColumnId.HasValue ?
                        answer.Question.MatrixColumns?.FirstOrDefault(mc => mc.Id == av.SelectedMatrixColumnId)?.ColumnLabel : null,
                    TextValue = av.TextValue,
                    NumericValue = av.NumericValue
                }).ToList() ?? new List<ResponseAnswerValueDto>()
            }).ToList() ?? new List<ResponseAnswerDto>()
        };

        return new JsonModel
        {
            Data = responseDetail,
            Message = StatusMessage.ResponseDetailsRetrievedSuccessfully,
            StatusCode = StatusCodes.Status200OK
        };
    }

    #region Phase 6: Conditional Logic Rules

    public async Task<JsonModel> CreateRuleAsync(CreateRuleRequest request, int userId)
    {
        // Get organization ID from current user claims
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            throw new UnauthorizedAccessException("User must belong to an organization to create rules");
        }

        _logger.LogInformation("Creating rule for form: {FormId}", request.FormId);

        // Validate that the form exists
        var form = await _formRepository.GetByIdAsync(request.FormId);
        if (form is null)
        {
            return new JsonModel
            {
                Data = null!,
                Message = StatusMessage.FormNotFound,
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        // Check for duplicate rules
        var ruleExists = await _ruleRepository.RuleExistsAsync(
            request.FormId,
            request.SourceQuestionId,
            request.TriggerOptionId,
            request.Condition,
            organizationId.Value);

        if (ruleExists)
        {
            return new JsonModel
            {
                Data = null!,
                Message = StatusMessage.A_RuleWithTheSameConditionAlreadyExistsForThisQuestion,
                StatusCode = StatusCodes.Status409Conflict
            };
        }

        var rule = _mapper.Map<Rule>(request);
        rule.CreatedBy = userId;
        rule.OrganizationId = organizationId.Value;

        var createdRule = await _ruleRepository.CreateAsync(rule);
        var ruleWithDetails = await _ruleRepository.GetByIdWithDetailsAsync(createdRule.Id, organizationId.Value);
        var response = _mapper.Map<RuleResponseDto>(ruleWithDetails);

        _logger.LogInformation("Successfully created rule with ID: {RuleId}", createdRule.Id);

        return new JsonModel
        {
            Data = response,
            Message = StatusMessage.RuleCreatedSuccessfully,
            StatusCode = StatusCodes.Status201Created
        };
    }

    public async Task<JsonModel> UpdateRuleAsync(UpdateRuleRequest request, int userId)
    {
        _logger.LogInformation("Updating rule: {RuleId}", request.RuleId);
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel
            {
                Data = null!,
                Message = "User must belong to an organization",
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        var existingRule = await _ruleRepository.GetByIdWithDetailsAsync(request.RuleId, organizationId.Value);
        if (existingRule is null)
        {
            return new JsonModel
            {
                Data = null!,
                Message = StatusMessage.RuleNotFound,
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        // Map update request to existing rule
        _mapper.Map(request, existingRule);
        existingRule.UpdatedBy = userId;

        var updatedRule = await _ruleRepository.UpdateAsync(existingRule);
        var ruleWithDetails = await _ruleRepository.GetByIdWithDetailsAsync(updatedRule.Id, organizationId.Value);
        var response = _mapper.Map<RuleResponseDto>(ruleWithDetails);

        _logger.LogInformation("Successfully updated rule with ID: {RuleId}", request.RuleId);

        return new JsonModel
        {
            Data = response,
            Message = StatusMessage.RulesUpdatedSuccessfully,
            StatusCode = StatusCodes.Status200OK
        };
    }

    public async Task<JsonModel> DeleteRuleAsync(int ruleId, int userId)
    {
        _logger.LogInformation("Deleting rule: {RuleId}", ruleId);

        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel
            {
                Data = null!,
                Message = "User must belong to an organization",
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        var success = await _ruleRepository.DeleteAsync(ruleId, organizationId.Value);
        if (!success)
        {
            return new JsonModel
            {
                Data = null!,
                Message = StatusMessage.RuleNotFound,
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        _logger.LogInformation("Successfully deleted rule with ID: {RuleId}", ruleId);

        return new JsonModel
        {
            Data = null!,
            Message = StatusMessage.RulesDeletedSuccessfully,
            StatusCode = StatusCodes.Status200OK
        };
    }

    public async Task<JsonModel> GetFormRulesAsync(int formId)
    {
        _logger.LogInformation("Getting rules for form: {FormId}", formId);
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel
            {
                Data = null!,
                Message = "User must belong to an organization",
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        var form = await _formRepository.GetByIdAsync(formId);
        if (form is null || form.OrganizationId != organizationId.Value)
        {
            return new JsonModel
            {
                Data = null!,
                Message = StatusMessage.FormNotFound,
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        var rules = await _ruleRepository.GetByFormIdWithDetailsAsync(formId, organizationId.Value);
        var response = _mapper.Map<List<RuleResponseDto>>(rules);

        return new JsonModel
        {
            Data = response,
            Message = StatusMessage.RulesRetrievedSuccessfully,
            StatusCode = StatusCodes.Status200OK
        };

    }

    public async Task<JsonModel> GetRuleByIdAsync(int ruleId)
    {
        _logger.LogInformation("Getting rule by ID: {RuleId}", ruleId);

        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel
            {
                Data = null!,
                Message = "User must belong to an organization",
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        var rule = await _ruleRepository.GetByIdWithDetailsAsync(ruleId, organizationId.Value);
        if (rule is null)
        {
            return new JsonModel
            {
                Data = null!,
                Message = StatusMessage.RuleNotFound,
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        var response = _mapper.Map<RuleResponseDto>(rule);

        return new JsonModel
        {
            Data = response,
            Message = StatusMessage.RulesRetrievedSuccessfully,
            StatusCode = StatusCodes.Status200OK
        };
    }

    public async Task<JsonModel> EvaluateRulesAsync(RuleEvaluationRequest request)
    {
        _logger.LogInformation("Evaluating rules for form: {FormId}", request.FormId);
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel
            {
                Data = new RuleEvaluationResult(),
                Message = "User must belong to an organization",
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        var rules = await _ruleRepository.GetByFormIdWithDetailsAsync(request.FormId, organizationId.Value);
        if (!rules.Any())
        {
            return new JsonModel
            {
                Data = new RuleEvaluationResult(),
                Message = StatusMessage.NoRulesFoundForEvaluation,
                StatusCode = StatusCodes.Status200OK
            };
        }

        var result = new RuleEvaluationResult();

        foreach (var rule in rules)
        {
            var isTriggered = await EvaluateRuleAsync(rule, request.Answers, organizationId.Value);

            // Record triggered rule ids for true evaluations
            if (isTriggered)
            {
                result.TriggeredRuleIds.Add(rule.Id);
            }

            // Always apply the rule action; the action may be inverted when the condition is false
            ApplyRuleAction(rule, result, isTriggered);
        }

        return new JsonModel
        {
            Data = result,
            Message = StatusMessage.RulesEvaluatedSuccessfully,
            StatusCode = StatusCodes.Status200OK
        };
    }

    private async Task<bool> EvaluateRuleAsync(Rule rule, List<RuleEvaluationAnswer> answers, int organizationId)
    {
        var answer = answers.FirstOrDefault(a => a.QuestionId == rule.SourceQuestionId);
        if (answer is null)
            return false;

        return rule.Condition switch
        {
            RuleConstants.IsSelected => rule.TriggerOptionId.HasValue && answer.SelectedOptionIds.Contains(rule.TriggerOptionId.Value),
            RuleConstants.IsNotSelected => rule.TriggerOptionId.HasValue && !answer.SelectedOptionIds.Contains(rule.TriggerOptionId.Value),
            RuleConstants.IsGreaterThan => answer.NumericValue.HasValue && rule.MinValue.HasValue && answer.NumericValue > rule.MinValue,
            RuleConstants.IsLessThan => answer.NumericValue.HasValue && rule.MinValue.HasValue && answer.NumericValue < rule.MinValue,
            RuleConstants.IsEqualTo => answer.NumericValue.HasValue && rule.MinValue.HasValue && answer.NumericValue == rule.MinValue,
            RuleConstants.IsNotEqualTo => answer.NumericValue.HasValue && rule.MinValue.HasValue && answer.NumericValue != rule.MinValue,
            RuleConstants.IsInRange => answer.NumericValue.HasValue && rule.MinValue.HasValue && rule.MaxValue.HasValue &&
                          answer.NumericValue >= rule.MinValue && answer.NumericValue <= rule.MaxValue,

            // Matrix-specific conditions
            RuleConstants.RowHasSelection => rule.MatrixRowId.HasValue &&
                                answer.MatrixAnswers.Any(ma => ma.MatrixRowId == rule.MatrixRowId.Value),
            RuleConstants.RowHasColumn => rule.MatrixRowId.HasValue && rule.MatrixColumnId.HasValue &&
                             answer.MatrixAnswers.Any(ma => ma.MatrixRowId == rule.MatrixRowId.Value &&
                                                           ma.SelectedColumnId == rule.MatrixColumnId.Value),
            RuleConstants.ColumnSelected => rule.MatrixColumnId.HasValue &&
                               answer.MatrixAnswers.Any(ma => ma.SelectedColumnId == rule.MatrixColumnId.Value),
            RuleConstants.ScoreGreaterThan => rule.ScoreValue.HasValue &&
                                await EvaluateMatrixScoreConditionAsync(answer.MatrixAnswers, rule, (score, ruleScore) => score > ruleScore, organizationId),
            RuleConstants.ScoreLessThan => rule.ScoreValue.HasValue &&
                                await EvaluateMatrixScoreConditionAsync(answer.MatrixAnswers, rule, (score, ruleScore) => score < ruleScore, organizationId),
            RuleConstants.ScoreEqualTo => rule.ScoreValue.HasValue &&
                                await EvaluateMatrixScoreConditionAsync(answer.MatrixAnswers, rule, (score, ruleScore) => score == ruleScore, organizationId),
            RuleConstants.ScoreInRange => rule.MinValue.HasValue && rule.MaxValue.HasValue &&
                                await EvaluateMatrixScoreRangeConditionAsync(answer.MatrixAnswers, rule, organizationId),

            _ => false
        };
    }

    private async Task<bool> EvaluateMatrixScoreConditionAsync(List<MatrixAnswer> matrixAnswers, Rule rule, Func<decimal, decimal, bool> comparison, int organizationId)
    {
        if (!rule.ScoreValue.HasValue || !matrixAnswers.Any())
            return false;

        try
        {
            // Get the source question with its matrix columns
            var sourceQuestion = await _questionRepository.GetByIdWithMatrixColumnsAsync(rule.SourceQuestionId, organizationId);
            if (sourceQuestion?.MatrixColumns == null || !sourceQuestion.MatrixColumns.Any())
                return false;

            // Get the column IDs from the matrix answers
            var columnIds = matrixAnswers.Select(ma => ma.SelectedColumnId).ToList();

            // Find the matrix columns that match the selected column IDs
            var selectedColumns = sourceQuestion.MatrixColumns
                .Where(mc => columnIds.Contains(mc.Id))
                .ToList();

            if (!selectedColumns.Any())
                return false;

            // Calculate total score for all selected columns
            var totalScore = selectedColumns.Sum(mc => mc.Score);

            // Apply the comparison function
            return comparison(totalScore, rule.ScoreValue.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating matrix score condition for rule {RuleId}", rule.Id);
            return false;
        }
    }

    private async Task<bool> EvaluateMatrixScoreRangeConditionAsync(List<MatrixAnswer> matrixAnswers, Rule rule, int organizationId)
    {
        if (!rule.MinValue.HasValue || !rule.MaxValue.HasValue || !matrixAnswers.Any())
            return false;

        try
        {
            // Get the source question with its matrix columns
            var sourceQuestion = await _questionRepository.GetByIdWithMatrixColumnsAsync(rule.SourceQuestionId, organizationId);
            if (sourceQuestion?.MatrixColumns == null || !sourceQuestion.MatrixColumns.Any())
                return false;

            // Get the column IDs from the matrix answers
            var columnIds = matrixAnswers.Select(ma => ma.SelectedColumnId).ToList();

            // Find the matrix columns that match the selected column IDs
            var selectedColumns = sourceQuestion.MatrixColumns
                .Where(mc => columnIds.Contains(mc.Id))
                .ToList();

            if (!selectedColumns.Any())
                return false;

            // Calculate total score for all selected columns
            var totalScore = selectedColumns.Sum(mc => mc.Score);

            // Check if the total score is within the specified range
            return totalScore >= rule.MinValue.Value && totalScore <= rule.MaxValue.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating matrix score range condition for rule {RuleId}", rule.Id);
            return false;
        }
    }

    private static void ApplyRuleAction(Rule rule, RuleEvaluationResult result, bool conditionMet)
    {
        // For Show/Hide actions we need to apply the inverse when the condition is false.
        switch (rule.ActionType)
        {
            case RuleConstants.HideQuestion:
                if (!rule.TargetQuestionId.HasValue) break;

                if (conditionMet)
                {
                    // Hide the question
                    if (!result.HiddenQuestionIds.Contains(rule.TargetQuestionId.Value))
                        result.HiddenQuestionIds.Add(rule.TargetQuestionId.Value);

                    // Ensure it's not also marked visible
                    if (result.VisibleQuestionIds.Contains(rule.TargetQuestionId.Value))
                        result.VisibleQuestionIds.Remove(rule.TargetQuestionId.Value);
                }
                else
                {
                    // Condition false -> inverse of hide is show
                    if (!result.VisibleQuestionIds.Contains(rule.TargetQuestionId.Value))
                        result.VisibleQuestionIds.Add(rule.TargetQuestionId.Value);

                    if (result.HiddenQuestionIds.Contains(rule.TargetQuestionId.Value))
                        result.HiddenQuestionIds.Remove(rule.TargetQuestionId.Value);
                }

                break;

            case RuleConstants.ShowQuestion:
                if (!rule.TargetQuestionId.HasValue) break;

                if (conditionMet)
                {
                    // Show the question
                    if (!result.VisibleQuestionIds.Contains(rule.TargetQuestionId.Value))
                        result.VisibleQuestionIds.Add(rule.TargetQuestionId.Value);

                    if (result.HiddenQuestionIds.Contains(rule.TargetQuestionId.Value))
                        result.HiddenQuestionIds.Remove(rule.TargetQuestionId.Value);
                }
                else
                {
                    // Condition false -> inverse of show is hide
                    if (!result.HiddenQuestionIds.Contains(rule.TargetQuestionId.Value))
                        result.HiddenQuestionIds.Add(rule.TargetQuestionId.Value);

                    if (result.VisibleQuestionIds.Contains(rule.TargetQuestionId.Value))
                        result.VisibleQuestionIds.Remove(rule.TargetQuestionId.Value);
                }

                break;

            case RuleConstants.SkipToPage:
                // Only set skip when condition is met
                if (conditionMet)
                    result.SkipToPageId = rule.TargetPageId;
                break;

            case RuleConstants.TerminateForm:
                // Only terminate when condition is met
                if (conditionMet)
                    result.TerminateForm = true;
                break;
        }
    }

    #endregion

    #region Test Mode Implementation

    public async Task<JsonModel> SubmitTestModeAsync(TestModeSubmissionRequest request)
    {
        _logger.LogInformation("Processing test mode submission for form: {FormId}", request.FormId);

        try
        {
            // Validate the form exists
            var formExists = await _formRepository.ExistsAsync(request.FormId);
            if (!formExists)
            {
                return new JsonModel
                {
                    Data = new object(),
                    Message = StatusMessage.FormNotFound,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            var result = new TestModeSubmissionResult
            {
                FormId = request.FormId,
                SubmittedAt = DateTime.UtcNow
            };

            // Evaluate rules if requested
            if (request.EvaluateRules)
            {
                var ruleEvaluationRequest = ConvertToRuleEvaluationRequest(request);
                var ruleResult = await EvaluateRulesAsync(ruleEvaluationRequest);
                if (ruleResult.StatusCode == StatusCodes.Status200OK && ruleResult.Data is RuleEvaluationResult ruleEvaluation)
                {
                    result.RuleEvaluation = ruleEvaluation;
                }
            }

            // Calculate scores if requested
            if (request.CalculateScore)
            {
                var scoreResult = await CalculateTestModeScoreInternalAsync(request);
                result.ScoreResult = scoreResult;
            }

            _logger.LogInformation("Test mode submission processed successfully for form: {FormId}", request.FormId);

            return new JsonModel
            {
                Data = result,
                Message = StatusMessage.TestModeSubmissionSuccess,
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing test mode submission for form: {FormId}", request.FormId);
            return new JsonModel
            {
                Data = new object(),
                Message = StatusMessage.TestModeSubmissionError,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

    public async Task<JsonModel> EvaluateTestModeRulesAsync(TestModeSubmissionRequest request)
    {
        _logger.LogInformation("Evaluating test mode rules for form: {FormId}", request.FormId);

        try
        {
            var ruleEvaluationRequest = ConvertToRuleEvaluationRequest(request);
            var result = await EvaluateRulesAsync(ruleEvaluationRequest);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating test mode rules for form: {FormId}", request.FormId);
            return new JsonModel
            {
                Data = new object(),
                Message = StatusMessage.TestModeRuleEvaluationError,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

    public async Task<JsonModel> CalculateTestModeScoreAsync(TestModeSubmissionRequest request)
    {
        _logger.LogInformation("Calculating test mode score for form: {FormId}", request.FormId);

        try
        {
            var scoreResult = await CalculateTestModeScoreInternalAsync(request);

            return new JsonModel
            {
                Data = scoreResult,
                Message = StatusMessage.TestModeScoreCalculationSuccess,
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating test mode score for form: {FormId}", request.FormId);
            return new JsonModel
            {
                Data = new object(),
                Message = StatusMessage.TestModeScoreCalculationError,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

    #region Test Mode Helper Methods

    private RuleEvaluationRequest ConvertToRuleEvaluationRequest(TestModeSubmissionRequest request)
    {
        return new RuleEvaluationRequest
        {
            FormId = request.FormId,
            Answers = request.Answers.Select(a => new RuleEvaluationAnswer
            {
                QuestionId = a.QuestionId,
                SelectedOptionIds = a.SelectedOptionIds,
                NumericValue = a.NumericValue,
                TextValue = a.TextValue,
                MatrixAnswers = a.MatrixAnswers.Select(ma => new MatrixAnswer
                {
                    MatrixRowId = ma.MatrixRowId,
                    SelectedColumnId = ma.SelectedColumnId
                }).ToList()
            }).ToList()
        };
    }

    private async Task<TestModeScoreResult> CalculateTestModeScoreInternalAsync(TestModeSubmissionRequest request)
    {
        // Get form with all details for scoring
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            throw new InvalidOperationException("User must belong to an organization to calculate test mode score");
        }

        var form = await _formRepository.GetByIdWithDetailsAsync(request.FormId, organizationId.Value);
        if (form == null)
        {
            throw new InvalidOperationException("Form not found for scoring calculation");
        }

        var scoreResult = new TestModeScoreResult();
        decimal totalScore = 0;
        decimal maxPossibleScore = 0;

        // Calculate score for each question
        foreach (var page in form.Pages)
        {
            foreach (var question in page.Questions)
            {
                var answer = request.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                var questionScore = CalculateQuestionScore(question, answer);
                var maxQuestionScore = CalculateMaxQuestionScore(question);

                totalScore += questionScore.Score;
                maxPossibleScore += maxQuestionScore;

                scoreResult.QuestionScores.Add(questionScore);
            }
        }

        scoreResult.TotalScore = totalScore;
        scoreResult.MaxPossibleScore = maxPossibleScore;
        scoreResult.ScorePercentage = maxPossibleScore > 0 ? (totalScore / maxPossibleScore) * 100 : 0;

        return scoreResult;
    }

    private TestModeQuestionScore CalculateQuestionScore(Question question, TestModeAnswer? answer)
    {
        var questionScore = new TestModeQuestionScore
        {
            QuestionId = question.Id,
            QuestionText = question.QuestionText,
            Score = 0,
            MaxScore = CalculateMaxQuestionScore(question)
        };

        if (answer == null)
        {
            return questionScore;
        }

        switch (question.QuestionType?.TypeName?.ToLower())
        {
            case "radio":
            case "dropdown":
                // Single selection questions
                if (answer.SelectedOptionIds.Any())
                {
                    var selectedOptionId = answer.SelectedOptionIds.First();
                    var option = question.Options.FirstOrDefault(o => o.Id == selectedOptionId);
                    if (option != null)
                    {
                        questionScore.Score = option.Score;
                        questionScore.AnswerScores.Add(new TestModeAnswerScore
                        {
                            OptionId = option.Id,
                            AnswerText = option.OptionText ?? string.Empty,
                            Score = option.Score
                        });
                    }
                }
                break;

            case "multi":
                // Multi-selection questions
                foreach (var selectedOptionId in answer.SelectedOptionIds)
                {
                    var option = question.Options.FirstOrDefault(o => o.Id == selectedOptionId);
                    if (option != null)
                    {
                        questionScore.Score += option.Score;
                        questionScore.AnswerScores.Add(new TestModeAnswerScore
                        {
                            OptionId = option.Id,
                            AnswerText = option.OptionText ?? string.Empty,
                            Score = option.Score
                        });
                    }
                }
                break;

            case "slider":
                // Slider questions - score might be based on the value or predefined scoring
                if (answer.NumericValue.HasValue)
                {
                    // For slider, we could implement custom scoring logic
                    // For now, using the numeric value as score if within range
                    var sliderValue = answer.NumericValue.Value;
                    questionScore.Score = sliderValue;
                    questionScore.AnswerScores.Add(new TestModeAnswerScore
                    {
                        AnswerText = sliderValue.ToString(),
                        Score = sliderValue
                    });
                }
                break;

            case "matrix":
                // Matrix questions
                foreach (var matrixAnswer in answer.MatrixAnswers)
                {
                    var column = question.MatrixColumns
                        .FirstOrDefault(c => c.Id == matrixAnswer.SelectedColumnId);
                    if (column != null)
                    {
                        questionScore.Score += column.Score;
                        questionScore.AnswerScores.Add(new TestModeAnswerScore
                        {
                            OptionId = column.Id,
                            AnswerText = column.ColumnLabel,
                            Score = column.Score
                        });
                    }
                }
                break;

            case "text":
            case "textarea":
            case "date":
                // Text-based questions typically don't have scores
                // But could be implemented based on business logic
                questionScore.AnswerScores.Add(new TestModeAnswerScore
                {
                    AnswerText = answer.TextValue ?? answer.DateValue?.ToString() ?? "",
                    Score = 0
                });
                break;
        }

        return questionScore;
    }

    private decimal CalculateMaxQuestionScore(Question question)
    {
        switch (question.QuestionType?.TypeName?.ToLower())
        {
            case "radio":
            case "dropdown":
                return question.Options.Any() ? question.Options.Max(o => o.Score) : 0;

            case "multi":
                return question.Options.Sum(o => o.Score);

            case "matrix":
                return question.MatrixColumns.Sum(c => c.Score);

            case "slider":
                // For slider, max score could be the max value or need specific configuration
                // This would need to be configured based on business requirements
                return question.SliderConfig?.MaxValue ?? 100;

            case "text":
            case "textarea":
            case "date":
            default:
                return 0;
        }
    }

    #endregion

    #endregion
}
