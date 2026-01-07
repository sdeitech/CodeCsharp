using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace App.Services.Test.DynamicQuestionnaire;

public class FormSubmissionTests : DynamicQuestionnaireTestBase
{
    private readonly IDynamicQuestionnaireService _service;

    public FormSubmissionTests()
    {
        _service = ServiceProvider.GetRequiredService<IDynamicQuestionnaireService>();
    }

    [Fact]
    public async Task SubmitFormResponseAsync_WithValidData_ShouldSubmitSuccessfully()
    {
        // Arrange
        var form = await CreatePublishedFormWithQuestions();
        var request = CreateValidSubmissionRequest(form);

        // Act
        var result = await _service.SubmitFormResponseAsync(form.PublicKey!, request);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be("Form submitted successfully");
        
        var submissionResponse = result.Data.Should().BeOfType<SubmissionResponseDto>().Subject;
        submissionResponse.RespondentEmail.Should().Be(request.RespondentEmail);
        submissionResponse.RespondentName.Should().Be(request.RespondentName);
        submissionResponse.SubmissionId.Should().BeGreaterThan(0);

        // Verify submission was saved to database
        var savedSubmission = await DbContext.Submissions
            .Include(s => s.Answers)
                .ThenInclude(a => a.AnswerValues)
            .FirstOrDefaultAsync(s => s.Id == submissionResponse.SubmissionId);
        
        savedSubmission.Should().NotBeNull();
        savedSubmission!.FormId.Should().Be(form.Id);
        savedSubmission.RespondentEmail.Should().Be(request.RespondentEmail);
        savedSubmission.Answers.Should().HaveCount(request.Answers.Count);
    }

    [Fact]
    public async Task SubmitFormResponseAsync_WithInvalidPublicKey_ShouldReturn404()
    {
        // Arrange
        var invalidPublicKey = "INVALID_KEY";
        var request = new SubmitFormRequest
        {
            RespondentEmail = "test@example.com",
            RespondentName = "Test User",
            Answers = new List<SubmissionAnswerDto>()
        };

        // Act
        var result = await _service.SubmitFormResponseAsync(invalidPublicKey, request);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("Form not found or not published");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task SubmitFormResponseAsync_WithUnpublishedForm_ShouldReturn404()
    {
        // Arrange
        var form = await CreateUnpublishedFormWithQuestions();
        var request = CreateValidSubmissionRequest(form);

        // Act
        var result = await _service.SubmitFormResponseAsync("SOME_KEY", request);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("Form not found or not published");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task SubmitFormResponseAsync_WithMissingRequiredQuestion_ShouldReturn400()
    {
        // Arrange
        var form = await CreatePublishedFormWithRequiredQuestions();
        var request = new SubmitFormRequest
        {
            RespondentEmail = "test@example.com",
            RespondentName = "Test User",
            Answers = new List<SubmissionAnswerDto>() // Missing required questions
        };

        // Act
        var result = await _service.SubmitFormResponseAsync(form.PublicKey!, request);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Contain("required questions must be answered");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task SubmitFormResponseAsync_WithEmptyRequiredAnswer_ShouldReturn400()
    {
        // Arrange
        var form = await CreatePublishedFormWithRequiredQuestions();
        var requiredQuestion = form.Pages.First().Questions.First(q => q.IsRequired);
        
        var request = new SubmitFormRequest
        {
            RespondentEmail = "test@example.com",
            RespondentName = "Test User",
            Answers = new List<SubmissionAnswerDto>
            {
                new()
                {
                    QuestionId = requiredQuestion.Id,
                    Values = new List<AnswerValueDto>() // Empty values for required question
                }
            }
        };

        // Act
        var result = await _service.SubmitFormResponseAsync(form.PublicKey!, request);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Contain("is required and must have an answer");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task SubmitFormResponseAsync_WithNoResubmissionAllowed_ShouldPreventDuplicateSubmission()
    {
        // Arrange
        var form = await CreatePublishedFormWithNoResubmission();
        var request = CreateValidSubmissionRequest(form);

        // Submit first time
        var firstResult = await _service.SubmitFormResponseAsync(form.PublicKey!, request);
        firstResult.StatusCode.Should().Be(200);

        // Act - Try to submit again with same email
        var secondResult = await _service.SubmitFormResponseAsync(form.PublicKey!, request);

        // Assert
        secondResult.Should().NotBeNull();
        secondResult.StatusCode.Should().Be(400);
        secondResult.Message.Should().Be("Multiple submissions are not allowed for this form");
        secondResult.Data.Should().BeNull();
    }

    [Fact]
    public async Task SubmitFormResponseAsync_WithResubmissionAllowed_ShouldAllowMultipleSubmissions()
    {
        // Arrange
        var form = await CreatePublishedFormWithResubmissionAllowed();
        var request = CreateValidSubmissionRequest(form);

        // Submit first time
        var firstResult = await _service.SubmitFormResponseAsync(form.PublicKey!, request);
        firstResult.StatusCode.Should().Be(200);

        // Act - Submit again with same email
        var secondResult = await _service.SubmitFormResponseAsync(form.PublicKey!, request);

        // Assert
        secondResult.Should().NotBeNull();
        secondResult.StatusCode.Should().Be(200);
        secondResult.Message.Should().Be("Form submitted successfully");
        secondResult.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task SubmitFormResponseAsync_WithRadioButtonAnswer_ShouldSaveCorrectly()
    {
        // Arrange
        var form = await CreateFormWithRadioQuestion();
        var radioQuestion = form.Pages.First().Questions.First();
        var selectedOption = radioQuestion.Options.First();
        
        var request = new SubmitFormRequest
        {
            RespondentEmail = "test@example.com",
            RespondentName = "Test User",
            Answers = new List<SubmissionAnswerDto>
            {
                new()
                {
                    QuestionId = radioQuestion.Id,
                    Values = new List<AnswerValueDto>
                    {
                        new() { SelectedOptionId = selectedOption.Id }
                    }
                }
            }
        };

        // Act
        var result = await _service.SubmitFormResponseAsync(form.PublicKey!, request);

        // Assert
        result.StatusCode.Should().Be(200);
        
        // Verify the answer was saved correctly
        var savedSubmission = await DbContext.Submissions
            .Include(s => s.Answers)
                .ThenInclude(a => a.AnswerValues)
            .FirstAsync();
        
        var savedAnswer = savedSubmission.Answers.First();
        var savedAnswerValue = savedAnswer.AnswerValues.First();
        savedAnswerValue.SelectedOptionId.Should().Be(selectedOption.Id);
    }

    [Fact]
    public async Task SubmitFormResponseAsync_WithMultiSelectAnswer_ShouldSaveAllSelections()
    {
        // Arrange
        var form = await CreateFormWithMultiSelectQuestion();
        var multiSelectQuestion = form.Pages.First().Questions.First();
        var selectedOptions = multiSelectQuestion.Options.Take(2).ToList();
        
        var request = new SubmitFormRequest
        {
            RespondentEmail = "test@example.com",
            RespondentName = "Test User",
            Answers = new List<SubmissionAnswerDto>
            {
                new()
                {
                    QuestionId = multiSelectQuestion.Id,
                    Values = selectedOptions.Select(opt => new AnswerValueDto 
                    { 
                        SelectedOptionId = opt.Id 
                    }).ToList()
                }
            }
        };

        // Act
        var result = await _service.SubmitFormResponseAsync(form.PublicKey!, request);

        // Assert
        result.StatusCode.Should().Be(200);
        
        // Verify all selections were saved
        var savedSubmission = await DbContext.Submissions
            .Include(s => s.Answers)
                .ThenInclude(a => a.AnswerValues)
            .FirstAsync();
        
        var savedAnswer = savedSubmission.Answers.First();
        savedAnswer.AnswerValues.Should().HaveCount(2);
        savedAnswer.AnswerValues.Select(av => av.SelectedOptionId).Should().BeEquivalentTo(selectedOptions.Select(o => o.Id));
    }

    [Fact]
    public async Task SubmitFormResponseAsync_WithTextAnswer_ShouldSaveText()
    {
        // Arrange
        var form = await CreateFormWithTextQuestion();
        var textQuestion = form.Pages.First().Questions.First();
        var textValue = "This is my text answer";
        
        var request = new SubmitFormRequest
        {
            RespondentEmail = "test@example.com",
            RespondentName = "Test User",
            Answers = new List<SubmissionAnswerDto>
            {
                new()
                {
                    QuestionId = textQuestion.Id,
                    Values = new List<AnswerValueDto>
                    {
                        new() { TextValue = textValue }
                    }
                }
            }
        };

        // Act
        var result = await _service.SubmitFormResponseAsync(form.PublicKey!, request);

        // Assert
        result.StatusCode.Should().Be(200);
        
        // Verify text was saved correctly
        var savedSubmission = await DbContext.Submissions
            .Include(s => s.Answers)
                .ThenInclude(a => a.AnswerValues)
            .FirstAsync();
        
        var savedAnswer = savedSubmission.Answers.First();
        var savedAnswerValue = savedAnswer.AnswerValues.First();
        savedAnswerValue.TextValue.Should().Be(textValue);
    }

    [Fact]
    public async Task SubmitFormResponseAsync_WithSliderAnswer_ShouldSaveNumericValue()
    {
        // Arrange
        var form = await CreateFormWithSliderQuestion();
        var sliderQuestion = form.Pages.First().Questions.First();
        var numericValue = 75.5m;
        
        var request = new SubmitFormRequest
        {
            RespondentEmail = "test@example.com",
            RespondentName = "Test User",
            Answers = new List<SubmissionAnswerDto>
            {
                new()
                {
                    QuestionId = sliderQuestion.Id,
                    Values = new List<AnswerValueDto>
                    {
                        new() { NumericValue = numericValue }
                    }
                }
            }
        };

        // Act
        var result = await _service.SubmitFormResponseAsync(form.PublicKey!, request);

        // Assert
        result.StatusCode.Should().Be(200);
        
        // Verify numeric value was saved correctly
        var savedSubmission = await DbContext.Submissions
            .Include(s => s.Answers)
                .ThenInclude(a => a.AnswerValues)
            .FirstAsync();
        
        var savedAnswer = savedSubmission.Answers.First();
        var savedAnswerValue = savedAnswer.AnswerValues.First();
        savedAnswerValue.NumericValue.Should().Be(numericValue);
    }

    [Fact]
    public async Task SubmitFormResponseAsync_WithMatrixAnswer_ShouldSaveMatrixSelection()
    {
        // Arrange
        var form = await CreateFormWithMatrixQuestion();
        var matrixQuestion = form.Pages.First().Questions.First();
        var matrixRow = matrixQuestion.MatrixRows.First();
        var matrixColumn = matrixQuestion.MatrixColumns.First();
        
        var request = new SubmitFormRequest
        {
            RespondentEmail = "test@example.com",
            RespondentName = "Test User",
            Answers = new List<SubmissionAnswerDto>
            {
                new()
                {
                    QuestionId = matrixQuestion.Id,
                    Values = new List<AnswerValueDto>
                    {
                        new() 
                        { 
                            MatrixRowId = matrixRow.Id,
                            SelectedMatrixColumnId = matrixColumn.Id
                        }
                    }
                }
            }
        };

        // Act
        var result = await _service.SubmitFormResponseAsync(form.PublicKey!, request);

        // Assert
        result.StatusCode.Should().Be(200);
        
        // Verify matrix selection was saved correctly
        var savedSubmission = await DbContext.Submissions
            .Include(s => s.Answers)
                .ThenInclude(a => a.AnswerValues)
            .FirstAsync();
        
        var savedAnswer = savedSubmission.Answers.First();
        var savedAnswerValue = savedAnswer.AnswerValues.First();
        savedAnswerValue.MatrixRowId.Should().Be(matrixRow.Id);
        savedAnswerValue.SelectedMatrixColumnId.Should().Be(matrixColumn.Id);
    }

    [Fact]
    public async Task SubmitFormResponseAsync_WithTrimmedEmailAndName_ShouldTrimWhitespace()
    {
        // Arrange
        var form = await CreatePublishedFormWithQuestions();
        var request = CreateValidSubmissionRequest(form);
        request.RespondentEmail = "  test@example.com  ";
        request.RespondentName = "  Test User  ";

        // Act
        var result = await _service.SubmitFormResponseAsync(form.PublicKey!, request);

        // Assert
        result.StatusCode.Should().Be(200);
        
        var submissionResponse = result.Data.Should().BeOfType<SubmissionResponseDto>().Subject;
        submissionResponse.RespondentEmail.Should().Be("test@example.com");
        submissionResponse.RespondentName.Should().Be("Test User");
    }

    // Helper methods for creating test forms and requests

    private async Task<Form> CreatePublishedFormWithQuestions()
    {
        var questionType = await DbContext.MasterQuestionTypes.FirstAsync(qt => qt.TypeName == "Radio");
        
        var form = new Form
        {
            Title = "Test Survey",
            Description = "Test Description",
            IsPublished = true,
            AllowResubmission = true,
            PublicKey = GeneratePublicKey(),
            PublicURL = $"/public/forms/{GeneratePublicKey()}",
            CreatedBy = 1,
            Pages = new List<Page>
            {
                new()
                {
                    Title = "Page 1",
                    PageOrder = 1,
                    CreatedBy = 1,
                    Questions = new List<Question>
                    {
                        new()
                        {
                            QuestionText = "What is your favorite color?",
                            QuestionTypeId = questionType.Id,
                            QuestionType = questionType,
                            IsRequired = false,
                            QuestionOrder = 1,
                            CreatedBy = 1,
                            Options = new List<Option>
                            {
                                new() { OptionText = "Red", DisplayOrder = 1, CreatedBy = 1 },
                                new() { OptionText = "Blue", DisplayOrder = 2, CreatedBy = 1 },
                                new() { OptionText = "Green", DisplayOrder = 3, CreatedBy = 1 }
                            }
                        }
                    }
                }
            }
        };

        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();
        return form;
    }

    private async Task<Form> CreateUnpublishedFormWithQuestions()
    {
        var questionType = await DbContext.MasterQuestionTypes.FirstAsync(qt => qt.TypeName == "Radio");
        
        var form = new Form
        {
            Title = "Unpublished Survey",
            Description = "Test Description",
            IsPublished = false, // Not published
            AllowResubmission = true,
            CreatedBy = 1,
            Pages = new List<Page>
            {
                new()
                {
                    Title = "Page 1",
                    PageOrder = 1,
                    CreatedBy = 1,
                    Questions = new List<Question>
                    {
                        new()
                        {
                            QuestionText = "Test Question",
                            QuestionTypeId = questionType.Id,
                            IsRequired = false,
                            QuestionOrder = 1,
                            CreatedBy = 1,
                            Options = new List<Option>
                            {
                                new() { OptionText = "Option 1", DisplayOrder = 1, CreatedBy = 1 }
                            }
                        }
                    }
                }
            }
        };

        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();
        return form;
    }

    private async Task<Form> CreatePublishedFormWithRequiredQuestions()
    {
        var questionType = await DbContext.MasterQuestionTypes.FirstAsync(qt => qt.TypeName == "Radio");
        
        var form = new Form
        {
            Title = "Required Questions Survey",
            Description = "Test Description",
            IsPublished = true,
            AllowResubmission = true,
            PublicKey = GeneratePublicKey(),
            PublicURL = $"/public/forms/{GeneratePublicKey()}",
            CreatedBy = 1,
            Pages = new List<Page>
            {
                new()
                {
                    Title = "Page 1",
                    PageOrder = 1,
                    CreatedBy = 1,
                    Questions = new List<Question>
                    {
                        new()
                        {
                            QuestionText = "Required Question",
                            QuestionTypeId = questionType.Id,
                            QuestionType = questionType,
                            IsRequired = true, // Required question
                            QuestionOrder = 1,
                            CreatedBy = 1,
                            Options = new List<Option>
                            {
                                new() { OptionText = "Yes", DisplayOrder = 1, CreatedBy = 1 },
                                new() { OptionText = "No", DisplayOrder = 2, CreatedBy = 1 }
                            }
                        }
                    }
                }
            }
        };

        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();
        return form;
    }

    private async Task<Form> CreatePublishedFormWithNoResubmission()
    {
        var form = await CreatePublishedFormWithQuestions();
        form.AllowResubmission = false; // No resubmission allowed
        await DbContext.SaveChangesAsync();
        return form;
    }

    private async Task<Form> CreatePublishedFormWithResubmissionAllowed()
    {
        var form = await CreatePublishedFormWithQuestions();
        form.AllowResubmission = true; // Resubmission allowed
        await DbContext.SaveChangesAsync();
        return form;
    }

    private async Task<Form> CreateFormWithRadioQuestion()
    {
        return await CreatePublishedFormWithQuestions(); // Already creates radio question
    }

    private async Task<Form> CreateFormWithMultiSelectQuestion()
    {
        var questionType = await DbContext.MasterQuestionTypes.FirstAsync(qt => qt.TypeName == "Multi");
        
        var form = new Form
        {
            Title = "Multi-Select Survey",
            Description = "Test Description",
            IsPublished = true,
            AllowResubmission = true,
            PublicKey = GeneratePublicKey(),
            PublicURL = $"/public/forms/{GeneratePublicKey()}",
            CreatedBy = 1,
            Pages = new List<Page>
            {
                new()
                {
                    Title = "Page 1",
                    PageOrder = 1,
                    CreatedBy = 1,
                    Questions = new List<Question>
                    {
                        new()
                        {
                            QuestionText = "Select multiple options",
                            QuestionTypeId = questionType.Id,
                            QuestionType = questionType,
                            IsRequired = false,
                            QuestionOrder = 1,
                            CreatedBy = 1,
                            Options = new List<Option>
                            {
                                new() { OptionText = "Option 1", DisplayOrder = 1, CreatedBy = 1 },
                                new() { OptionText = "Option 2", DisplayOrder = 2, CreatedBy = 1 },
                                new() { OptionText = "Option 3", DisplayOrder = 3, CreatedBy = 1 }
                            }
                        }
                    }
                }
            }
        };

        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();
        return form;
    }

    private async Task<Form> CreateFormWithTextQuestion()
    {
        var questionType = await DbContext.MasterQuestionTypes.FirstAsync(qt => qt.TypeName == "Text");
        
        var form = new Form
        {
            Title = "Text Question Survey",
            Description = "Test Description",
            IsPublished = true,
            AllowResubmission = true,
            PublicKey = GeneratePublicKey(),
            PublicURL = $"/public/forms/{GeneratePublicKey()}",
            CreatedBy = 1,
            Pages = new List<Page>
            {
                new()
                {
                    Title = "Page 1",
                    PageOrder = 1,
                    CreatedBy = 1,
                    Questions = new List<Question>
                    {
                        new()
                        {
                            QuestionText = "Enter your comment",
                            QuestionTypeId = questionType.Id,
                            QuestionType = questionType,
                            IsRequired = false,
                            QuestionOrder = 1,
                            CreatedBy = 1
                        }
                    }
                }
            }
        };

        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();
        return form;
    }

    private async Task<Form> CreateFormWithSliderQuestion()
    {
        var questionType = await DbContext.MasterQuestionTypes.FirstAsync(qt => qt.TypeName == "Slider");
        
        var form = new Form
        {
            Title = "Slider Question Survey",
            Description = "Test Description",
            IsPublished = true,
            AllowResubmission = true,
            PublicKey = GeneratePublicKey(),
            PublicURL = $"/public/forms/{GeneratePublicKey()}",
            CreatedBy = 1,
            Pages = new List<Page>
            {
                new()
                {
                    Title = "Page 1",
                    PageOrder = 1,
                    CreatedBy = 1,
                    Questions = new List<Question>
                    {
                        new()
                        {
                            QuestionText = "Rate your satisfaction",
                            QuestionTypeId = questionType.Id,
                            QuestionType = questionType,
                            IsRequired = false,
                            QuestionOrder = 1,
                            CreatedBy = 1,
                            SliderConfig = new SliderConfig
                            {
                                MinValue = 0,
                                MaxValue = 100,
                                StepValue = 1,
                                MinLabel = "Not Satisfied",
                                MaxLabel = "Very Satisfied",
                                CreatedBy = 1
                            }
                        }
                    }
                }
            }
        };

        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();
        return form;
    }

    private async Task<Form> CreateFormWithMatrixQuestion()
    {
        var questionType = await DbContext.MasterQuestionTypes.FirstAsync(qt => qt.TypeName == "Matrix");
        
        var form = new Form
        {
            Title = "Matrix Question Survey",
            Description = "Test Description",
            IsPublished = true,
            AllowResubmission = true,
            PublicKey = GeneratePublicKey(),
            PublicURL = $"/public/forms/{GeneratePublicKey()}",
            CreatedBy = 1,
            Pages = new List<Page>
            {
                new()
                {
                    Title = "Page 1",
                    PageOrder = 1,
                    CreatedBy = 1,
                    Questions = new List<Question>
                    {
                        new()
                        {
                            QuestionText = "Rate these items",
                            QuestionTypeId = questionType.Id,
                            QuestionType = questionType,
                            IsRequired = false,
                            QuestionOrder = 1,
                            CreatedBy = 1,
                            MatrixRows = new List<MatrixRow>
                            {
                                new() { RowLabel = "Quality", DisplayOrder = 1, CreatedBy = 1 },
                                new() { RowLabel = "Price", DisplayOrder = 2, CreatedBy = 1 }
                            },
                            MatrixColumns = new List<MatrixColumn>
                            {
                                new() { ColumnLabel = "Poor", DisplayOrder = 1, Score = 1, CreatedBy = 1 },
                                new() { ColumnLabel = "Good", DisplayOrder = 2, Score = 3, CreatedBy = 1 },
                                new() { ColumnLabel = "Excellent", DisplayOrder = 3, Score = 5, CreatedBy = 1 }
                            }
                        }
                    }
                }
            }
        };

        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();
        return form;
    }

    private SubmitFormRequest CreateValidSubmissionRequest(Form form)
    {
        var question = form.Pages.First().Questions.First();
        var request = new SubmitFormRequest
        {
            RespondentEmail = "test@example.com",
            RespondentName = "Test User",
            Answers = new List<SubmissionAnswerDto>
            {
                new()
                {
                    QuestionId = question.Id,
                    Values = new List<AnswerValueDto>()
                }
            }
        };

        // Add appropriate answer values based on question type
        if (question.Options.Any())
        {
            // Radio, Dropdown, or Multi-select
            request.Answers.First().Values.Add(new AnswerValueDto 
            { 
                SelectedOptionId = question.Options.First().Id 
            });
        }
        else if (question.QuestionType?.TypeName == "Text")
        {
            request.Answers.First().Values.Add(new AnswerValueDto 
            { 
                TextValue = "Sample text answer" 
            });
        }
        else if (question.QuestionType?.TypeName == "Slider")
        {
            request.Answers.First().Values.Add(new AnswerValueDto 
            { 
                NumericValue = 50 
            });
        }
        else if (question.MatrixRows.Any() && question.MatrixColumns.Any())
        {
            request.Answers.First().Values.Add(new AnswerValueDto 
            { 
                MatrixRowId = question.MatrixRows.First().Id,
                SelectedMatrixColumnId = question.MatrixColumns.First().Id
            });
        }

        return request;
    }

    private static string GeneratePublicKey()
    {
        return Guid.NewGuid().ToString("N")[..16].ToUpper();
    }
}
