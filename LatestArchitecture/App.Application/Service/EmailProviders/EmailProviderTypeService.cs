using App.Application.Dto.EmailFactory;
using App.Application.Interfaces.Repositories.EmailFactory;
using App.Application.Interfaces.Services.EmailFactory;
using App.Common.Constant;
using App.Common.Models;
using Microsoft.Extensions.Logging;
using System.Net;

namespace App.Application.Service.EmailProviders
{
    /// <summary>
    /// Service for managing email provider type operations
    /// </summary>
    public class EmailProviderTypeService : IEmailProviderTypeService
    {
        private readonly IEmailProviderTypeRepository _emailProviderTypeRepository;
        private readonly ILogger<EmailProviderTypeService> _logger;

        public EmailProviderTypeService(
            IEmailProviderTypeRepository emailProviderTypeRepository,
            ILogger<EmailProviderTypeService> logger)
        {
            _emailProviderTypeRepository = emailProviderTypeRepository ?? throw new ArgumentNullException(nameof(emailProviderTypeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all active email provider types
        /// </summary>
        /// <returns>A JsonModel containing the list of email provider types</returns>
        public async Task<JsonModel> GetAllActiveProviderTypesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all active email provider types");

                // Validate repository dependency
                if (_emailProviderTypeRepository == null)
                {
                    _logger.LogError("EmailProviderTypeRepository is null");
                    return new JsonModel
                    {
                        Data = new List<EmailProviderTypeDto>(),
                        Message = "Repository dependency not available",
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }

                var providerTypes = await _emailProviderTypeRepository.GetAllAsync();

                if (providerTypes != null && providerTypes.Any())
                {
                    _logger.LogInformation("Found {Count} email provider types from database", providerTypes.Count());

                    var providerTypeDtos = new List<EmailProviderTypeDto>();

                    foreach (var pt in providerTypes)
                    {
                        if (pt == null)
                        {
                            _logger.LogWarning("Null provider type found in results, skipping");
                            continue;
                        }

                        try
                        {
                            var dto = new EmailProviderTypeDto
                            {
                                Id = pt.Id,
                                Name = pt.Name ?? string.Empty,
                                Description = pt.Description ?? string.Empty,
                                IsActive = pt.IsActive
                            };
                            providerTypeDtos.Add(dto);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error mapping provider type with ID {Id} to DTO", pt.Id);
                            // Continue processing other items
                        }
                    }

                    _logger.LogInformation("Successfully mapped {Count} email provider types to DTOs", providerTypeDtos.Count);

                    return new JsonModel
                    {
                        Data = providerTypeDtos,
                        Message = StatusMessage.FetchSuccessfully,
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }

                _logger.LogInformation("No active email provider types found in database");

                return new JsonModel
                {
                    Data = new List<EmailProviderTypeDto>(),
                    Message = StatusMessage.NoRecordsFound,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving email provider types. Exception: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}",
                    ex.GetType().Name, ex.Message, ex.StackTrace);

                return new JsonModel
                {
                    Data = new List<EmailProviderTypeDto>(),
                    Message = "An error occurred while retrieving email provider types. Please try again later.",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
