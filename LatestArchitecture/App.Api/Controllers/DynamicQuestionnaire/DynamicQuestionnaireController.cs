using App.Application.Dto.Common;
using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.DynamicQuestionnaire;

[ApiController]
[Route("api/v1/[controller]")]
public class DynamicQuestionnaireController : BaseController
{    
    private readonly IDynamicQuestionnaireService _dynamicQuestionnaireService;
    private readonly IScoringEngineService _scoringEngineService;
    private readonly ICurrentUserClaimService _currentUserClaimService;

    public DynamicQuestionnaireController(
        IDynamicQuestionnaireService dynamicQuestionnaireService,
        IScoringEngineService scoringEngineService,
        ICurrentUserClaimService currentUserClaimService)
    {
        _dynamicQuestionnaireService = dynamicQuestionnaireService;
        _scoringEngineService = scoringEngineService;
        _currentUserClaimService = currentUserClaimService;
    }

    /// <summary>
    /// Create a new form with pages, questions, and options
    /// </summary>
    /// <param name="createFormDto">Form creation data</param>
    /// <returns>Created form details</returns>
    [HttpPost("forms")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateForm([FromBody] CreateFormDto createFormDto)
    {
        // Get Current User ID
        int userId = _currentUserClaimService.UserId ?? Number.Zero;

        var result = await _dynamicQuestionnaireService.CreateFormAsync(createFormDto, userId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status201Created => CreatedAtAction(nameof(GetFormById), new { id = ((FormResponseDto)result.Data!).FormId }, result),
            StatusCodes.Status400BadRequest => BadRequest(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Update an existing form with pages, questions, and options
    /// </summary>
    /// <param name="id">Form ID to update</param>
    /// <param name="updateFormDto">Form update data</param>
    /// <returns>Updated form details</returns>
    [HttpPut("forms/{id:int}")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateForm(int id, [FromBody] UpdateFormDto updateFormDto)
    {
        // Get Current User ID
        int userId = _currentUserClaimService.UserId ?? Number.Zero;

        var result = await _dynamicQuestionnaireService.UpdateFormAsync(id, updateFormDto, userId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status400BadRequest => BadRequest(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Get form by ID with all details
    /// </summary>
    /// <param name="id">Form ID</param>
    /// <returns>Form details</returns>
    [HttpGet("forms/{id:int}")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFormById(int id)
    {
        var result = await _dynamicQuestionnaireService.GetFormByIdAsync(id);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Get all forms
    /// </summary>
    /// <returns>List of all forms</returns>
    [HttpGet("forms")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllForms()
    {
        var result = await _dynamicQuestionnaireService.GetAllFormsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get forms with advanced pagination, sorting, and search using stored procedure
    /// </summary>
    /// <param name="filter">Filter parameters for pagination, sorting, and search</param>
    /// <returns>Paginated list of forms with metadata</returns>
    [HttpPost("forms/paged")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPagedForms([FromBody] FormFilterDto filter)
    {
        var result = await _dynamicQuestionnaireService.GetPagedFormsAsync(filter);
        return Ok(result);
    }

    /// <summary>
    /// Get all master question types
    /// </summary>
    /// <returns>List of question types</returns>
    [HttpGet("question-types")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMasterQuestionTypes()
    {
        var result = await _dynamicQuestionnaireService.GetMasterQuestionTypesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Publish a form to make it publicly accessible
    /// </summary>
    /// <param name="id">Form ID to publish</param>
    /// <returns>Public key and URL for the published form</returns>
    [HttpPut("forms/{id:int}/publish")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PublishForm(int id)
    {
        // Get Current User ID
        int userId = _currentUserClaimService.UserId ?? Number.Zero;

        var result = await _dynamicQuestionnaireService.PublishFormAsync(id, userId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status400BadRequest => BadRequest(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Delete a form (soft delete)
    /// </summary>
    /// <param name="id">Form ID to delete</param>
    /// <returns>Deletion confirmation</returns>
    [HttpDelete("forms/{id:int}")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteForm(int id)
    {
        // Get Current User ID
        int userId = _currentUserClaimService.UserId ?? Number.Zero;

        var result = await _dynamicQuestionnaireService.DeleteFormAsync(id, userId);

        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }


    /// <summary>
    /// Get paginated submissions for a specific form
    /// </summary>
    /// <param name="formId">Form ID to get submissions for</param>
    /// <param name="filter">Filter parameters for pagination, sorting, and search</param>
    /// <returns>Paginated list of form submissions</returns>
    [HttpPost("forms/{formId:int}/submissions")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFormSubmissions(int formId, [FromBody] ResponseFilterDto filter)
    {
        // Basic validation
        if (filter == null)
        {
            return BadRequest(new JsonModel(responseData: null!, StatusMessage.FilterParametersAreRequired, StatusCodes.Status400BadRequest));
        }

        if (filter.PageNumber < 1 || filter.PageSize < 1)
        {
            return BadRequest(new JsonModel(responseData: null!, StatusMessage.PageNumberAndPageSizeMustBeGreaterThan_0, StatusCodes.Status400BadRequest));
        }

        var result = await _dynamicQuestionnaireService.GetFormSubmissionsAsync(formId, filter);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Get detailed submission information by submission ID
    /// </summary>
    /// <param name="submissionId">Submission ID</param>
    /// <returns>Detailed submission information</returns>
    [HttpGet("submissions/{submissionId:int}")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSubmissionById(int submissionId)
    {
        var result = await _dynamicQuestionnaireService.GetSubmissionByIdAsync(submissionId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    #region Conditional Logic Rules

    /// <summary>
    /// Create a new conditional logic rule for a form
    /// </summary>
    /// <param name="request">Rule creation data</param>
    /// <returns>Created rule details</returns>
    [HttpPost("rules")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRule([FromBody] CreateRuleRequest request)
    {
        // Get Current User ID
        int userId = _currentUserClaimService.UserId ?? Number.Zero;

        var result = await _dynamicQuestionnaireService.CreateRuleAsync(request, userId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status201Created => CreatedAtAction(nameof(GetRuleById), new { ruleId = ((RuleResponseDto)result.Data!).RuleId }, result),
            StatusCodes.Status400BadRequest => BadRequest(result),
            StatusCodes.Status404NotFound => NotFound(result),
            StatusCodes.Status409Conflict => Conflict(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Update an existing conditional logic rule
    /// </summary>
    /// <param name="request">Rule update data</param>
    /// <returns>Updated rule details</returns>
    [HttpPut("rules")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRule([FromBody] UpdateRuleRequest request)
    {
        // Get Current User ID
        int userId = _currentUserClaimService.UserId ?? Number.Zero;

        var result = await _dynamicQuestionnaireService.UpdateRuleAsync(request, userId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status400BadRequest => BadRequest(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Delete a conditional logic rule
    /// </summary>
    /// <param name="ruleId">Rule ID to delete</param>
    /// <returns>Deletion confirmation</returns>
    [HttpDelete("rules/{ruleId:int}")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRule(int ruleId)
    {
        // Get Current User ID
        int userId = _currentUserClaimService.UserId ?? Number.Zero;

        var result = await _dynamicQuestionnaireService.DeleteRuleAsync(ruleId, userId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Get all rules for a specific form
    /// </summary>
    /// <param name="formId">Form ID</param>
    /// <returns>List of rules for the form</returns>
    [HttpGet("forms/{formId:int}/rules")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFormRules(int formId)
    {
        var result = await _dynamicQuestionnaireService.GetFormRulesAsync(formId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Get a specific rule by ID
    /// </summary>
    /// <param name="ruleId">Rule ID</param>
    /// <returns>Rule details</returns>
    [HttpGet("rules/{ruleId:int}")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRuleById(int ruleId)
    {
        var result = await _dynamicQuestionnaireService.GetRuleByIdAsync(ruleId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Evaluate rules for a form based on current answers
    /// </summary>
    /// <param name="request">Rule evaluation data with current answers</param>
    /// <returns>Rule evaluation result indicating which questions to show/hide</returns>
    [HttpPost("rules/evaluate")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EvaluateRules([FromBody] RuleEvaluationRequest request)
    {
        var result = await _dynamicQuestionnaireService.EvaluateRulesAsync(request);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status400BadRequest => BadRequest(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    #endregion

    #region Scoring Engine

    /// <summary>
    /// Calculate score for a specific submission
    /// </summary>
    /// <param name="submissionId">Submission ID</param>
    /// <returns>Calculated score</returns>
    [HttpPost("submissions/{submissionId:int}/calculate-score")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CalculateSubmissionScore(int submissionId)
    {
        var result = await _scoringEngineService.CalculateSubmissionScoreAsync(submissionId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Get scoring analytics for a form
    /// </summary>
    /// <param name="formId">Form ID</param>
    /// <returns>Scoring analytics</returns>
    [HttpGet("forms/{formId:int}/scoring/analytics")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFormScoringAnalytics(int formId)
    {
        var result = await _scoringEngineService.GetFormScoringAnalyticsAsync(formId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Recalculate scores for all submissions in a form
    /// </summary>
    /// <param name="formId">Form ID</param>
    /// <returns>Number of submissions updated</returns>
    [HttpPost("forms/{formId:int}/recalculate-scores")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RecalculateFormScores(int formId)
    {
        var result = await _scoringEngineService.RecalculateFormScoresAsync(formId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Get detailed score breakdown for a submission
    /// </summary>
    /// <param name="submissionId">Submission ID</param>
    /// <returns>Detailed score breakdown</returns>
    [HttpGet("submissions/{submissionId:int}/score-breakdown")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSubmissionScoreBreakdown(int submissionId)
    {
        var result = await _scoringEngineService.GetSubmissionScoreBreakdownAsync(submissionId);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Export scoring report in various formats
    /// </summary>
    /// <param name="formId">Form ID</param>
    /// <param name="format">Export format (CSV, Excel, PDF)</param>
    /// <returns>Export file data</returns>
    [HttpGet("forms/{formId:int}/scoring/export")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportScoringReport(int formId, string format = "CSV")
    {
        var result = await _scoringEngineService.ExportScoringReportAsync(formId, format);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    #endregion

    #region Test Mode APIs for Preview

    /// <summary>
    /// Submit a test mode form for rule evaluation and scoring without saving to database
    /// </summary>
    /// <param name="request">Test mode submission data</param>
    /// <returns>Rule evaluation results and calculated scores</returns>
    [HttpPost("test-mode/submit")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitTestMode([FromBody] TestModeSubmissionRequest request)
    {
        var result = await _dynamicQuestionnaireService.SubmitTestModeAsync(request);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status400BadRequest => BadRequest(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Evaluate rules for a test mode submission
    /// </summary>
    /// <param name="request">Test mode submission data for rule evaluation</param>
    /// <returns>Rule evaluation results</returns>
    [HttpPost("test-mode/evaluate-rules")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EvaluateTestModeRules([FromBody] TestModeSubmissionRequest request)
    {
        var result = await _dynamicQuestionnaireService.EvaluateTestModeRulesAsync(request);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status400BadRequest => BadRequest(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    /// <summary>
    /// Calculate score for a test mode submission
    /// </summary>
    /// <param name="request">Test mode submission data for scoring</param>
    /// <returns>Calculated score results</returns>
    [HttpPost("test-mode/calculate-score")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CalculateTestModeScore([FromBody] TestModeSubmissionRequest request)
    {
        var result = await _dynamicQuestionnaireService.CalculateTestModeScoreAsync(request);
        
        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status400BadRequest => BadRequest(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    #endregion
}
