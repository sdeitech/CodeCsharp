using App.Application.Dto.Common;
using App.Application.Dto.MasterAdmin;
using App.Application.Interfaces;
using App.Application.Interfaces.Repositories;
using App.Application.Interfaces.Services;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Interfaces.Repositories.AuditLogs;
using App.Application.Interfaces.Repositories.EmailFactory;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using Amazon.SimpleEmail.Model;
using App.Application.Service.EmailProviders;
using App.Common.Utility;
using Microsoft.AspNetCore.Http;

namespace App.Application.Service.MasterAdmin
{
    /// <summary>
    /// Service for managing admin operations including CRUD operations and authentication.
    /// </summary>
    public sealed class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<AdminService> _logger;
        private readonly EmailProviderFactory _emailProviderFactory;
        private readonly IEmailProviderConfigRepository _emailProviderConfigRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminService(
            IEmailProviderConfigRepository emailProviderConfigRepository,
            IAdminRepository adminRepository,
            ILogger<AdminService> logger,
            EmailProviderFactory emailProviderFactory, IHttpContextAccessor httpContextAccessor)
        {
            _emailProviderConfigRepository = emailProviderConfigRepository ?? throw new ArgumentNullException(nameof(emailProviderConfigRepository));
            _adminRepository = adminRepository ?? throw new ArgumentNullException(nameof(adminRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailProviderFactory = emailProviderFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Adds a new admin or updates an existing admin.
        /// </summary>
        /// <param name="masterAdminDto">The admin data transfer object.</param>
        /// <param name="performedBy">The user performing the operation.</param>
        /// <param name="cancellationToken">Cancellation token for async operation.</param>
        /// <returns>A JsonModel containing the operation result.</returns>
        public async Task<JsonModel> AddOrUpdateAsync(
            MasterAdminDto masterAdminDto,
            string performedBy)
        {
            if (masterAdminDto is null)
            {
                _logger.LogWarning("AddOrUpdateAsync called with null masterAdminDto");
                return CreateErrorResponse(StatusMessage.InvalidData, HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(performedBy))
            {
                _logger.LogWarning("AddOrUpdateAsync called with null or empty performedBy parameter");
                return CreateErrorResponse(StatusMessage.InvalidData, HttpStatusCode.BadRequest);
            }

            try
            {
                _logger.LogInformation("Starting AddOrUpdateAsync operation for admin {AdminId} by {PerformedBy}",
                    masterAdminDto.AgencyAdminId, performedBy);

                var response = await _adminRepository.AddOrUpdateAdminAsync(masterAdminDto, performedBy);

                if (IsSuccessfulResponse(response))
                {
                    var successMessage = masterAdminDto.AgencyAdminId is null
                        ? StatusMessage.CreatedSuccessfully
                        : StatusMessage.UpdatedSuccessfully;

                    _logger.LogInformation("Successfully {Operation} admin with ID {AdminId}",
                        masterAdminDto.AgencyAdminId is null ? "created" : "updated", response.AgencyAdminId);
                    
                    // Retrieve the active email provider configuration
                    var emailConfig = await _emailProviderConfigRepository.GetActiveProviderByOrganizationIdAsync(masterAdminDto.AgencyId);
                    if (emailConfig != null)
                    {
                        try
                        {
                            // Create the email provider
                            var emailProvider = EmailProviderFactory.CreateProvider(emailConfig);
                            
                            // Define email subject and body
                            string subject = masterAdminDto.AgencyAdminId is null ? "New Admin Account Created" : "Admin Account Updated";
                            string body = $"Hello {masterAdminDto.FirstName} {masterAdminDto.LastName},\n\n" +
                                          $"Your admin account has been {(masterAdminDto.AgencyAdminId is null ? "created" : "updated")}.\n" +
                                          $"Agency: {masterAdminDto.AgencyName}\n" +
                                          $"Role: {masterAdminDto.RoleName}\n\n" +
                                          "Thank you!";

                            // Send the email
                            await emailProvider.SendEmailAsync(masterAdminDto.Email, subject, body);
                            _logger.LogInformation("Email notification sent successfully to {Email}", masterAdminDto.Email);
                        }
                        catch (Exception emailEx)
                        {
                            // Log email sending error but don't fail the main operation
                            _logger.LogWarning(emailEx, "Failed to send email notification to {Email}", masterAdminDto.Email);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No active email provider configuration found for organization ID {OrganizationId}", masterAdminDto.AgencyId);
                    }

                    return CreateSuccessResponse(
                        new { response.AgencyAdminId },
                        successMessage,
                        HttpStatusCode.OK
                    );
                }

                _logger.LogWarning("AddOrUpdateAsync operation failed: {Message}", response?.Message);
                return CreateErrorResponse(
                    response?.Message ?? StatusMessage.OperationFailed,
                    HttpStatusCode.BadRequest
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during AddOrUpdateAsync operation for admin {AdminId}",
                    masterAdminDto.AgencyAdminId);

                return CreateErrorResponse(
                    StatusMessage.InternalServerError,
                    HttpStatusCode.InternalServerError
                );
            }
        }

        /// <summary>
        /// Retrieves all admins based on the provided filter criteria.
        /// </summary>
        /// <param name="filter">Filter criteria for retrieving admins.</param>
        /// <param name="cancellationToken">Cancellation token for async operation.</param>
        /// <returns>A JsonModel containing the list of admins.</returns>
        public async Task<JsonModel> GetAllAdminAsync(FilterDto filterDto)
        {
            if (filterDto is null)
            {
                _logger.LogWarning("GetAllAdminAsync called with null filter");
                return new JsonModel
                {
                    Meta = new Meta(),
                    Data = Array.Empty<object>(),
                    Message = StatusMessage.InvalidData,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            try
            {
                _logger.LogInformation("Retrieving all admins with filter criteria {@Filter}", filterDto);

                var admins = await _adminRepository.GetAllAdminAsync(filterDto);

                if (admins?.Any() == true)
                {
                    int totalRecords = Convert.ToInt32(admins.FirstOrDefault()?.TotalCount ?? 0);
                    _logger.LogInformation("Successfully retrieved {Count} admin records", admins.Count());

                    return new JsonModel
                    {
                        Meta = new Meta
                        {
                            TotalRecords = totalRecords,
                            CurrentPage = filterDto.PageNumber,
                            PageSize = filterDto.PageSize,
                            DefaultPageSize = filterDto.PageSize,
                            TotalPages = (int)Math.Ceiling((decimal)totalRecords / filterDto.PageSize)
                        },
                        Data = admins,
                        Message = StatusMessage.FetchSuccessfully,
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }

                _logger.LogInformation("No admin records found matching the filter criteria");

                return new JsonModel
                {
                    Meta = new Meta
                    {
                        TotalRecords = 0,
                        CurrentPage = filterDto.PageNumber,
                        PageSize = filterDto.PageSize,
                        DefaultPageSize = filterDto.PageSize,
                        TotalPages = 0
                    },
                    Data = Array.Empty<object>(),
                    Message = StatusMessage.NoRecordsFound,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving admin records");

                return new JsonModel
                {
                    Meta = new Meta(),
                    Data = Array.Empty<object>(),
                    Message = StatusMessage.InternalServerError,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        /// <summary>
        /// Deletes an admin by their ID.
        /// </summary>
        /// <param name="agencyAdminId">The ID of the admin to delete.</param>
        /// <param name="cancellationToken">Cancellation token for async operation.</param>
        /// <returns>A JsonModel indicating the result of the delete operation.</returns>
        public async Task<JsonModel> DeleteAsync(int agencyAdminId)
        {
            if (agencyAdminId <= 0)
            {
                _logger.LogWarning("DeleteAsync called with invalid agencyAdminId: {AdminId}", agencyAdminId);
                return CreateErrorResponse(StatusMessage.InvalidData, HttpStatusCode.BadRequest);
            }

            try
            {
                _logger.LogInformation("Attempting to delete admin with ID {AdminId}", agencyAdminId);

                string ipAddress = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);
                var result = await _adminRepository.DeleteAsync(agencyAdminId , ipAddress);

                if (result)
                {
                    _logger.LogInformation("Successfully deleted admin with ID {AdminId}", agencyAdminId);
                    return CreateSuccessResponse(
                        true,
                        StatusMessage.DeletedSuccessfully,
                        HttpStatusCode.OK
                    );
                }

                _logger.LogWarning("Admin with ID {AdminId} not found for deletion", agencyAdminId);
                return CreateErrorResponse(
                    StatusMessage.NoRecordsFound,
                    HttpStatusCode.NotFound
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting admin with ID {AdminId}", agencyAdminId);

                return CreateErrorResponse(
                    StatusMessage.InternalServerError,
                    HttpStatusCode.InternalServerError
                );
            }
        }

        /// <summary>
        /// Toggles the active status of a Master Admin.
        /// </summary>
        /// <param name="agencyAdminId">The ID of the admin to update.</param>
        /// <param name="isActive">The new active status.</param>
        /// <returns>A JsonModel indicating the result of the operation.</returns>
        public async Task<JsonModel> ToggleActiveStatusAsync(int agencyAdminId, bool isActive)
        {
            if (agencyAdminId <= 0)
            {
                _logger.LogWarning("ToggleActiveStatusAsync called with invalid agencyAdminId: {AdminId}", agencyAdminId);
                return CreateErrorResponse(StatusMessage.InvalidData, HttpStatusCode.BadRequest);
            }

            try
            {
                _logger.LogInformation("Attempting to {Action} admin with ID {AdminId}", 
                    isActive ? "activate" : "deactivate", agencyAdminId);

                string ipAddress = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);
                var result = await _adminRepository.ToggleActiveStatusAsync(agencyAdminId, isActive,ipAddress);

                if (result)
                {
                    _logger.LogInformation("Successfully {Action} admin with ID {AdminId}", 
                        isActive ? "activated" : "deactivated", agencyAdminId);
                    return CreateSuccessResponse(
                        true,
                        isActive ? StatusMessage.ActivatedSuccessfully : StatusMessage.DeactivatedSuccessfully,
                        HttpStatusCode.OK
                    );
                }

                _logger.LogWarning("Admin with ID {AdminId} not found for status update", agencyAdminId);
                return CreateErrorResponse(
                    StatusMessage.NoRecordsFound,
                    HttpStatusCode.NotFound
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling active status for admin with ID {AdminId}", agencyAdminId);

                return CreateErrorResponse(
                    StatusMessage.InternalServerError,
                    HttpStatusCode.InternalServerError
                );
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Determines if the admin response indicates a successful operation.
        /// </summary>
        /// <param name="response">The admin response to evaluate.</param>
        /// <returns>True if the response indicates success; otherwise, false.</returns>
        private static bool IsSuccessfulResponse(AdminResponseDto? response)
        {
            return response is not null &&
                   string.Equals(response.Status, "SUCCESS", StringComparison.OrdinalIgnoreCase) &&
                   response.AgencyAdminId > 0;
        }

        /// <summary>
        /// Creates a standardized success response.
        /// </summary>
        /// <param name="data">The data to include in the response.</param>
        /// <param name="message">The success message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <returns>A JsonModel representing the success response.</returns>
        private static JsonModel CreateSuccessResponse(object? data, string message, HttpStatusCode statusCode)
        {
            return new JsonModel
            {
                Data = data,
                Message = message,
                StatusCode = (int)statusCode
            };
        }

        /// <summary>
        /// Creates a standardized error response.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <returns>A JsonModel representing the error response.</returns>
        private static JsonModel CreateErrorResponse(string message, HttpStatusCode statusCode)
        {
            return new JsonModel
            {
                Data = false,
                Message = message,
                StatusCode = (int)statusCode
            };
        }

        #endregion
    }
}
