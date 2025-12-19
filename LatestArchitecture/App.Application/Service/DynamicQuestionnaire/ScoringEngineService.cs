using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Common.Constant;
using App.Common.Models;
using App.Domain.Entities.DynamicQuestionnaire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using App.Application.Interfaces.Services.AuthenticationModule;

namespace App.Application.Service.DynamicQuestionnaire;

/// <summary>
/// Simple scoring engine service implementation for Phase 7
/// </summary>
public class ScoringEngineService : IScoringEngineService
{
    private readonly IFormRepository _formRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly ILogger<ScoringEngineService> _logger;
    private readonly ICurrentUserClaimService _currentUserClaimService;

    public ScoringEngineService(
        IFormRepository formRepository,
        ISubmissionRepository submissionRepository,
        ICurrentUserClaimService currentUserClaimService,
        ILogger<ScoringEngineService> logger)
    {
        _formRepository = formRepository;
        _submissionRepository = submissionRepository;
        _currentUserClaimService = currentUserClaimService;
        _logger = logger;
    }

    public async Task<JsonModel> CalculateSubmissionScoreAsync(int submissionId)
    {
        _logger.LogInformation("Calculating score for submission {SubmissionId}", submissionId);

        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel(responseData: null!, "User must belong to an organization", StatusCodes.Status401Unauthorized);
        }

        var submission = await _submissionRepository.GetByIdWithDetailsAsync(submissionId, organizationId.Value);
        if (submission is null)
        {
            return new JsonModel(new { }, StatusMessage.SubmissionNotFound, StatusCodes.Status404NotFound);
        }

        var totalScore = CalculateScoreForSubmission(submission);

        // Update the submission with the calculated score
        submission.TotalScore = totalScore;
        await _submissionRepository.UpdateAsync(submission);

        _logger.LogInformation("Score calculated and updated for submission {SubmissionId}: {TotalScore}",
            submissionId, totalScore);

        return new JsonModel(new { SubmissionId = submissionId, TotalScore = totalScore },
            StatusMessage.ScoreCalculationSuccess, StatusCodes.Status200OK);

    }

    public async Task<JsonModel> RecalculateFormScoresAsync(int formId)
    {
        _logger.LogInformation("Recalculating scores for form {FormId}", formId);
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel(responseData: null!, "User must belong to an organization", StatusCodes.Status401Unauthorized);
        }

        var formExists = await _formRepository.ExistsAsync(formId);
        if (!formExists)
        {
            return new JsonModel(new { }, StatusMessage.FormNotFound, StatusCodes.Status404NotFound);
        }

        // Execute stored procedure to recalculate scores
        var updatedCount = await _submissionRepository.RecalculateFormScoresAsync(formId, organizationId.Value);

        _logger.LogInformation("Recalculated scores for {UpdatedCount} submissions in form {FormId}",
            updatedCount, formId);

        return new JsonModel(new { FormId = formId, UpdatedSubmissions = updatedCount },
            StatusMessage.ScoreReCalculationSuccess, StatusCodes.Status200OK);
    }

    public async Task<JsonModel> GetFormScoringAnalyticsAsync(int formId)
    {
        _logger.LogInformation("Getting scoring analytics for form {FormId}", formId);
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel(responseData: null!, "User must belong to an organization", StatusCodes.Status401Unauthorized);
        }

        var form = await _formRepository.GetByIdWithDetailsAsync(formId, organizationId.Value);
        if (form is null)
        {
            return new JsonModel(new { }, StatusMessage.FormNotFound, StatusCodes.Status404NotFound);
        }

        var submissions = await _submissionRepository.GetByFormIdAsync(formId, organizationId.Value);
        if (!submissions.Any())
        {
            return new JsonModel(new ScoringAnalyticsDto
            {
                FormId = formId,
                FormTitle = form.Title,
                TotalSubmissions = Number.Zero
            }, StatusMessage.NoSubmissions, StatusCodes.Status200OK);
        }

        var scores = submissions.Select(s => s.TotalScore).OrderBy(s => s).ToList();
        var maxPossible = await CalculateMaxPossibleScore(formId, organizationId.Value);

        var analytics = new ScoringAnalyticsDto
        {
            FormId = formId,
            FormTitle = form.Title,
            TotalSubmissions = submissions.Count(),
            AverageScore = scores.Average(),
            HighestScore = scores.Max(),
            LowestScore = scores.Min(),
            MedianScore = CalculateMedian(scores),
            MaxPossibleScore = maxPossible,
            MinPossibleScore = Number.Zero,
            StandardDeviation = CalculateStandardDeviation(scores),
            ScoreRanges = CalculateScoreRanges(scores, maxPossible),
            LastUpdated = DateTime.UtcNow
        };

        return new JsonModel(analytics, StatusMessage.RetrievedSuccessfully, StatusCodes.Status200OK);
    }

    public async Task<JsonModel> GetSubmissionScoreBreakdownAsync(int submissionId)
    {
        _logger.LogInformation("Getting score breakdown for submission {SubmissionId}", submissionId);
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel(responseData: null!, "User must belong to an organization", StatusCodes.Status401Unauthorized);
        }

        var submission = await _submissionRepository.GetByIdWithDetailsAsync(submissionId, organizationId.Value);
        if (submission is null)
        {
            return new JsonModel(new { }, StatusMessage.SubmissionNotFound, StatusCodes.Status404NotFound);
        }

        var form = await _formRepository.GetByIdWithDetailsAsync(submission.FormId, organizationId.Value);
        var maxPossible = await CalculateMaxPossibleScore(submission.FormId, organizationId.Value);

        var breakdown = new SubmissionScoreBreakdownDto
        {
            SubmissionId = submissionId,
            RespondentEmail = submission.RespondentEmail,
            TotalScore = submission.TotalScore,
            MaxPossibleScore = maxPossible,
            ScorePercentage = maxPossible > Number.Zero ? (submission.TotalScore / maxPossible) * Number.Hundred : Number.Zero,
            SubmittedAt = submission.SubmittedDate,
            QuestionScores = GetQuestionScores(submission, form!)
        };

        return new JsonModel(breakdown, StatusMessage.BreakdownRetrievedSuccessfully, StatusCodes.Status200OK);

    }

    public async Task<JsonModel> UpdateFormScoringAsync(UpdateFormScoringRequest request, int userId)
    {
        _logger.LogInformation("Updating scoring for form {FormId}", request.FormId);
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel(responseData: null!, "User must belong to an organization", StatusCodes.Status401Unauthorized);
        }

        var form = await _formRepository.GetByIdWithDetailsAsync(request.FormId, organizationId.Value);
        if (form is null)
        {
            return new JsonModel(new { }, StatusMessage.FormNotFound, StatusCodes.Status404NotFound);
        }

        // Update option scores
        foreach (var optionScore in request.OptionScores)
        {
            var option = form.Pages
                .SelectMany(p => p.Questions)
                .SelectMany(q => q.Options)
                .FirstOrDefault(o => o.Id == optionScore.OptionId);

            if (option is not null)
            {
                option.Score = optionScore.Score;
                option.UpdatedBy = userId;
                option.UpdatedAt = DateTime.UtcNow;
            }
        }

        // Update matrix column scores
        foreach (var matrixScore in request.MatrixColumnScores)
        {
            var matrixColumn = form.Pages
                .SelectMany(p => p.Questions)
                .SelectMany(q => q.MatrixColumns)
                .FirstOrDefault(mc => mc.Id == matrixScore.MatrixColumnId);

            if (matrixColumn is not null)
            {
                matrixColumn.Score = matrixScore.Score;
                matrixColumn.UpdatedBy = userId;
                matrixColumn.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _formRepository.UpdateAsync(form);

        _logger.LogInformation("Scoring updated for form {FormId}", request.FormId);

        return new JsonModel(new { FormId = request.FormId },
            StatusMessage.ScoringUpdatedSuccessfully, StatusCodes.Status200OK);

    }

    public async Task<JsonModel> GetScoreDistributionAsync(int formId)
    {
        _logger.LogInformation("Getting score distribution for form {FormId}", formId);
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel(responseData: null!, "User must belong to an organization", StatusCodes.Status401Unauthorized);
        }

        var formExists = await _formRepository.ExistsAsync(formId);
        if (!formExists)
        {
            return new JsonModel(new { }, StatusMessage.FormNotFound, StatusCodes.Status404NotFound);
        }

        var submissions = await _submissionRepository.GetByFormIdAsync(formId, organizationId.Value);
        if (!submissions.Any())
        {
            return new JsonModel(new ScoreDistributionDto
            {
                FormId = formId,
                DistributionPoints = new List<ScoreDistributionPoint>(),
                Statistics = new ScoreStatistics()
            }, StatusMessage.NoDistributionSubmissionsFound, StatusCodes.Status200OK);
        }

        var scores = submissions.Select(s => s.TotalScore).OrderBy(s => s).ToList();
        var distribution = CalculateDistribution(scores);
        var statistics = CalculateStatistics(scores);

        var distributionDto = new ScoreDistributionDto
        {
            FormId = formId,
            DistributionPoints = distribution,
            Statistics = statistics
        };

        return new JsonModel(distributionDto, StatusMessage.ScoreDistributionRetrieved, StatusCodes.Status200OK);

    }

    public async Task<JsonModel> GetTopPerformersAsync(int formId, int topCount = Number.Ten)
    {
        _logger.LogInformation("Getting top {TopCount} performers for form {FormId}", topCount, formId);
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel(responseData: null!, "User must belong to an organization", StatusCodes.Status401Unauthorized);
        }

        var formExists = await _formRepository.ExistsAsync(formId);
        if (!formExists)
        {
            return new JsonModel(new { }, StatusMessage.FormNotFound, StatusCodes.Status404NotFound);
        }

        var submissions = await _submissionRepository.GetByFormIdAsync(formId, organizationId.Value);
        var maxPossible = await CalculateMaxPossibleScore(formId, organizationId.Value);

        var topPerformers = submissions
            .OrderByDescending(s => s.TotalScore)
            .ThenBy(s => s.SubmittedDate)
            .Take(topCount)
            .Select((s, index) => new TopPerformerDto
            {
                SubmissionId = s.Id,
                RespondentEmail = s.RespondentEmail,
                TotalScore = s.TotalScore,
                ScorePercentage = maxPossible > Number.Zero ? (s.TotalScore / maxPossible) * Number.Hundred : Number.Zero,
                Rank = index + Number.One,
                SubmittedAt = s.SubmittedDate,
                TimeTaken = null
            })
            .ToList();

        return new JsonModel(topPerformers, StatusMessage.PerformerRetrievedSuccessfully, StatusCodes.Status200OK);
    }

    public async Task<JsonModel> ExportScoringReportAsync(int formId, string format = "CSV")
    {
        _logger.LogInformation("Exporting scoring report for form {FormId} in {Format} format", formId, format);
        var organizationId = _currentUserClaimService.OrganizationId;
        if (!organizationId.HasValue)
        {
            return new JsonModel(responseData: null!, "User must belong to an organization", StatusCodes.Status401Unauthorized);
        }

        var form = await _formRepository.GetByIdWithDetailsAsync(formId, organizationId.Value);
        if (form is null)
        {
            return new JsonModel(new { }, StatusMessage.FormNotFound, StatusCodes.Status404NotFound);
        }

        var submissions = await _submissionRepository.GetByFormIdAsync(formId, organizationId.Value);
        if (!submissions.Any())
        {
            return new JsonModel(new { }, StatusMessage.ExportSubmissionError, StatusCodes.Status404NotFound);
        }

        var maxPossible = await CalculateMaxPossibleScore(formId, organizationId.Value);
        var reportData = submissions.Select(s => new
        {
            s.Id,
            s.RespondentEmail,
            s.TotalScore,
            s.SubmittedDate,
            ScorePercentage = maxPossible > Number.Zero ? (s.TotalScore / maxPossible) * Number.Hundred : Number.Zero
        }).ToList();

        return new JsonModel(new
        {
            FormTitle = form.Title,
            ExportFormat = format,
            Data = reportData,
            ExportedAt = DateTime.UtcNow
        }, StatusMessage.ReportDataExport, StatusCodes.Status200OK);
    }

    #region Private Helper Methods

    private static decimal CalculateScoreForSubmission(Submission submission)
    {
        decimal totalScore = Number.Zero;

        foreach (var answer in submission.Answers)
        {
            var question = answer.Question;
            if (question is null)
            {
                answer.Score = Number.Zero;
                continue;
            }

            decimal answerScore = Number.Zero;

            // Calculate score based on question type
            foreach (var answerValue in answer.AnswerValues)
            {
                switch (question.QuestionType?.TypeName)
                {
                    case "Slider":
                        // For slider questions, use numeric value directly as score
                        if (answerValue.NumericValue.HasValue)
                        {
                            answerScore += answerValue.NumericValue.Value;
                        }
                        break;

                    case "Radio":
                    case "Dropdown":
                        // For radio/dropdown, use the selected option's score
                        if (answerValue.SelectedOptionId.HasValue)
                        {
                            var selectedOption = question.Options?.FirstOrDefault(o => o.Id == answerValue.SelectedOptionId.Value);
                            if (selectedOption is not null)
                            {
                                answerScore += selectedOption.Score;
                            }
                        }
                        break;

                    case "Multi":
                        // For multi-select, sum all selected option scores
                        if (answerValue.SelectedOptionId.HasValue)
                        {
                            var selectedOption = question.Options?.FirstOrDefault(o => o.Id == answerValue.SelectedOptionId.Value);
                            if (selectedOption is not null)
                            {
                                answerScore += selectedOption.Score;
                            }
                        }
                        break;

                    case "Matrix":
                        // For matrix questions, use the selected column's score
                        if (answerValue.SelectedMatrixColumnId.HasValue)
                        {
                            var selectedColumn = question.MatrixColumns?.FirstOrDefault(c => c.Id == answerValue.SelectedMatrixColumnId.Value);
                            if (selectedColumn is not null)
                            {
                                answerScore += selectedColumn.Score;
                            }
                        }
                        break;

                    case "Text":
                    case "TextArea":
                    case "Date":
                    default:
                        // For text questions or unknown types, score remains 0
                        break;
                }
            }

            // Update the answer's score
            answer.Score = answerScore;
            totalScore += answerScore;
        }

        return totalScore;
    }

    private async Task<decimal> CalculateMaxPossibleScore(int formId, int organizationId)
    {
        var form = await _formRepository.GetByIdWithDetailsAsync(formId, organizationId);
        if (form is null) return Number.Zero;

        decimal maxScore = Number.Zero;

        foreach (var page in form.Pages)
        {
            foreach (var question in page.Questions)
            {
                var questionType = question.QuestionType?.TypeName;

                switch (questionType)
                {
                    case "Radio":
                    case "Dropdown":
                        // For single-select questions, max is the highest option score
                        if (question.Options?.Any() ?? false)
                        {
                            maxScore += question.Options.Max(o => o.Score);
                        }
                        break;

                    case "Multi":
                        // For multi-select questions, max is sum of all positive option scores
                        if (question.Options?.Any() ?? false)
                        {
                            maxScore += question.Options.Where(o => o.Score > Number.Zero).Sum(o => o.Score);
                        }
                        break;

                    case "Matrix":
                        // For matrix questions, max is number of rows * highest column score
                        if ((question.MatrixRows?.Any() ?? false) && (question.MatrixColumns?.Any() ?? false))
                        {
                            var maxColumnScore = question.MatrixColumns!.Max(c => c.Score);
                            maxScore += question.MatrixRows!.Count * maxColumnScore;
                        }
                        break;

                    case "Slider":
                        // For slider questions, use the actual max value from SliderConfig
                        if (question.SliderConfig is not null)
                        {
                            maxScore += question.SliderConfig.MaxValue;
                        }
                        else
                        {
                            maxScore += Number.Hundred; // fallback to 100 if no config found
                        }
                        break;

                    case "Text":
                    case "TextArea":
                    case "Date":
                    default:
                        // Text questions typically don't contribute to score
                        break;
                }
            }
        }

        return maxScore;
    }

    private static List<QuestionScoreDto> GetQuestionScores(Submission submission, Form form)
    {
        var questionScores = new List<QuestionScoreDto>();

        foreach (var page in form.Pages)
        {
            foreach (var question in page.Questions)
            {
                var answer = submission.Answers.FirstOrDefault(a => a.QuestionId == question.Id);

                // Calculate the actual score earned for this question from answer values
                decimal scoreEarned = Number.Zero;
                if (answer?.AnswerValues?.Any() ?? false)
                {
                    scoreEarned = CalculateQuestionScore(answer, question);
                }

                var questionScore = new QuestionScoreDto
                {
                    QuestionId = question.Id,
                    QuestionText = question.QuestionText,
                    QuestionType = question.QuestionType?.TypeName ?? "Unknown",
                    ScoreEarned = scoreEarned,
                    MaxPossibleScore = CalculateQuestionMaxScore(question),
                    SelectedOptions = GetSelectedOptionScores(answer, question),
                    MatrixAnswers = GetMatrixAnswerScores(answer, question)
                };

                questionScores.Add(questionScore);
            }
        }

        return questionScores;
    }

    private static decimal CalculateQuestionMaxScore(Question question)
    {
        var questionType = question.QuestionType?.TypeName;

        switch (questionType)
        {
            case "Radio":
            case "Dropdown":
                // For single-select questions, max is the highest option score
                if (question.Options?.Any() ?? false)
                {
                    return question.Options.Max(o => o.Score);
                }
                break;

            case "Multi":
                // For multi-select questions, max is sum of all positive option scores
                if (question.Options?.Any() ?? false)
                {
                    return question.Options.Where(o => o.Score > 0).Sum(o => o.Score);
                }
                break;

            case "Matrix":
                // For matrix questions, max is number of rows * highest column score
                if ((question.MatrixRows?.Any() ?? false) && (question.MatrixColumns?.Any() ?? false))
                {
                    var maxColumnScore = question.MatrixColumns!.Max(c => c.Score);
                    return question.MatrixRows!.Count * maxColumnScore;
                }
                break;

            case "Slider":
                // For slider questions, use the actual max value from SliderConfig
                if (question.SliderConfig is not null)
                {
                    return question.SliderConfig.MaxValue;
                }
                return 100; // fallback to 100 if no config found

            case "Text":
            case "TextArea":
            case "Date":
            default:
                // Text questions typically don't contribute to score
                break;
        }

        return 0;
    }

    private static decimal CalculateQuestionScore(Answer answer, Question question)
    {
        decimal questionScore = Number.Zero;

        foreach (var answerValue in answer.AnswerValues)
        {
            switch (question.QuestionType?.TypeName)
            {
                case "Slider":
                    if (answerValue.NumericValue.HasValue)
                    {
                        questionScore += answerValue.NumericValue.Value;
                    }
                    break;

                case "Radio":
                case "Dropdown":
                    if (answerValue.SelectedOptionId.HasValue)
                    {
                        var selectedOption = question.Options?.FirstOrDefault(o => o.Id == answerValue.SelectedOptionId.Value);
                        if (selectedOption is not null)
                        {
                            questionScore += selectedOption.Score;
                        }
                    }
                    break;

                case "Multi":
                    if (answerValue.SelectedOptionId.HasValue)
                    {
                        var selectedOption = question.Options?.FirstOrDefault(o => o.Id == answerValue.SelectedOptionId.Value);
                        if (selectedOption is not null)
                        {
                            questionScore += selectedOption.Score;
                        }
                    }
                    break;

                case "Matrix":
                    if (answerValue.SelectedMatrixColumnId.HasValue)
                    {
                        var selectedColumn = question.MatrixColumns?.FirstOrDefault(c => c.Id == answerValue.SelectedMatrixColumnId.Value);
                        if (selectedColumn is not null)
                        {
                            questionScore += selectedColumn.Score;
                        }
                    }
                    break;

                case "Text":
                case "TextArea":
                case "Date":
                default:
                    // For text questions or unknown types, score remains 0
                    break;
            }
        }

        return questionScore;
    }

    private static List<SelectedOptionScoreDto> GetSelectedOptionScores(Answer? answer, Question question)
    {
        if (!(answer?.AnswerValues?.Any() ?? false))
            return new List<SelectedOptionScoreDto>();

        var selectedOptionIds = answer.AnswerValues
            .Where(av => av.SelectedOptionId.HasValue)
            .Select(av => av.SelectedOptionId!.Value)
            .ToList();

        return question.Options?
            .Where(o => selectedOptionIds.Contains(o.Id))
            .Select(o => new SelectedOptionScoreDto
            {
                OptionId = o.Id,
                OptionText = o.OptionText ?? string.Empty,
                Score = o.Score
            })
            .ToList() ?? new List<SelectedOptionScoreDto>();
    }

    private static List<MatrixAnswerScoreDto> GetMatrixAnswerScores(Answer? answer, Question question)
    {
        if (!(answer?.AnswerValues?.Any() ?? false))
            return new List<MatrixAnswerScoreDto>();

        var matrixScores = new List<MatrixAnswerScoreDto>();

        var matrixAnswerValues = answer.AnswerValues
            .Where(av => av.MatrixRowId.HasValue && av.SelectedMatrixColumnId.HasValue)
            .ToList();

        foreach (var matrixAnswer in matrixAnswerValues)
        {
            var row = question.MatrixRows?.FirstOrDefault(r => r.Id == matrixAnswer.MatrixRowId);
            var column = question.MatrixColumns?.FirstOrDefault(c => c.Id == matrixAnswer.SelectedMatrixColumnId);

            if (row is not null && column is not null)
            {
                matrixScores.Add(new MatrixAnswerScoreDto
                {
                    MatrixRowId = row.Id,
                    RowLabel = row.RowLabel,
                    SelectedColumnId = column.Id,
                    ColumnLabel = column.ColumnLabel,
                    Score = column.Score
                });
            }
        }

        return matrixScores;
    }

    private static decimal CalculateMedian(List<decimal> scores)
    {
        var sortedScores = scores.OrderBy(s => s).ToList();
        int count = sortedScores.Count;

        if (count % Number.Two == Number.Zero)
        {
            return (sortedScores[count / Number.Two - Number.One] + sortedScores[count / Number.Two]) / Number.Two;
        }
        else
        {
            return sortedScores[count / Number.Two];
        }
    }

    private static decimal CalculateStandardDeviation(List<decimal> scores)
    {
        if (scores.Count <= Number.One) return Number.Zero;

        var mean = scores.Average();
        var sumOfSquaredDifferences = scores.Sum(s => Math.Pow((double)(s - mean), Number.Two));
        var variance = sumOfSquaredDifferences / (scores.Count - Number.One);
        return (decimal)Math.Sqrt(variance);
    }

    private static List<ScoreRangeDto> CalculateScoreRanges(List<decimal> scores, decimal maxPossible)
    {
        var ranges = new List<ScoreRangeDto>();
        if (maxPossible <= Number.Zero) return ranges;

        for (int i = Number.Zero; i < Number.Five; i++)
        {
            var minPercent = i * Number.Twenty;
            var maxPercent = (i + Number.One) * Number.Twenty;
            var minScore = (maxPossible * minPercent) / Number.Hundred;
            var maxScore = (maxPossible * maxPercent) / Number.Hundred;

            var count = scores.Count(s => s >= minScore && s <= maxScore);
            var percentage = scores.Count > Number.Zero ? (decimal)count / scores.Count * Number.Hundred : Number.Zero;

            ranges.Add(new ScoreRangeDto
            {
                RangeLabel = $"{minPercent}-{maxPercent}%",
                MinScore = minScore,
                MaxScore = maxScore,
                Count = count,
                Percentage = percentage
            });
        }

        return ranges;
    }

    private static List<ScoreDistributionPoint> CalculateDistribution(List<decimal> scores)
    {
        var distribution = new List<ScoreDistributionPoint>();
        var scoreGroups = scores.GroupBy(s => s).OrderBy(g => g.Key);
        var totalCount = scores.Count;
        var cumulativeCount = Number.Zero;

        foreach (var group in scoreGroups)
        {
            cumulativeCount += group.Count();
            distribution.Add(new ScoreDistributionPoint
            {
                Score = group.Key,
                Count = group.Count(),
                CumulativePercentage = (decimal)cumulativeCount / totalCount * Number.Hundred
            });
        }

        return distribution;
    }

    private static ScoreStatistics CalculateStatistics(List<decimal> scores)
    {
        if (!scores.Any())
        {
            return new ScoreStatistics();
        }

        var mean = scores.Average();
        var median = CalculateMedian(scores);
        var mode = scores.GroupBy(s => s).OrderByDescending(g => g.Count()).First().Key;
        var variance = scores.Sum(s => Math.Pow((double)(s - mean), Number.Two)) / scores.Count;
        var stdDev = (decimal)Math.Sqrt(variance);

        var skewness = scores.Sum(s => Math.Pow((double)(s - mean), Number.Three)) / (scores.Count * Math.Pow((double)stdDev, Number.Three));
        var kurtosis = scores.Sum(s => Math.Pow((double)(s - mean), Number.Four)) / (scores.Count * Math.Pow((double)stdDev, Number.Four)) - Number.Three;

        return new ScoreStatistics
        {
            Mean = mean,
            Median = median,
            Mode = mode,
            StandardDeviation = stdDev,
            Variance = (decimal)variance,
            Skewness = (decimal)skewness,
            Kurtosis = (decimal)kurtosis
        };
    }

    #endregion
}
