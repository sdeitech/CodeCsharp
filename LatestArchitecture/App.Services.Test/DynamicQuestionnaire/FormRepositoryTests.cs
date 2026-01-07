using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Domain.Entities.DynamicQuestionnaire;
using App.Domain.Enums.DynamicQuestionnaire;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace App.Services.Test.DynamicQuestionnaire;

public class FormRepositoryTests : DynamicQuestionnaireTestBase
{
    private readonly IFormRepository _formRepository;

    public FormRepositoryTests()
    {
        _formRepository = ServiceProvider.GetRequiredService<IFormRepository>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllForms()
    {
        // Arrange
        var form1 = CreateTestForm("Form 1");
        var form2 = CreateTestForm("Form 2");
        
        DbContext.Forms.AddRange(form1, form2);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _formRepository.GetAllAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(f => f.Title == "Form 1");
        result.Should().Contain(f => f.Title == "Form 2");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnForm()
    {
        // Arrange
        var form = CreateTestForm("Test Form");
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _formRepository.GetByIdAsync(form.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Form");
        result.Description.Should().Be("Test Description");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _formRepository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_ShouldReturnFormWithPages()
    {
        // Arrange
        var form = CreateTestForm("Form with Pages");
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        var page1 = CreateTestPage(form.Id, 1);
        var page2 = CreateTestPage(form.Id, 2);
        DbContext.Pages.AddRange(page1, page2);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _formRepository.GetByIdWithDetailsAsync(form.Id, 1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Form with Pages");
        result.Pages.Should().HaveCount(2);
        result.Pages.Should().Contain(p => p.PageOrder == 1);
        result.Pages.Should().Contain(p => p.PageOrder == 2);
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_ShouldReturnFormWithAllRelatedData()
    {
        // Arrange
        var form = CreateTestForm("Complete Form");
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        var page = CreateTestPage(form.Id);
        DbContext.Pages.Add(page);
        await DbContext.SaveChangesAsync();

        var question = CreateTestQuestion(page.Id, QuestionType.Radio);
        DbContext.Questions.Add(question);
        await DbContext.SaveChangesAsync();

        var option = CreateTestOption(question.Id);
        DbContext.Options.Add(option);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _formRepository.GetByIdWithDetailsAsync(form.Id, 1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Complete Form");
        result.Pages.Should().HaveCount(1);
        result.Pages.First().Questions.Should().HaveCount(1);
        result.Pages.First().Questions.First().Options.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddFormToDatabase()
    {
        // Arrange
        var form = CreateTestForm("New Form");

        // Act
        await _formRepository.CreateAsync(form);

        // Assert
        var savedForm = await DbContext.Forms.FindAsync(form.Id);
        savedForm.Should().NotBeNull();
        savedForm!.Title.Should().Be("New Form");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingForm()
    {
        // Arrange
        var form = CreateTestForm("Original Title");
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        // Act
        form.Title = "Updated Title";
        form.Description = "Updated Description";
        await _formRepository.UpdateAsync(form);

        // Assert
        var updatedForm = await DbContext.Forms.FindAsync(form.Id);
        updatedForm.Should().NotBeNull();
        updatedForm!.Title.Should().Be("Updated Title");
        updatedForm.Description.Should().Be("Updated Description");
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteForm()
    {
        // Arrange
        var form = CreateTestForm("Form to Delete");
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _formRepository.DeleteAsync(form.Id, 1);

        // Assert
        result.Should().BeTrue();
        var deletedForm = await DbContext.Forms.FindAsync(form.Id);
        deletedForm.Should().NotBeNull();
        deletedForm!.IsDeleted.Should().BeTrue();
        deletedForm.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ExistsAsync_WithExistingForm_ShouldReturnTrue()
    {
        // Arrange
        var form = CreateTestForm("Existing Form");
        DbContext.Forms.Add(form);
        await DbContext.SaveChangesAsync();

        // Act
        var exists = await _formRepository.ExistsAsync(form.Id);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingForm_ShouldReturnFalse()
    {
        // Act
        var exists = await _formRepository.ExistsAsync(999);

        // Assert
        exists.Should().BeFalse();
    }
}