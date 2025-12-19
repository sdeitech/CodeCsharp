using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Common.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace App.Services.Test.DynamicQuestionnaire;

public class DynamicQuestionnaireServiceTests : DynamicQuestionnaireTestBase
{
    private readonly IDynamicQuestionnaireService _service;

    public DynamicQuestionnaireServiceTests()
    {
        _service = ServiceProvider.GetRequiredService<IDynamicQuestionnaireService>();
    }

    [Fact]
    public async Task CreateFormAsync_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var createDto = new CreateFormDto
        {
            Title = "Test Form",
            Description = "Test Description"
        };

        // Act
        var result = await _service.CreateFormAsync(createDto, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(201); // Created
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetFormByIdAsync_WithValidId_ShouldReturnForm()
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

    [Fact]
    public async Task GetFormByIdAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var result = await _service.GetFormByIdAsync(999);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetAllFormsAsync_ShouldReturnAllForms()
    {
        // Arrange
        var form1 = new Form
        {
            Title = "Form 1",
            Description = "Description 1",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var form2 = new Form
        {
            Title = "Form 2",
            Description = "Description 2",
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
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
    public async Task GetMasterQuestionTypesAsync_ShouldReturnQuestionTypes()
    {
        // Arrange
        var questionType1 = new MasterQuestionType
        {
            TypeName = "Text Input",
            IsActive = true
        };

        var questionType2 = new MasterQuestionType
        {
            TypeName = "Multiple Choice",
            IsActive = true
        };

        await DbContext.MasterQuestionTypes.AddRangeAsync(questionType1, questionType2);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetMasterQuestionTypesAsync();

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateFormAsync_WithPagesAndQuestions_ShouldCreateCompleteForm()
    {
        // Arrange
        var createDto = new CreateFormDto
        {
            Title = "Survey Form",
            Description = "A comprehensive survey",
            Pages = new List<CreatePageDto>
            {
                new CreatePageDto
                {
                    Title = "Page 1",
                    Description = "First page",
                    PageOrder = 1,
                    Questions = new List<CreateQuestionDto>
                    {
                        new CreateQuestionDto
                        {
                            QuestionTypeId = 1,
                            QuestionText = "What is your name?",
                            IsRequired = true,
                            QuestionOrder = 1
                        }
                    }
                }
            }
        };

        // Act
        var result = await _service.CreateFormAsync(createDto, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(201); // Created
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteFormAsync_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var form = new Form
        {
            Title = "Form To Delete",
            Description = "Will be deleted",
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await DbContext.Forms.AddAsync(form);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.DeleteFormAsync(form.Id, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Message.Should().Be(App.Common.Constant.StatusMessage.FormDeletedSuccessfully);

        var deleted = await DbContext.Forms.FindAsync(form.Id);
        deleted.Should().NotBeNull();
        deleted!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteFormAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var result = await _service.DeleteFormAsync(9999, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be(App.Common.Constant.StatusMessage.FormNotFound);
    }
}
