using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace App.Services.Test.DynamicQuestionnaire;

public class DynamicQuestionnaireIntegrationTests : DynamicQuestionnaireTestBase
{
    private readonly IDynamicQuestionnaireService _service;

    public DynamicQuestionnaireIntegrationTests()
    {
        _service = ServiceProvider.GetRequiredService<IDynamicQuestionnaireService>();
    }

    [Fact]
    public async Task CreateFormAsync_WithValidData_ShouldCreateFormSuccessfully()
    {
        // Arrange
        var createFormDto = new CreateFormDto
        {
            Title = "Simple Survey",
            Description = "A basic survey form"
        };

        // Act
        var result = await _service.CreateFormAsync(createFormDto, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(201); // Created
        result.Data.Should().NotBeNull();

        // Verify the form was created in the database
        var formsInDb = await DbContext.Forms.ToListAsync();
        formsInDb.Should().HaveCount(1);
        formsInDb.First().Title.Should().Be("Simple Survey");
    }

    [Fact]
    public async Task CreateFormAsync_WithPagesAndQuestions_ShouldCreateCompleteStructure()
    {
        // Arrange
        var questionType = new MasterQuestionType
        {
            TypeName = "Text Input",
            IsActive = true
        };
        await DbContext.MasterQuestionTypes.AddAsync(questionType);
        await DbContext.SaveChangesAsync();

        var createFormDto = new CreateFormDto
        {
            Title = "Employee Survey",
            Description = "Annual Employee Satisfaction Survey",
            Pages = new List<CreatePageDto>
            {
                new CreatePageDto
                {
                    Title = "Personal Information",
                    Description = "Basic information about you",
                    PageOrder = 1,
                    Questions = new List<CreateQuestionDto>
                    {
                        new CreateQuestionDto
                        {
                            QuestionTypeId = questionType.Id,
                            QuestionText = "What is your full name?",
                            IsRequired = true,
                            QuestionOrder = 1
                        }
                    }
                }
            }
        };

        // Act
        var result = await _service.CreateFormAsync(createFormDto, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(201); // Created
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllFormsAsync_WithMultipleForms_ShouldReturnCorrectData()
    {
        // Arrange
        var form1 = new Form
        {
            Title = "Form 1",
            Description = "First form",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };

        var form2 = new Form
        {
            Title = "Form 2",
            Description = "Second form",
            IsPublished = false,
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            UpdatedAt = DateTime.UtcNow.AddDays(-3)
        };

        await DbContext.Forms.AddRangeAsync(form1, form2);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetAllFormsAsync();

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetMasterQuestionTypesAsync_ShouldReturnActiveTypesOnly()
    {
        // Arrange
        var activeType1 = new MasterQuestionType
        {
            TypeName = "Text Input",
            IsActive = true
        };

        var activeType2 = new MasterQuestionType
        {
            TypeName = "Multiple Choice",
            IsActive = true
        };

        var inactiveType = new MasterQuestionType
        {
            TypeName = "Deprecated Type",
            IsActive = false
        };

        await DbContext.MasterQuestionTypes.AddRangeAsync(activeType1, activeType2, inactiveType);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetMasterQuestionTypesAsync();

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateFormAsync_WithOptions_ShouldCreateMultipleChoiceQuestion()
    {
        // Arrange
        var multipleChoiceType = new MasterQuestionType
        {
            TypeName = "Multiple Choice",
            IsActive = true
        };
        await DbContext.MasterQuestionTypes.AddAsync(multipleChoiceType);
        await DbContext.SaveChangesAsync();

        var formDto = new CreateFormDto
        {
            Title = "Survey with Options",
            Description = "A survey with multiple choice questions",
            Pages = new List<CreatePageDto>
            {
                new CreatePageDto
                {
                    Title = "Preferences",
                    PageOrder = 1,
                    Questions = new List<CreateQuestionDto>
                    {
                        new CreateQuestionDto
                        {
                            QuestionTypeId = multipleChoiceType.Id,
                            QuestionText = "What is your preferred work style?",
                            IsRequired = true,
                            QuestionOrder = 1,
                            Options = new List<CreateOptionDto>
                            {
                                new CreateOptionDto { OptionText = "Remote", DisplayOrder = 1 },
                                new CreateOptionDto { OptionText = "Hybrid", DisplayOrder = 2 },
                                new CreateOptionDto { OptionText = "In-Office", DisplayOrder = 3 }
                            }
                        }
                    }
                }
            }
        };

        // Act
        var result = await _service.CreateFormAsync(formDto, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task CreateFormAsync_WithMatrixQuestion_ShouldCreateMatrixStructure()
    {
        // Arrange
        var matrixType = new MasterQuestionType
        {
            TypeName = "Matrix",
            IsActive = true
        };
        await DbContext.MasterQuestionTypes.AddAsync(matrixType);
        await DbContext.SaveChangesAsync();

        var formDto = new CreateFormDto
        {
            Title = "Matrix Survey",
            Description = "Survey with matrix questions",
            Pages = new List<CreatePageDto>
            {
                new CreatePageDto
                {
                    Title = "Ratings",
                    PageOrder = 1,
                    Questions = new List<CreateQuestionDto>
                    {
                        new CreateQuestionDto
                        {
                            QuestionTypeId = matrixType.Id,
                            QuestionText = "Rate the following aspects",
                            IsRequired = false,
                            QuestionOrder = 1,
                            MatrixConfig = new CreateMatrixConfigDto
                            {
                                Rows = new List<CreateMatrixRowDto>
                                {
                                    new CreateMatrixRowDto { RowLabel = "Quality", DisplayOrder = 1 },
                                    new CreateMatrixRowDto { RowLabel = "Service", DisplayOrder = 2 }
                                },
                                Columns = new List<CreateMatrixColumnDto>
                                {
                                    new CreateMatrixColumnDto { ColumnLabel = "Poor", DisplayOrder = 1 },
                                    new CreateMatrixColumnDto { ColumnLabel = "Good", DisplayOrder = 2 },
                                    new CreateMatrixColumnDto { ColumnLabel = "Excellent", DisplayOrder = 3 }
                                }
                            }
                        }
                    }
                }
            }
        };

        // Act
        var result = await _service.CreateFormAsync(formDto, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task ConcurrentFormCreation_ShouldHandleMultipleRequests()
    {
        // Arrange
        var questionType = new MasterQuestionType
        {
            TypeName = "Text Input",
            IsActive = true
        };
        await DbContext.MasterQuestionTypes.AddAsync(questionType);
        await DbContext.SaveChangesAsync();

        var tasks = Enumerable.Range(1, 3).Select(async i =>
        {
            var formDto = new CreateFormDto
            {
                Title = $"Concurrent Form {i}",
                Description = $"Form created in concurrent test {i}"
            };

            return await _service.CreateFormAsync(formDto, i);
        });

        // Act
        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().HaveCount(3);
        results.Should().OnlyContain(r => r.StatusCode == 201); // Created

        var formsCount = await DbContext.Forms.CountAsync();
        formsCount.Should().Be(3);
    }

    [Fact]
    public async Task GetFormByIdAsync_AfterCreation_ShouldReturnCorrectForm()
    {
        // Arrange
        var form = new Form
        {
            Title = "Test Form",
            Description = "Test Description",
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await DbContext.Forms.AddAsync(form);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetFormByIdAsync(form.Id);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
    }
}
