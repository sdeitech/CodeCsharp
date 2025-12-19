using App.Application.Dto.EmailFactory;
using App.Application.Interfaces.Repositories.EmailFactory;
using App.Application.Interfaces.Services.EmailFactory;
using App.Common.Constant;
using App.Common.Models;
using App.Domain.Entities.EmailFactory;
using Microsoft.Extensions.Logging;
using System.Net;

namespace App.Application.Service.EmailConfiguration
{
    /// <summary>
    /// Service for managing email configuration operations
    /// </summary>
    public sealed class EmailConfigurationService : IEmailConfigurationService
    {
        private readonly IEmailProviderConfigRepository _emailProviderConfigRepository;
        private readonly ILogger<EmailConfigurationService> _logger;

        public EmailConfigurationService(
            IEmailProviderConfigRepository emailProviderConfigRepository,
            ILogger<EmailConfigurationService> logger)
        {
            _emailProviderConfigRepository = emailProviderConfigRepository ?? throw new ArgumentNullException(nameof(emailProviderConfigRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Sets or updates email configuration for an organization using stored procedure
        /// </summary>
        /// <param name="emailConfig">The email configuration DTO</param>
        /// <param name="performedBy">The user who performed the operation</param>
        /// <returns>A JsonModel containing the operation result</returns>
        public async Task<JsonModel> SetEmailConfigurationAsync(MasterEmailConfigurationDto emailConfig, string? performedBy = null)
        {
            if (emailConfig == null)
            {
                _logger.LogWarning("SetEmailConfigurationAsync called with null emailConfig");
                return CreateErrorResponse(StatusMessage.InvalidData, HttpStatusCode.BadRequest);
            }

            if (emailConfig.OrganizationId <= 0)
            {
                _logger.LogWarning("SetEmailConfigurationAsync called with invalid OrganizationId: {OrganizationId}",
                    emailConfig.OrganizationId);
                return CreateErrorResponse(StatusMessage.InvalidData, HttpStatusCode.BadRequest);
            }

            try
            {
                _logger.LogInformation("Setting email configuration for organization {OrganizationId} using stored procedure",
                    emailConfig.OrganizationId);

                // Prepare parameters for stored procedure
                string? smtpServer = null, userName = null, password = null, fromEmail = null, fromName = null;
                string? awsAccessKey = null, awsSecretKey = null, awsRegion = null;
                string? apiKey = null;
                string? connectionString = null, senderAddress = null;

                // Extract provider-specific parameters based on provider type
                switch (emailConfig.EmailProviderTypeId)
                {
                    case 1: // SMTP
                        if (emailConfig.SmtpConfig != null)
                        {
                            smtpServer = emailConfig.SmtpConfig.SmtpServer;
                            userName = emailConfig.SmtpConfig.UserName;
                            password = emailConfig.SmtpConfig.Password;
                            fromEmail = emailConfig.SmtpConfig.FromEmail;
                            fromName = emailConfig.SmtpConfig.FromName;
                        }
                        break;
                    case 2: // SendGrid
                        if (emailConfig.SendGridConfig != null)
                        {
                            apiKey = emailConfig.SendGridConfig.ApiKey;
                            fromEmail = emailConfig.SendGridConfig.FromEmail;
                            fromName = emailConfig.SendGridConfig.FromName;
                        }
                        break;
                    case 3: // Azure
                        if (emailConfig.AzureConfig != null)
                        {
                            connectionString = emailConfig.AzureConfig.ConnectionString;
                            senderAddress = emailConfig.AzureConfig.SenderAddress;
                        }
                        break;
                    case 4: // AWS SES
                        if (emailConfig.AwsSesConfig != null)
                        {
                            awsAccessKey = emailConfig.AwsSesConfig.AwsAccessKey;
                            awsSecretKey = emailConfig.AwsSesConfig.AwsSecretKey;
                            awsRegion = emailConfig.AwsSesConfig.AwsRegion;
                            fromEmail = emailConfig.AwsSesConfig.FromEmail;
                            fromName = emailConfig.AwsSesConfig.FromName;
                        }
                        break;
                }

                // Call stored procedure
                var result = await _emailProviderConfigRepository.SetEmailConfigurationAsync(
                    organizationId: emailConfig.OrganizationId,
                    emailProviderTypeId: emailConfig.EmailProviderTypeId,
                    isActive: emailConfig.IsActive,
                    enableSsl: emailConfig.EnableSsl,
                    templatesPath: emailConfig.TemplatesPath,
                    smtpServer: smtpServer,
                    port: emailConfig.SmtpConfig?.Port,
                    userName: userName,
                    password: password,
                    fromEmail: fromEmail,
                    fromName: fromName,
                    awsAccessKey: awsAccessKey,
                    awsSecretKey: awsSecretKey,
                    awsRegion: awsRegion,
                    apiKey: apiKey,
                    connectionString: connectionString,
                    senderAddress: senderAddress,
                    performedBy: performedBy ?? "System"
                );

                if (result.Status == "SUCCESS")
                {
                    _logger.LogInformation("Successfully set email configuration for organization {OrganizationId}. ConfigId: {ConfigId}",
                        emailConfig.OrganizationId, result.ConfigId);

                    return CreateSuccessResponse(
                        new { ConfigId = result.ConfigId },
                        result.Message,
                        HttpStatusCode.OK
                    );
                }
                else
                {
                    _logger.LogWarning("Failed to set email configuration for organization {OrganizationId}: {Message}",
                        emailConfig.OrganizationId, result.Message);

                    return CreateErrorResponse(
                        result.Message,
                        HttpStatusCode.BadRequest
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during SetEmailConfigurationAsync for organization {OrganizationId}",
                    emailConfig.OrganizationId);

                return CreateErrorResponse(
                    StatusMessage.InternalServerError,
                    HttpStatusCode.InternalServerError
                );
            }
        }

        /// <summary>
        /// Gets email configurations for an organization
        /// </summary>
        /// <param name="organizationId">The organization ID</param>
        /// <returns>A JsonModel containing the list of email configuration response DTOs</returns>
        public async Task<JsonModel> GetEmailConfigurationAsync(int organizationId)
        {
            if (organizationId <= 0)
            {
                _logger.LogWarning("GetEmailConfigurationAsync called with invalid organizationId: {OrganizationId}", organizationId);
                return CreateErrorResponse(StatusMessage.InvalidData, HttpStatusCode.BadRequest);
            }

            try
            {
                _logger.LogInformation("Getting email configurations for organization {OrganizationId}", organizationId);

                var configs = await _emailProviderConfigRepository.GetActiveProvidersByOrganizationIdAsync(organizationId);

                if (configs == null || !configs.Any())
                {
                    _logger.LogInformation("No email configurations found for organization {OrganizationId}", organizationId);
                    return CreateSuccessResponse(
                        new List<MasterEmailConfigurationResponseDto>(),
                        StatusMessage.FetchSuccessfully,
                        HttpStatusCode.OK
                    );
                }

                // Convert entities to DTOs
                var configDtos = new List<MasterEmailConfigurationResponseDto>();
                foreach (var config in configs)
                {
                    var configDto = await ConvertToDtoAsync(config);
                    configDtos.Add(configDto);
                }

                _logger.LogInformation("Successfully retrieved {Count} email configurations for organization {OrganizationId}",
                    configDtos.Count, organizationId);

                return CreateSuccessResponse(
                    configDtos,
                    StatusMessage.FetchSuccessfully,
                    HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during GetEmailConfigurationAsync for organization {OrganizationId}", organizationId);

                return CreateErrorResponse(
                    StatusMessage.InternalServerError,
                    HttpStatusCode.InternalServerError
                );
            }
        }

        /// <summary>
        /// Gets all email configurations
        /// </summary>
        /// <returns>A JsonModel containing the list of email configuration response DTOs</returns>
        public async Task<JsonModel> GetAllEmailConfigurationsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all email configurations");

                var configs = await _emailProviderConfigRepository.GetAllAsync();

                if (configs == null || !configs.Any())
                {
                    _logger.LogInformation("No email configurations found");
                    return CreateSuccessResponse(
                        new List<MasterEmailConfigurationResponseDto>(),
                        StatusMessage.FetchSuccessfully,
                        HttpStatusCode.OK
                    );
                }

                // Convert entities to DTOs
                var configDtos = new List<MasterEmailConfigurationResponseDto>();
                foreach (var config in configs)
                {
                    var configDto = await ConvertToDtoAsync(config);
                    configDtos.Add(configDto);
                }

                _logger.LogInformation("Successfully retrieved {Count} email configurations", configDtos.Count);

                return CreateSuccessResponse(
                    configDtos,
                    StatusMessage.FetchSuccessfully,
                    HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during GetAllEmailConfigurationsAsync");

                return CreateErrorResponse(
                    StatusMessage.InternalServerError,
                    HttpStatusCode.InternalServerError
                );
            }
        }

        /// <summary>
        /// Gets email configurations with pagination, search, and sorting using stored procedure
        /// </summary>
        /// <param name="pageNumber">Page number for pagination</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="search">Search term for filtering</param>
        /// <param name="sortColumn">Column to sort by</param>
        /// <param name="sortDirection">Sort direction (ASC/DESC)</param>
        /// <returns>A JsonModel containing paginated email configuration results</returns>
        public async Task<JsonModel> GetEmailConfigurationsWithPaginationAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? sortColumn,
            string? sortDirection)
        {
            try
            {
                _logger.LogInformation("Getting email configurations with pagination. Page: {PageNumber}, Size: {PageSize}, Search: {Search}, Sort: {SortColumn} {SortDirection}",
                    pageNumber, pageSize, search, sortColumn, sortDirection);

                // Validate parameters
                if (pageNumber < 1)
                {
                    _logger.LogWarning("Invalid page number: {PageNumber}, setting to 1", pageNumber);
                    pageNumber = 1;
                }

                if (pageSize < 1 || pageSize > 100)
                {
                    _logger.LogWarning("Invalid page size: {PageSize}, setting to 10", pageSize);
                    pageSize = 10;
                }

                // Validate sort column
                var validSortColumns = new[] { "OrganizationName", "ProviderName", "ConfigId" };
                if (string.IsNullOrEmpty(sortColumn) || !validSortColumns.Contains(sortColumn))
                {
                    _logger.LogWarning("Invalid sort column: {SortColumn}, setting to OrganizationName", sortColumn);
                    sortColumn = "OrganizationName";
                }

                // Validate sort direction
                if (string.IsNullOrEmpty(sortDirection) || (sortDirection != "ASC" && sortDirection != "DESC"))
                {
                    _logger.LogWarning("Invalid sort direction: {SortDirection}, setting to ASC", sortDirection);
                    sortDirection = "ASC";
                }

                // Call repository method
                var result = await _emailProviderConfigRepository.GetEmailConfigurationsAsync(
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    search: search,
                    sortColumn: sortColumn,
                    sortDirection: sortDirection
                );

                if (result == null)
                {
                    _logger.LogWarning("Repository returned null result for GetEmailConfigurationsAsync");
                    return CreateErrorResponse(
                        StatusMessage.InternalServerError,
                        HttpStatusCode.InternalServerError
                    );
                }

                _logger.LogInformation("Successfully retrieved {Count} email configurations (Total: {TotalCount})",
                    result.Items.Count, result.TotalCount);

                return CreateSuccessResponse(
                    result,
                    StatusMessage.FetchSuccessfully,
                    HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during GetEmailConfigurationsWithPaginationAsync");

                return CreateErrorResponse(
                    StatusMessage.InternalServerError,
                    HttpStatusCode.InternalServerError
                );
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Creates a new email configuration
        /// </summary>
        private async Task CreateEmailConfigurationAsync(MasterEmailConfigurationDto dto)
        {
            var config = new EmailProviderConfigs
            {
                OrganizationId = dto.OrganizationId,
                EmailProviderTypeId = dto.EmailProviderTypeId,
                IsActive = dto.IsActive,
                EnableSsl = dto.EnableSsl,
                TemplatesPath = dto.TemplatesPath,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add provider-specific configuration
            await AddProviderSpecificConfigAsync(config, dto);

            await _emailProviderConfigRepository.AddAsync(config);
        }

        /// <summary>
        /// Updates an existing email configuration
        /// </summary>
        private async Task UpdateEmailConfigurationAsync(EmailProviderConfigs existingConfig, MasterEmailConfigurationDto dto)
        {
            existingConfig.EmailProviderTypeId = dto.EmailProviderTypeId;
            existingConfig.IsActive = dto.IsActive;
            existingConfig.EnableSsl = dto.EnableSsl;
            existingConfig.TemplatesPath = dto.TemplatesPath;
            existingConfig.UpdatedAt = DateTime.UtcNow;

            // Clear existing provider-specific configuration
            existingConfig.SmtpConfig = null;
            existingConfig.AwsSesConfig = null;
            existingConfig.SendGridConfig = null;
            existingConfig.AzureConfig = null;

            // Add new provider-specific configuration
            await AddProviderSpecificConfigAsync(existingConfig, dto);

            await _emailProviderConfigRepository.UpdateAsync(existingConfig);
        }

        /// <summary>
        /// Adds provider-specific configuration
        /// </summary>
        private async Task AddProviderSpecificConfigAsync(EmailProviderConfigs config, MasterEmailConfigurationDto dto)
        {
            switch (dto.EmailProviderTypeId)
            {
                case 1: // SMTP
                    if (dto.SmtpConfig != null)
                    {
                        config.SmtpConfig = new SmtpConfig
                        {
                            SmtpServer = dto.SmtpConfig.SmtpServer,
                            Port = dto.SmtpConfig.Port,
                            UserName = dto.SmtpConfig.UserName,
                            Password = dto.SmtpConfig.Password,
                            FromEmail = dto.SmtpConfig.FromEmail,
                            FromName = dto.SmtpConfig.FromName
                        };
                    }
                    break;
                case 2: // SendGrid
                    if (dto.SendGridConfig != null)
                    {
                        config.SendGridConfig = new SendGridConfig
                        {
                            ApiKey = dto.SendGridConfig.ApiKey,
                            FromEmail = dto.SendGridConfig.FromEmail,
                            FromName = dto.SendGridConfig.FromName
                        };
                    }
                    break;
                case 3: // Azure
                    if (dto.AzureConfig != null)
                    {
                        config.AzureConfig = new AzureConfig
                        {
                            ConnectionString = dto.AzureConfig.ConnectionString,
                            SenderAddress = dto.AzureConfig.SenderAddress
                        };
                    }
                    break;
                case 4: // AWS SES
                    if (dto.AwsSesConfig != null)
                    {
                        config.AwsSesConfig = new AwsSesConfig
                        {
                            AwsAccessKey = dto.AwsSesConfig.AwsAccessKey,
                            AwsSecretKey = dto.AwsSesConfig.AwsSecretKey,
                            AwsRegion = dto.AwsSesConfig.AwsRegion,
                            FromEmail = dto.AwsSesConfig.FromEmail,
                            FromName = dto.AwsSesConfig.FromName
                        };
                    }
                    break;
            }
        }



        /// <summary>
        /// Converts EmailProviderConfig entity to DTO
        /// </summary>
        private async Task<MasterEmailConfigurationResponseDto> ConvertToDtoAsync(EmailProviderConfigs config)
        {
            var dto = new MasterEmailConfigurationResponseDto
            {
                Id = config.Id,
                OrganizationId = config.OrganizationId,
                EmailProviderTypeId = config.EmailProviderTypeId,
                EmailProviderTypeName = config.EmailProviderType?.Name, // Include provider type name
                IsActive = config.IsActive,
                EnableSsl = config.EnableSsl,
                TemplatesPath = config.TemplatesPath,
                CreatedAt = config.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = config.UpdatedAt ?? DateTime.UtcNow
            };

            // Add provider-specific configuration
            if (config.SmtpConfig != null)
            {
                dto.SmtpConfig = new SmtpConfigResponseDto
                {
                    SmtpServer = config.SmtpConfig.SmtpServer,
                    Port = (int)config.SmtpConfig.Port,
                    UserName = config.SmtpConfig.UserName,
                    Password = config.SmtpConfig.Password,
                    FromEmail = config.SmtpConfig.FromEmail,
                    FromName = config.SmtpConfig.FromName
                };
            }
            else if (config.AwsSesConfig != null)
            {
                dto.AwsSesConfig = new AwsSesConfigResponseDto
                {
                    AwsAccessKey = config.AwsSesConfig.AwsAccessKey,
                    AwsSecretKey = config.AwsSesConfig.AwsSecretKey,
                    AwsRegion = config.AwsSesConfig.AwsRegion,
                    FromEmail = config.AwsSesConfig.FromEmail,
                    FromName = config.AwsSesConfig.FromName
                };
            }
            else if (config.SendGridConfig != null)
            {
                dto.SendGridConfig = new SendGridConfigResponseDto
                {
                    ApiKey = config.SendGridConfig.ApiKey,
                    FromEmail = config.SendGridConfig.FromEmail,
                    FromName = config.SendGridConfig.FromName
                };
            }
            else if (config.AzureConfig != null)
            {
                dto.AzureConfig = new AzureConfigResponseDto
                {
                    ConnectionString = config.AzureConfig.ConnectionString,
                    SenderAddress = config.AzureConfig.SenderAddress
                };
            }

            return dto;
        }

        /// <summary>
        /// Creates a standardized success response
        /// </summary>
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
        /// Creates a standardized error response
        /// </summary>
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
