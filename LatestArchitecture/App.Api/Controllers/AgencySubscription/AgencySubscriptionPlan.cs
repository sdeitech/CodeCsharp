using App.Application.Interfaces.Services.AgencySubscriptions;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.Api.Controllers.AgencySubscription
{
    /// <summary>
    /// Controller for managing agency subscription plans.
    /// Provides endpoints to get, buy, and cancel subscription plans for agencies.
    /// </summary>
    [Route("api/agency/[controller]")]
    [ApiController]
    public class AgencySubscriptionPlan : BaseController
    {


        private readonly IAgencySubscriptionPlanService _agencySubscriptionPlanService;

        public AgencySubscriptionPlan(IAgencySubscriptionPlanService agencySubscriptionPlanService)
        {
            _agencySubscriptionPlanService = agencySubscriptionPlanService;
        }
        /// <summary>
        /// Gets the current subscription plan for the specified agency.
        /// </summary>
        /// <param name="id">The agency identifier.</param>
        /// <returns>The current subscription plan details.</returns>
        [HttpGet("current-plan")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentSubscriptionPlan(int id)
        {
            var result = await _agencySubscriptionPlanService.GetCurrentSubscriptionPlan(id);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }


        /// <summary>
        /// Gets all subscription plans for the specified agency.
        /// </summary>
        /// <param name="id">The agency identifier.</param>
        /// <returns>All subscription plans for the agency.</returns>
        [HttpGet("all-plans")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSubscriptionPlan(int id)
        {
            var result = await _agencySubscriptionPlanService.GetAllSubscriptionPlan(id);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Gets the list of all available subscription plans for the current agency user.
        /// </summary>
        /// <returns>List of all available subscription plans.</returns>
        [HttpGet("plan-list")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSubscriptionPlanList(int id)
        {
            var result = await _agencySubscriptionPlanService.GetSubscriptionAllPlanList(id);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }


        /// <summary>
        /// Cancels the current subscription for the specified agency.
        /// </summary>
        /// <param name="id">The subscription identifier.</param>
        /// <returns>Result of the cancellation operation.</returns>
        [HttpPost("cancel")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelSubscription(int id, int? subid)
        {
            var result = await _agencySubscriptionPlanService.CancelSubscriptionAsync(id, subid);

            return result
                ? Ok(new JsonModel(null, StatusMessage.CancelSubscription, StatusCodes.Status200OK))
                : BadRequest(new JsonModel(null, StatusMessage.CancelSubscriptionError, StatusCodes.Status400BadRequest));
        }





        /// <summary>
        /// Purchases a new subscription plan for the agency.
        /// </summary>
        /// <param name="session">The session metadata containing plan and agency details.</param>
        /// <returns>Result of the purchase operation.</returns>
        [HttpPost("purchase")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BuyPlan([FromBody] SessionMetadataModel session)
        {
            var result = await _agencySubscriptionPlanService.BuyPlan(session);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        [HttpGet("plan-byid")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSubscriptionPlanId(int planid,int organizationId)
        {
            var result = await _agencySubscriptionPlanService.GetSubscriptionPlanById(planid, organizationId);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }




    }
}
