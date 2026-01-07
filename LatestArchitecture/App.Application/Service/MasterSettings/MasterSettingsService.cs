using App.Application.Dto.Common;
using App.Application.Dto.MasterSetting;
using App.Application.Interfaces.Repositories;
using App.Application.Interfaces.Repositories.MasterSettings;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Interfaces.Services.MasterSettings;
using App.Common.Constant;
using App.Common.Models;
using App.Domain.Entities.MasterSettings;
using AutoMapper;
using System.Net;

namespace App.Application.Service.MasterSettings
{
    public class MasterSettingsService : IMasterSettingsService
    {
        /// <summary>
        /// Service implementation for handling operations related to Master Settings.
        /// This service follows Clean Architecture principles by delegating
        /// data persistence to the repository and handling business logic at the service layer.
        /// </summary>
        private readonly IMasterSettingsRepository _masterSettingsRepository;
        private readonly ICurrentUserClaimService _currentUserClaimService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterSettingsService"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository instance used to interact with master settings data source.
        /// </param>
        /// <param name="currentUser">
        /// Service for retrieving current user context/claims.
        /// </param>
        public MasterSettingsService(
            IMasterSettingsRepository masterSettingsRepository,
            ICurrentUserClaimService currentUserClaimService,
            IMapper mapper)
        {
            _masterSettingsRepository = masterSettingsRepository;
            _currentUserClaimService = currentUserClaimService;
            _mapper = mapper;
        }

        public async Task<JsonModel> DeleteAsync(int settingId)
        {
            // Validate input
            if (settingId <= 0)
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = StatusMessage.InvalidId,
                    Data = false
                };
            }

            // Check if the setting exists
            var existingSetting = await _masterSettingsRepository.GetByIdAsync(settingId);
            if (existingSetting == null)
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = StatusMessage.SettingNotFound,
                    Data = false
                };
            }
            existingSetting.IsDeleted = true;
            existingSetting.DeletedBy = _currentUserClaimService.UserId;
            existingSetting.DeletedAt = DateTime.UtcNow;

            _masterSettingsRepository.Update(existingSetting);
            await _masterSettingsRepository.SaveChangesAsync();



            // Return success
            return new JsonModel
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = StatusMessage.SettingDeleted,
                Data = true
            };
        }


        /// <summary>
        /// Retrieves all master settings.
        /// </summary>
        public async Task<JsonModel> GetAllAsync(FilterDto filter)
        {
            var settings = await _masterSettingsRepository.GetAllAsync(filter);

            if (settings == null || !settings.Any())
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = StatusMessage.NoSettingsFound,
                    Data = false
                };
            }
            int totalRecords = Convert.ToInt32(settings.FirstOrDefault()?.TotalCount ?? 0);

            return new JsonModel
            {
                Meta = new Meta
                {
                    TotalRecords = totalRecords,
                    CurrentPage = filter.PageNumber,
                    PageSize = filter.PageSize,
                    DefaultPageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling((decimal)totalRecords / filter.PageSize)
                },
                StatusCode = (int)HttpStatusCode.OK,
                Message = StatusMessage.SettingsRetrieved,
                Data = settings
            };
        }

        public async Task<JsonModel> GetAllSettingNamesAsync()
        {
            var settings = await _masterSettingsRepository.GetAllSettingNamesAsync();

            if (settings == null || !settings.Any())
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = StatusMessage.NoSettingsFound,
                    Data = false
                };
            }

            return new JsonModel
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = StatusMessage.SettingsRetrieved,
                Data = settings
            };
        }


        /// <summary>
        /// Retrieves a master setting by its Id.
        /// </summary>
        /// <param name="id">The Id of the master setting.</param>
        public async Task<JsonModel> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = StatusMessage.InvalidId,
                    Data = false
                };
            }

            var setting = await _masterSettingsRepository.GetByIdAsync(id);
            if (setting == null)
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = StatusMessage.SettingNotFound,
                    Data = false
                };
            }

            return new JsonModel
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = StatusMessage.SettingsRetrieved,
                Data = setting
            };
        }

        /// <summary>
        /// Saves or updates a master setting asynchronously.
        /// </summary>
        /// <param name="masterSettingDto">The master setting DTO containing setting details.</param>
        /// <returns>A <see cref="JsonModel"/> indicating success or failure.</returns>

        public async Task<JsonModel> SaveAsync(MasterSettingsDto masterSettingDto)
        {
            var jsonModel = new JsonModel
            {
                Data = false,
                Message = StatusMessage.InternalServerError,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            if (masterSettingDto == null)
            {
                jsonModel.Message = StatusMessage.SaveFailed;
                jsonModel.StatusCode = (int)HttpStatusCode.BadRequest;
                return jsonModel;
            }

            var userId = _currentUserClaimService.UserId;

            
            var existingSettingByName = await _masterSettingsRepository
                .GetByNameAsync(masterSettingDto.SettingName);

            if (existingSettingByName != null && masterSettingDto.Id == 0)
            {
                jsonModel.Message = StatusMessage.SettingExist;
                jsonModel.StatusCode = (int)HttpStatusCode.Conflict; // 409
                return jsonModel;
            }

            MasterSetting entity;

            if (masterSettingDto.Id == 0)
            {
                // New entity
                entity = _mapper.Map<MasterSetting>(masterSettingDto);
                entity.IsActive = true;
                entity.CreatedBy = userId;
                entity.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                // Update existing tracked entity
                entity = await _masterSettingsRepository.GetByIdAsync(masterSettingDto.Id);
                if (entity == null)
                {
                    jsonModel.Message = StatusMessage.NotFound;
                    jsonModel.StatusCode = (int)HttpStatusCode.NotFound;
                    return jsonModel;
                }
                var originalCreatedAt = entity.CreatedAt;
                var originalCreatedBy = entity.CreatedBy;
                // Map DTO fields onto tracked entity
                _mapper.Map(masterSettingDto, entity);
                entity.CreatedAt = originalCreatedAt;
                entity.CreatedBy = originalCreatedBy;
                entity.UpdatedBy = userId;
                entity.UpdatedAt = DateTime.UtcNow;
            }

            await _masterSettingsRepository.SaveAsync(entity);

            var savedDto = _mapper.Map<MasterSettingsDto>(entity);
            jsonModel.Data = savedDto;
            jsonModel.StatusCode = (int)HttpStatusCode.OK;
            jsonModel.Message = StatusMessage.SaveSuccess;

            return jsonModel;
        }



        /// <summary>
        /// Toggles the active status of a master setting by its ID.
        /// </summary>
        /// <param name="settingId">The unique identifier of the master setting.</param>
        /// <param name="isActive">The desired status to set: <c>true</c> for active, <c>false</c> for inactive.</param>
        /// <returns>
        /// A <see cref="JsonModel"/> containing the result of the operation:
        /// <list type="bullet">
        /// <item>
        /// <description>StatusCode: HTTP status code representing the outcome.</description>
        /// </item>
        /// <item>
        /// <description>Message: A descriptive message indicating success or failure.</description>
        /// </item>
        /// <item>
        /// <description>Data: A boolean indicating whether the operation was successful.</description>
        /// </item>
        /// </list>
        /// Returns <c>BadRequest</c> if the ID is invalid, <c>NotFound</c> if the setting does not exist, or <c>OK</c> if the status was successfully toggled.
        /// </returns>
        public async Task<JsonModel> ToggleStatusAsync(int settingId, bool isActive)
        {
            if (settingId <= 0)
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = StatusMessage.InvalidId,
                    Data = false
                };
            }

            // Fetch the setting from repository
            var setting = await _masterSettingsRepository.GetByIdAsync(settingId);
            if (setting == null)
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = StatusMessage.SettingNotFound,
                    Data = false
                };
            }

            // Toggle the status
            setting.IsActive = isActive;

            // Update in database
            _masterSettingsRepository.Update(setting);
            await _masterSettingsRepository.SaveChangesAsync();

            return new JsonModel
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = isActive ? StatusMessage.SettingActivated : StatusMessage.SettingDeactivated,
                Data = true
            };
        }
    }
}
