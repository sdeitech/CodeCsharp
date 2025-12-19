﻿using App.Application.Dto.Common;
using App.Application.Dto.MasterAdmin;
using App.Application.Interfaces.Services;
using App.Application.Service.MasterTimeZones;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.Api.Controllers.MasterAdmin
{
    [ServiceFilter(typeof(DateTimeConversionFilterService))]
    [ApiController]
    [Route("/api/[controller]")]
    public class MasterAdminController(IAdminService adminService) : ControllerBase
    {
        private readonly IAdminService _adminService = adminService;

        /// <summary>
        /// Create or update a Master Admin
        /// </summary>
        /// <param name="masterAdminDto">The Master Admin data transfer object containing admin details</param>
        /// <returns>
        /// Returns a <see cref="JsonModel"/> containing the created/updated admin details if successful,
        /// or an error message if the operation fails.
        /// </returns>
        /// <response code="200">Master Admin created or updated successfully</response>
        /// <response code="400">Invalid input data provided</response>
        /// <response code="422">Unprocessable entity - data validation failed</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpPost("add-update")]
      
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddOrUpdateAsync([FromBody] MasterAdminDto masterAdminDto)
        {
          
            var performedBy = User?.Identity?.Name
                              ?? User?.FindFirst("email")?.Value
                              ?? "System";

            var result = await _adminService.AddOrUpdateAsync(masterAdminDto, performedBy);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                StatusCodes.Status422UnprocessableEntity => UnprocessableEntity(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Get all Master Admins with filtering, sorting, and paging
        /// </summary>
        /// <param name="filter">The filter criteria for retrieving Master Admins</param>
        /// <returns>
        /// Returns a <see cref="JsonModel"/> containing the list of Master Admins if successful,
        /// or an error message if the operation fails.
        /// </returns>
        /// <response code="200">Master Admins retrieved successfully</response>
        /// <response code="400">Invalid filter criteria provided</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpPost("get-all")]
        
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync([FromBody] FilterDto filter)
        {
            var result = await _adminService.GetAllAdminAsync(filter);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Soft delete a Master Admin by Id
        /// </summary>
        /// <param name="agencyAdminId">The ID of the Master Admin to delete</param>
        /// <returns>
        /// Returns a <see cref="JsonModel"/> indicating whether the deletion was successful,
        /// or an error message if the operation fails.
        /// </returns>
        /// <response code="200">Master Admin deleted successfully</response>
        /// <response code="400">Invalid admin ID provided</response>
        /// <response code="404">Master Admin not found</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpDelete("delete/{agencyAdminId}")]

        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(int agencyAdminId)
        {
            var result = await _adminService.DeleteAsync(agencyAdminId);

            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Toggle active status of a Master Admin
        /// </summary>
        /// <param name="agencyAdminId">The ID of the Master Admin to activate/deactivate</param>
        /// <param name="isActive">The new active status (true for activate, false for deactivate)</param>
        /// <returns>
        /// Returns a <see cref="JsonModel"/> indicating whether the status update was successful,
        /// or an error message if the operation fails.
        /// </returns>
        /// <response code="200">Master Admin status updated successfully</response>
        /// <response code="400">Invalid admin ID provided</response>
        /// <response code="404">Master Admin not found</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpPatch("toggle-status/{agencyAdminId}/{isActive}")]

        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ToggleActiveStatusAsync(int agencyAdminId, bool isActive)
        {
            var result = await _adminService.ToggleActiveStatusAsync(agencyAdminId, isActive);

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
