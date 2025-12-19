using App.Application.Dto.DynamicQuestionnaire;
using App.Application.Interfaces.Services.DynamicQuestionnaire;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.DynamicQuestionnaire;

[ApiController]
[Route("api/v1/public")]
public class PublicFormController : BaseController
{
    private const string UnexpectedErrorMessage = "An unexpected error occurred";

    private readonly IDynamicQuestionnaireService _dynamicQuestionnaireService;

    public PublicFormController(
        IDynamicQuestionnaireService dynamicQuestionnaireService)
    {
        _dynamicQuestionnaireService = dynamicQuestionnaireService;
    }

    /// <summary>
    /// Get a published form by its public key for respondents
    /// </summary>
    /// <param name="publicKey">The public key of the form</param>
    /// <returns>Public form structure without admin metadata</returns>
    [HttpGet("forms/{publicKey}")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPublicForm(string publicKey)
    {
        try
        {
            var result = await _dynamicQuestionnaireService.GetPublicFormAsync(publicKey);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new JsonModel(responseData: null!, UnexpectedErrorMessage, StatusCodes.Status500InternalServerError, ex.Message));
        }
    }

    /// <summary>
    /// Submit a response to a published form
    /// </summary>
    /// <param name="publicKey">The public key of the form</param>
    /// <param name="request">The form submission data</param>
    /// <returns>Submission confirmation</returns>
    [HttpPost("forms/{publicKey}/submit")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitFormResponse(string publicKey, [FromBody] SubmitFormRequest request)
    {
        // Basic validation
        if (request == null)
        {
            return BadRequest(new JsonModel(responseData: null!, StatusMessage.RequestBodyRequired, StatusCodes.Status400BadRequest));
        }

        if (string.IsNullOrWhiteSpace(request.RespondentEmail))
        {
            return BadRequest(new JsonModel(responseData: null!, StatusMessage.RespondentEmailIsRequired, StatusCodes.Status400BadRequest));
        }

        var result = await _dynamicQuestionnaireService.SubmitFormResponseAsync(publicKey, request);

        return result.StatusCode switch
        {
            StatusCodes.Status200OK => Ok(result),
            StatusCodes.Status400BadRequest => BadRequest(result),
            StatusCodes.Status404NotFound => NotFound(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }
}
