using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Services.Test.DynamicQuestionnaire;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace App.Services.Test.DynamicQuestionnaire;

public class FormPublishingTests : DynamicQuestionnaireTestBase
{
    private readonly IDynamicQuestionnaireService _service;

    public FormPublishingTests()
    {
        _service = ServiceProvider.GetRequiredService<IDynamicQuestionnaireService>();
    }

    [Fact]
    public async Task PublishFormAsync_WithValidForm_ShouldPublishSuccessfully()
    {
        // Arrange
        var form = new Form
        {
            Title = "Test Survey",
            Description = "Test Description",
            IsPublished = false,
            CreatedBy = 1,
            Pages = new List<Page>
            {
                new Page
                {
                    Title = "Page 1",
                    PageOrder = 1,
                    CreatedBy = 1,
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            QuestionText = "Test Question",
                            QuestionTypeId = 1,
                            IsRequired = true,
                            QuestionOrder = 1,
                            CreatedBy = 1
                        }
                    }
                }
            }
        };

        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.PublishFormAsync(form.Id, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        
        var publishedForm = await DbContext.Forms.FindAsync(form.Id);
        publishedForm.Should().NotBeNull();
        publishedForm!.IsPublished.Should().BeTrue();
        publishedForm.PublishedDate.Should().NotBeNull();
        publishedForm.PublicKey.Should().NotBeNullOrEmpty();
        publishedForm.PublicURL.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task PublishFormAsync_WithNonExistentForm_ShouldReturn404()
    {
        // Act
        var result = await _service.PublishFormAsync(999, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("Form not found");
    }

    [Fact]
    public async Task PublishFormAsync_WithAlreadyPublishedForm_ShouldReturn400()
    {
        // Arrange
        var form = new Form
        {
            Title = "Already Published Survey",
            Description = "Test Description",
            IsPublished = true,
            PublishedDate = DateTime.UtcNow,
            PublicKey = "TEST123456789012",
            PublicURL = "/public/forms/TEST123456789012",
            CreatedBy = 1
        };

        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.PublishFormAsync(form.Id, 1);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(400);
        result.Message.Should().Be("Form is already published");
    }

    [Fact]
    public async Task GetPublicFormAsync_WithValidPublicKey_ShouldReturnForm()
    {
        // Arrange
        var questionType = new MasterQuestionType { TypeName = "Text", IsActive = true };
        DbContext.MasterQuestionTypes.Add(questionType);
        await DbContext.SaveChangesAsync();

        var form = new Form
        {
            Title = "Public Survey",
            Description = "Public Description",
            Instructions = "Please fill out this survey",
            IsPublished = true,
            PublishedDate = DateTime.UtcNow,
            PublicKey = "PUBLICKEY123456",
            PublicURL = "/public/forms/PUBLICKEY123456",
            AllowResubmission = true,
            CreatedBy = 1,
            Pages = new List<Page>
            {
                new Page
                {
                    Title = "Page 1",
                    Description = "First page",
                    PageOrder = 1,
                    CreatedBy = 1,
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            QuestionText = "What is your name?",
                            QuestionTypeId = questionType.Id,
                            IsRequired = true,
                            QuestionOrder = 1,
                            CreatedBy = 1
                        }
                    }
                }
            }
        };

        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetPublicFormAsync("PUBLICKEY123456");

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        
        var publicForm = result.Data as PublicFormResponseDto;
        publicForm.Should().NotBeNull();
        publicForm!.PublicKey.Should().Be("PUBLICKEY123456");
        publicForm.Title.Should().Be("Public Survey");
        publicForm.Description.Should().Be("Public Description");
        publicForm.Instructions.Should().Be("Please fill out this survey");
        publicForm.AllowResubmission.Should().BeTrue();
        publicForm.Pages.Should().HaveCount(1);
        publicForm.Pages[0].Questions.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPublicFormAsync_WithInvalidPublicKey_ShouldReturn404()
    {
        // Act
        var result = await _service.GetPublicFormAsync("INVALIDKEY123");

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("Form not found or not published");
    }

    [Fact]
    public async Task GetPublicFormAsync_WithUnpublishedForm_ShouldReturn404()
    {
        // Arrange
        var form = new Form
        {
            Title = "Unpublished Survey",
            Description = "Test Description",
            IsPublished = false,
            PublicKey = "UNPUBLISHED123",
            CreatedBy = 1
        };

        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetPublicFormAsync("UNPUBLISHED123");

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(404);
        result.Message.Should().Be("Form not found or not published");
    }
}
