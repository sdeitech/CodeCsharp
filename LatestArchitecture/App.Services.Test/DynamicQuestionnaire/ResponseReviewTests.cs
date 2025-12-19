// using App.Application.Dto.DynamicQuestionnaire;
// using App.Application.Interfaces.Services.DynamicQuestionnaire;
// using App.Domain.Entities.DynamicQuestionnaire;
// using FluentAssertions;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.EntityFrameworkCore;
// using Xunit;

// namespace App.Services.Test.DynamicQuestionnaire;

// public class ResponseReviewTests : DynamicQuestionnaireTestBase
// {
//     private readonly IDynamicQuestionnaireService _service;

//     public ResponseReviewTests()
//     {
//         _service = ServiceProvider.GetRequiredService<IDynamicQuestionnaireService>();
//     }

//     [Fact]
//     public async Task GetFormResponsesAsync_WithValidFormId_ShouldReturnPaginatedResponses()
//     {
//         // Arrange
//         var form = await CreatePublishedFormWithSubmissions();
//         var filter = new ResponseFilterDto
//         {
//             PageNumber = 1,
//             PageSize = 10
//         };

//         // Act
//         var result = await _service.GetFormResponsesAsync(form.Id, filter);

//         // Assert
//         result.Should().NotBeNull();
//         result.StatusCode.Should().Be(200);
//         result.Message.Should().Be("Form responses retrieved successfully");

//         var pagedResult = result.Data.Should().BeOfType<App.Application.Dto.Common.PagedResult<FormResponseListDto>>().Subject;
//         pagedResult.Items.Should().HaveCount(3); // We created 3 submissions
//         pagedResult.TotalCount.Should().Be(3);
//         pagedResult.PageNumber.Should().Be(1);
//         pagedResult.PageSize.Should().Be(10);

//         // Verify the response data
//         var firstResponse = pagedResult.Items.First();
//         firstResponse.FormTitle.Should().Be(form.Title);
//         firstResponse.RespondentEmail.Should().NotBeNullOrEmpty();
//         firstResponse.SubmissionId.Should().BeGreaterThan(0);
//     }

//     [Fact]
//     public async Task GetFormResponsesAsync_WithInvalidFormId_ShouldReturn404()
//     {
//         // Arrange
//         var invalidFormId = 99999;
//         var filter = new ResponseFilterDto
//         {
//             PageNumber = 1,
//             PageSize = 10
//         };

//         // Act
//         var result = await _service.GetFormResponsesAsync(invalidFormId, filter);

//         // Assert
//         result.Should().NotBeNull();
//         result.StatusCode.Should().Be(404);
//         result.Message.Should().Be("Form not found");
//         result.Data.Should().BeNull();
//     }

//     [Fact]
//     public async Task GetFormResponsesAsync_WithEmailFilter_ShouldReturnFilteredResults()
//     {
//         // Arrange
//         var form = await CreatePublishedFormWithSubmissions();
//         var filter = new ResponseFilterDto
//         {
//             RespondentEmail = "john.doe",
//             PageNumber = 1,
//             PageSize = 10
//         };

//         // Act
//         var result = await _service.GetFormResponsesAsync(form.Id, filter);

//         // Assert
//         result.Should().NotBeNull();
//         result.StatusCode.Should().Be(200);

//         var pagedResult = result.Data.Should().BeOfType<App.Application.Dto.Common.PagedResult<FormResponseListDto>>().Subject;
//         pagedResult.Items.Should().HaveCount(1);
//         pagedResult.Items.First().RespondentEmail.Should().Contain("john.doe");
//     }

//     [Fact]
//     public async Task GetFormResponsesAsync_WithDateFilter_ShouldReturnFilteredResults()
//     {
//         // Arrange
//         var form = await CreatePublishedFormWithSubmissions();
//         var yesterday = DateTime.UtcNow.AddDays(-1);
//         var filter = new ResponseFilterDto
//         {
//             SubmittedDateFrom = yesterday,
//             PageNumber = 1,
//             PageSize = 10
//         };

//         // Act
//         var result = await _service.GetFormResponsesAsync(form.Id, filter);

//         // Assert
//         result.Should().NotBeNull();
//         result.StatusCode.Should().Be(200);

//         var pagedResult = result.Data.Should().BeOfType<App.Application.Dto.Common.PagedResult<FormResponseListDto>>().Subject;
//         pagedResult.Items.Should().HaveCount(3); // All submissions are from today
//         pagedResult.Items.Should().AllSatisfy(r => r.SubmittedDate.Should().BeAfter(yesterday));
//     }

//     [Fact]
//     public async Task GetFormResponsesAsync_WithSortByEmail_ShouldReturnSortedResults()
//     {
//         // Arrange
//         var form = await CreatePublishedFormWithSubmissions();
//         var filter = new ResponseFilterDto
//         {
//             SortBy = "RespondentEmail",
//             SortOrder = "asc",
//             PageNumber = 1,
//             PageSize = 10
//         };

//         // Act
//         var result = await _service.GetFormResponsesAsync(form.Id, filter);

//         // Assert
//         result.Should().NotBeNull();
//         result.StatusCode.Should().Be(200);

//         var pagedResult = result.Data.Should().BeOfType<App.Application.Dto.Common.PagedResult<FormResponseListDto>>().Subject;
//         pagedResult.Items.Should().HaveCount(3);

//         // Verify sorting
//         var emails = pagedResult.Items.Select(r => r.RespondentEmail).ToList();
//         emails.Should().BeInAscendingOrder();
//     }

//     [Fact]
//     public async Task GetFormResponsesAsync_WithPagination_ShouldReturnCorrectPage()
//     {
//         // Arrange
//         var form = await CreatePublishedFormWithSubmissions();
//         var filter = new ResponseFilterDto
//         {
//             PageNumber = 2,
//             PageSize = 2
//         };

//         // Act
//         var result = await _service.GetFormResponsesAsync(form.Id, filter);

//         // Assert
//         result.Should().NotBeNull();
//         result.StatusCode.Should().Be(200);

//         var pagedResult = result.Data.Should().BeOfType<App.Application.Dto.Common.PagedResult<FormResponseListDto>>().Subject;
//         pagedResult.Items.Should().HaveCount(1); // Only 1 item on page 2 (3 total, 2 per page)
//         pagedResult.TotalCount.Should().Be(3);
//         pagedResult.PageNumber.Should().Be(2);
//         pagedResult.PageSize.Should().Be(2);
//         pagedResult.TotalPages.Should().Be(2);
//     }

//     [Fact]
//     public async Task GetResponseByIdAsync_WithValidResponseId_ShouldReturnDetailedResponse()
//     {
//         // Arrange
//         var form = await CreatePublishedFormWithSubmissions();
//         var submission = await DbContext.Submissions
//             .FirstAsync(s => s.FormId == form.Id);

//         // Act
//         var result = await _service.GetResponseByIdAsync(submission.Id);

//         // Assert
//         result.Should().NotBeNull();
//         result.StatusCode.Should().Be(200);
//         result.Message.Should().Be("Response details retrieved successfully");

//         var responseDetail = result.Data.Should().BeOfType<ResponseDetailDto>().Subject;
//         responseDetail.SubmissionId.Should().Be(submission.Id);
//         responseDetail.FormId.Should().Be(form.Id);
//         responseDetail.FormTitle.Should().Be(form.Title);
//         responseDetail.RespondentEmail.Should().Be(submission.RespondentEmail);
//         responseDetail.SubmittedDate.Should().Be(submission.SubmittedDate);
//         responseDetail.Answers.Should().NotBeEmpty();

//         // Verify answer details
//         var firstAnswer = responseDetail.Answers.First();
//         firstAnswer.QuestionText.Should().NotBeNullOrEmpty();
//         firstAnswer.QuestionType.Should().NotBeNullOrEmpty();
//         firstAnswer.Values.Should().NotBeEmpty();
//     }

//     [Fact]
//     public async Task GetResponseByIdAsync_WithInvalidResponseId_ShouldReturn404()
//     {
//         // Arrange
//         var invalidResponseId = 99999;

//         // Act
//         var result = await _service.GetResponseByIdAsync(invalidResponseId);

//         // Assert
//         result.Should().NotBeNull();
//         result.StatusCode.Should().Be(404);
//         result.Message.Should().Be("Response not found");
//         result.Data.Should().BeNull();
//     }

//     [Fact]
//     public async Task GetResponseByIdAsync_WithTextAnswer_ShouldIncludeTextValue()
//     {
//         // Arrange
//         var form = await CreateFormWithTextQuestionAndSubmission();
//         var submission = await DbContext.Submissions
//             .FirstAsync(s => s.FormId == form.Id);

//         // Act
//         var result = await _service.GetResponseByIdAsync(submission.Id);

//         // Assert
//         result.Should().NotBeNull();
//         result.StatusCode.Should().Be(200);

//         var responseDetail = result.Data.Should().BeOfType<ResponseDetailDto>().Subject;
//         var textAnswer = responseDetail.Answers.First();
//         textAnswer.QuestionType.Should().Be("Text");
//         textAnswer.Values.Should().HaveCount(1);
//         textAnswer.Values.First().TextValue.Should().NotBeNullOrEmpty();
//     }

//     [Fact]
//     public async Task GetResponseByIdAsync_WithRadioAnswer_ShouldIncludeSelectedOption()
//     {
//         // Arrange
//         var form = await CreateFormWithRadioQuestionAndSubmission();
//         var submission = await DbContext.Submissions
//             .FirstAsync(s => s.FormId == form.Id);

//         // Act
//         var result = await _service.GetResponseByIdAsync(submission.Id);

//         // Assert
//         result.Should().NotBeNull();
//         result.StatusCode.Should().Be(200);

//         var responseDetail = result.Data.Should().BeOfType<ResponseDetailDto>().Subject;
//         var radioAnswer = responseDetail.Answers.First();
//         radioAnswer.QuestionType.Should().Be("Radio");
//         radioAnswer.Values.Should().HaveCount(1);
        
//         var answerValue = radioAnswer.Values.First();
//         answerValue.SelectedOptionId.Should().NotBeNull();
//         answerValue.SelectedOptionText.Should().NotBeNullOrEmpty();
//     }

//     [Fact]
//     public async Task GetResponseByIdAsync_WithSliderAnswer_ShouldIncludeNumericValue()
//     {
//         // Arrange
//         var form = await CreateFormWithSliderQuestionAndSubmission();
//         var submission = await DbContext.Submissions
//             .FirstAsync(s => s.FormId == form.Id);

//         // Act
//         var result = await _service.GetResponseByIdAsync(submission.Id);

//         // Assert
//         result.Should().NotBeNull();
//         result.StatusCode.Should().Be(200);

//         var responseDetail = result.Data.Should().BeOfType<ResponseDetailDto>().Subject;
//         var sliderAnswer = responseDetail.Answers.First();
//         sliderAnswer.QuestionType.Should().Be("Slider");
//         sliderAnswer.Values.Should().HaveCount(1);
//         sliderAnswer.Values.First().NumericValue.Should().NotBeNull();
//     }

//     [Fact]
//     public async Task GetResponseByIdAsync_WithMultiSelectAnswer_ShouldIncludeAllSelectedOptions()
//     {
//         // Arrange
//         var form = await CreateFormWithMultiSelectQuestionAndSubmission();
//         var submission = await DbContext.Submissions
//             .FirstAsync(s => s.FormId == form.Id);

//         // Act
//         var result = await _service.GetResponseByIdAsync(submission.Id);

//         // Assert
//         result.Should().NotBeNull();
//         result.StatusCode.Should().Be(200);

//         var responseDetail = result.Data.Should().BeOfType<ResponseDetailDto>().Subject;
//         var multiAnswer = responseDetail.Answers.First();
//         multiAnswer.QuestionType.Should().Be("Multi");
//         multiAnswer.Values.Should().HaveCountGreaterThan(1);
//         multiAnswer.Values.Should().AllSatisfy(v => v.SelectedOptionId.Should().NotBeNull());
//         multiAnswer.Values.Should().AllSatisfy(v => v.SelectedOptionText.Should().NotBeNullOrEmpty());
//     }

//     // Helper methods for creating test data

//     private async Task<Form> CreatePublishedFormWithSubmissions()
//     {
//         var form = await CreatePublishedFormWithQuestions();
        
//         // Create multiple submissions
//         var submissions = new[]
//         {
//             new Submission
//             {
//                 FormId = form.Id,
//                 RespondentEmail = "john.doe@example.com",
//                 RespondentName = "John Doe",
//                 SubmittedDate = DateTime.UtcNow.AddMinutes(-30),
//                 TotalScore = 85.5m,
//                 CreatedBy = 1
//             },
//             new Submission
//             {
//                 FormId = form.Id,
//                 RespondentEmail = "jane.smith@example.com",
//                 RespondentName = "Jane Smith",
//                 SubmittedDate = DateTime.UtcNow.AddMinutes(-20),
//                 TotalScore = 92.0m,
//                 CreatedBy = 1
//             },
//             new Submission
//             {
//                 FormId = form.Id,
//                 RespondentEmail = "bob.wilson@example.com",
//                 RespondentName = "Bob Wilson",
//                 SubmittedDate = DateTime.UtcNow.AddMinutes(-10),
//                 TotalScore = 78.5m,
//                 CreatedBy = 1
//             }
//         };

//         DbContext.Submissions.AddRange(submissions);
//         await DbContext.SaveChangesAsync();

//         // Get the question and options to create answers
//         var question = await DbContext.Questions
//             .Include(q => q.Options)
//             .FirstAsync(q => q.Page.FormId == form.Id);

//         // Create answers for each submission
//         var answers = new List<Answer>();
//         var answerValues = new List<AnswerValue>();

//         foreach (var submission in submissions)
//         {
//             var answer = new Answer
//             {
//                 SubmissionId = submission.Id,
//                 QuestionId = question.Id,
//                 CreatedBy = 1
//             };
//             answers.Add(answer);

//             // Create answer value - select first option for simplicity
//             var selectedOption = question.Options.First();
//             var answerValue = new AnswerValue
//             {
//                 Answer = answer,
//                 SelectedOptionId = selectedOption.Id,
//                 CreatedBy = 1
//             };
//             answerValues.Add(answerValue);
//         }

//         DbContext.Answers.AddRange(answers);
//         await DbContext.SaveChangesAsync();

//         // Set the answer references for answer values
//         for (int i = 0; i < answerValues.Count; i++)
//         {
//             answerValues[i].AnswerId = answers[i].Id;
//         }

//         DbContext.AnswerValues.AddRange(answerValues);
//         await DbContext.SaveChangesAsync();

//         return form;
//     }

//     private async Task<Form> CreatePublishedFormWithQuestions()
//     {
//         var questionType = await DbContext.MasterQuestionTypes.FirstAsync(qt => qt.TypeName == "Radio");
        
//         var form = new Form
//         {
//             Title = "Test Survey",
//             Description = "A test survey",
//             IsPublished = true,
//             PublicKey = GeneratePublicKey(),
//             CreatedBy = 1
//         };

//         var page = new Page
//         {
//             Form = form,
//             Title = "Page 1",
//             PageOrder = 1,
//             CreatedBy = 1
//         };

//         var question = new Question
//         {
//             Page = page,
//             QuestionTypeId = questionType.Id,
//             QuestionText = "How satisfied are you?",
//             IsRequired = true,
//             QuestionOrder = 1,
//             CreatedBy = 1
//         };

//         var options = new[]
//         {
//             new Option { Question = question, OptionText = "Very Satisfied", DisplayOrder = 1, Score = 100, CreatedBy = 1 },
//             new Option { Question = question, OptionText = "Satisfied", DisplayOrder = 2, Score = 75, CreatedBy = 1 },
//             new Option { Question = question, OptionText = "Neutral", DisplayOrder = 3, Score = 50, CreatedBy = 1 },
//             new Option { Question = question, OptionText = "Dissatisfied", DisplayOrder = 4, Score = 25, CreatedBy = 1 }
//         };

//         form.Pages.Add(page);
//         page.Questions.Add(question);
//         question.Options = options.ToList();

//         DbContext.Forms.Add(form);
//         await DbContext.SaveChangesAsync();

//         return form;
//     }

//     private async Task<Form> CreateFormWithTextQuestionAndSubmission()
//     {
//         var questionType = await DbContext.MasterQuestionTypes.FirstAsync(qt => qt.TypeName == "Text");
        
//         var form = new Form
//         {
//             Title = "Text Survey",
//             Description = "A survey with text questions",
//             IsPublished = true,
//             PublicKey = GeneratePublicKey(),
//             CreatedBy = 1
//         };

//         var page = new Page
//         {
//             Form = form,
//             Title = "Page 1",
//             PageOrder = 1,
//             CreatedBy = 1
//         };

//         var question = new Question
//         {
//             Page = page,
//             QuestionTypeId = questionType.Id,
//             QuestionText = "What is your name?",
//             IsRequired = true,
//             QuestionOrder = 1,
//             CreatedBy = 1
//         };

//         form.Pages.Add(page);
//         page.Questions.Add(question);

//         DbContext.Forms.Add(form);
//         await DbContext.SaveChangesAsync();

//         // Create submission with text answer
//         var submission = new Submission
//         {
//             FormId = form.Id,
//             RespondentEmail = "test@example.com",
//             RespondentName = "Test User",
//             SubmittedDate = DateTime.UtcNow,
//             TotalScore = 0,
//             CreatedBy = 1
//         };

//         var answer = new Answer
//         {
//             Submission = submission,
//             QuestionId = question.Id,
//             Score = 0,
//             CreatedBy = 1
//         };

//         var answerValue = new AnswerValue
//         {
//             Answer = answer,
//             TextValue = "John Smith",
//             CreatedBy = 1
//         };

//         submission.Answers.Add(answer);
//         answer.AnswerValues.Add(answerValue);

//         DbContext.Submissions.Add(submission);
//         await DbContext.SaveChangesAsync();

//         return form;
//     }

//     private async Task<Form> CreateFormWithRadioQuestionAndSubmission()
//     {
//         var questionType = await DbContext.MasterQuestionTypes.FirstAsync(qt => qt.TypeName == "Radio");
        
//         var form = new Form
//         {
//             Title = "Radio Survey",
//             Description = "A survey with radio questions",
//             IsPublished = true,
//             PublicKey = GeneratePublicKey(),
//             CreatedBy = 1
//         };

//         var page = new Page
//         {
//             Form = form,
//             Title = "Page 1",
//             PageOrder = 1,
//             CreatedBy = 1
//         };

//         var question = new Question
//         {
//             Page = page,
//             QuestionTypeId = questionType.Id,
//             QuestionText = "Rate our service",
//             IsRequired = true,
//             QuestionOrder = 1,
//             CreatedBy = 1
//         };

//         var option = new Option 
//         { 
//             Question = question, 
//             OptionText = "Excellent", 
//             DisplayOrder = 1, 
//             Score = 100, 
//             CreatedBy = 1 
//         };

//         form.Pages.Add(page);
//         page.Questions.Add(question);
//         question.Options.Add(option);

//         DbContext.Forms.Add(form);
//         await DbContext.SaveChangesAsync();

//         // Create submission with radio answer
//         var submission = new Submission
//         {
//             FormId = form.Id,
//             RespondentEmail = "test@example.com",
//             RespondentName = "Test User",
//             SubmittedDate = DateTime.UtcNow,
//             TotalScore = 100,
//             CreatedBy = 1
//         };

//         var answer = new Answer
//         {
//             Submission = submission,
//             QuestionId = question.Id,
//             Score = 100,
//             CreatedBy = 1
//         };

//         var answerValue = new AnswerValue
//         {
//             Answer = answer,
//             SelectedOptionId = option.Id,
//             CreatedBy = 1
//         };

//         submission.Answers.Add(answer);
//         answer.AnswerValues.Add(answerValue);

//         DbContext.Submissions.Add(submission);
//         await DbContext.SaveChangesAsync();

//         return form;
//     }

//     private async Task<Form> CreateFormWithSliderQuestionAndSubmission()
//     {
//         var questionType = await DbContext.MasterQuestionTypes.FirstAsync(qt => qt.TypeName == "Slider");
        
//         var form = new Form
//         {
//             Title = "Slider Survey",
//             Description = "A survey with slider questions",
//             IsPublished = true,
//             PublicKey = GeneratePublicKey(),
//             CreatedBy = 1
//         };

//         var page = new Page
//         {
//             Form = form,
//             Title = "Page 1",
//             PageOrder = 1,
//             CreatedBy = 1
//         };

//         var question = new Question
//         {
//             Page = page,
//             QuestionTypeId = questionType.Id,
//             QuestionText = "Rate from 1 to 10",
//             IsRequired = true,
//             QuestionOrder = 1,
//             CreatedBy = 1
//         };

//         var sliderConfig = new SliderConfig
//         {
//             Question = question,
//             MinValue = 1,
//             MaxValue = 10,
//             StepValue = 1,
//             MinLabel = "Poor",
//             MaxLabel = "Excellent",
//             CreatedBy = 1
//         };

//         form.Pages.Add(page);
//         page.Questions.Add(question);
//         question.SliderConfig = sliderConfig;

//         DbContext.Forms.Add(form);
//         await DbContext.SaveChangesAsync();

//         // Create submission with slider answer
//         var submission = new Submission
//         {
//             FormId = form.Id,
//             RespondentEmail = "test@example.com",
//             RespondentName = "Test User",
//             SubmittedDate = DateTime.UtcNow,
//             TotalScore = 75,
//             CreatedBy = 1
//         };

//         var answer = new Answer
//         {
//             Submission = submission,
//             QuestionId = question.Id,
//             Score = 75,
//             CreatedBy = 1
//         };

//         var answerValue = new AnswerValue
//         {
//             Answer = answer,
//             NumericValue = 7.5m,
//             CreatedBy = 1
//         };

//         submission.Answers.Add(answer);
//         answer.AnswerValues.Add(answerValue);

//         DbContext.Submissions.Add(submission);
//         await DbContext.SaveChangesAsync();

//         return form;
//     }

//     private async Task<Form> CreateFormWithMultiSelectQuestionAndSubmission()
//     {
//         var questionType = await DbContext.MasterQuestionTypes.FirstAsync(qt => qt.TypeName == "Multi");
        
//         var form = new Form
//         {
//             Title = "Multi-Select Survey",
//             Description = "A survey with multi-select questions",
//             IsPublished = true,
//             PublicKey = GeneratePublicKey(),
//             CreatedBy = 1
//         };

//         var page = new Page
//         {
//             Form = form,
//             Title = "Page 1",
//             PageOrder = 1,
//             CreatedBy = 1
//         };

//         var question = new Question
//         {
//             Page = page,
//             QuestionTypeId = questionType.Id,
//             QuestionText = "Select all that apply",
//             IsRequired = true,
//             QuestionOrder = 1,
//             CreatedBy = 1
//         };

//         var options = new[]
//         {
//             new Option { Question = question, OptionText = "Option A", DisplayOrder = 1, Score = 25, CreatedBy = 1 },
//             new Option { Question = question, OptionText = "Option B", DisplayOrder = 2, Score = 25, CreatedBy = 1 },
//             new Option { Question = question, OptionText = "Option C", DisplayOrder = 3, Score = 25, CreatedBy = 1 }
//         };

//         form.Pages.Add(page);
//         page.Questions.Add(question);
//         question.Options = options.ToList();

//         DbContext.Forms.Add(form);
//         await DbContext.SaveChangesAsync();

//         // Create submission with multi-select answer
//         var submission = new Submission
//         {
//             FormId = form.Id,
//             RespondentEmail = "test@example.com",
//             RespondentName = "Test User",
//             SubmittedDate = DateTime.UtcNow,
//             TotalScore = 50,
//             CreatedBy = 1
//         };

//         var answer = new Answer
//         {
//             Submission = submission,
//             QuestionId = question.Id,
//             Score = 50,
//             CreatedBy = 1
//         };

//         // Select first two options
//         var answerValues = new[]
//         {
//             new AnswerValue
//             {
//                 Answer = answer,
//                 SelectedOptionId = options[0].Id,
//                 CreatedBy = 1
//             },
//             new AnswerValue
//             {
//                 Answer = answer,
//                 SelectedOptionId = options[1].Id,
//                 CreatedBy = 1
//             }
//         };

//         submission.Answers.Add(answer);
//         answer.AnswerValues = answerValues.ToList();

//         DbContext.Submissions.Add(submission);
//         await DbContext.SaveChangesAsync();

//         return form;
//     }

//     private static string GeneratePublicKey()
//     {
//         return Guid.NewGuid().ToString("N")[..16].ToUpper();
//     }
// }
