using App.Application.Dto.Common;
using App.Application.Dto.SubscriptionPlan;
using App.Application.Interfaces.Services.SubscriptionPlan;
using App.Application.Service.MasterTimeZones;
using App.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace App.Api.Controllers
{

    /// <summary>
    /// Controller for managing Subscription Plans For Agency.
    /// </summary>
    //[ServiceFilter(typeof(DateTimeConversionFilterService))]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "SuperAdminScheme")]
    public class SubscriptionPlansController : BaseController
    {
        private readonly ISubscriptionPlansService _subscriptionservice;

        public SubscriptionPlansController(ISubscriptionPlansService subscriptionservice)
        {
            _subscriptionservice = subscriptionservice ?? throw new ArgumentNullException(nameof(subscriptionservice));
        }

        /// <summary>
        /// Creates or updates a subscription plan.
        /// </summary>
        /// <param name="subscriptionPlansDTO">The subscription plan data to save.</param>
        /// <returns>Result of the save operation.</returns>
        [HttpPost("save-plan")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SaveSubscriptionPlan(SubscriptionPlansDTO subscriptionPlansDTO)
        {
            JsonModel result = await _subscriptionservice.SaveSubscriptionPlan(subscriptionPlansDTO);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Deletes a subscription plan by its ID.
        /// </summary>
        /// <param name="id">The ID of the subscription plan to delete.</param>
        /// <returns>Result of the delete operation.</returns>
        [HttpDelete("delete-plan")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSubscriptionPlan(int id)
        {
            var result = await _subscriptionservice.DeleteSubscriptionPlan(id);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Gets a subscription plan by its ID.
        /// </summary>
        /// <param name="id">The ID of the subscription plan.</param>
        /// <returns>The subscription plan details.</returns>
        [HttpGet("plan-by-id")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
       
        public async Task<IActionResult> GetSubscriptionPlanId(int id)
        {
            var result = await _subscriptionservice.GetSubscriptionPlanId(id);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Gets all subscription plans with advanced filtering and pagination.
        /// </summary>
        /// <param name="listingFiltterDTO">Filter and pagination parameters.</param>
        /// <returns>List of subscription plans with metadata.</returns>
        [HttpPost("all-plans")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSubscriptionPlan([FromBody] ListingFiltterDTO listingFiltterDTO)
        {
            var result = await _subscriptionservice.GetAllSubscriptionPlan(listingFiltterDTO);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Gets paginated and filtered subscription plans.
        /// </summary>
        /// <param name="searchKey">Search keyword.</param>
        /// <param name="sortColumn">Column to sort by.</param>
        /// <param name="sortOrder">Sort order (asc/desc).</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Paginated list of subscription plans.</returns>
        [HttpPost("subscription-plans")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSubscriptionPlans([FromBody] FilterDto filterDto)
        {
            var result = await _subscriptionservice.GetSubscriptionPlans(filterDto);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Gets all subscription plans (no filter/pagination).
        /// </summary>
        /// <returns>List of all subscription plans.</returns>
        [HttpGet("all-plans")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSubscriptionPlans()
        {
            var result = await _subscriptionservice.GetAllSubscriptionPlans();

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Sets a subscription plan as active or inactive.
        /// </summary>
        /// <param name="planId">The ID of the plan to update.</param>
        /// <param name="value">True to activate, false to deactivate.</param>
        /// <returns>Result of the update operation.</returns>
        [HttpPost("set-status")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetActiveInActive(int planId, bool value)
        {
            var result = await _subscriptionservice.SetActiveInActive(planId, value);

            return result?.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Gets all available modules for subscription plans.
        /// </summary>
        /// <returns>List of all modules.</returns>
        [HttpGet("all-module-list")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllModuleList()
        {
            var result = await _subscriptionservice.GetAllModuleList();

            return result?.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Gets paginated and filtered subscription plans.
        /// </summary>
        /// <param name="searchKey">Search keyword.</param>
        /// <param name="sortColumn">Column to sort by.</param>
        /// <param name="sortOrder">Sort order (asc/desc).</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Paginated list of subscription plans.</returns>
       
        [HttpPost("subscriptions")]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrgSubscriptionPlans([FromBody] FilterDto filterDto)
        {
            var result = await _subscriptionservice.GetOrgSubscriptionPlans(filterDto);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

    }
}
