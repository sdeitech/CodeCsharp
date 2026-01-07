using App.Application.Dto.Common;
using App.Application.Dto.MasterSystemSettings;
using App.Application.Dto.SubscriptionPlan;
using App.Application.Interfaces.Repositories.MasterSettings;
using App.Application.Interfaces.Repositories.MasterSystemSettings;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Interfaces.Services.MasterSystemSettingsService;
using App.Common.Constant;
using App.Common.Models;
using App.Domain.Entities.MasterSettings;
using App.Domain.Entities.MasterSystemSettings;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Service.MasterSystemSettings
{
    /// <summary>
    /// Service implementation for managing Master System Settings.
    /// Handles CRUD operations, toggle functionality, and business logic.
    /// </summary>
    public class MasterSystemSettingsService : IMasterSystemSettingsService
    {
        private readonly IMasterSystemSettingsRepository _repository;
        private readonly ICurrentUserClaimService _currentUserClaimService;
        private readonly IMapper _mapper;
        /// <summary>
        /// Service implementation for handling operations related to Master Settings.
        /// This service follows Clean Architecture principles by delegating
        /// data persistence to the repository and handling business logic at the service layer.
        /// </summary>
        /// 
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterSystemSettingsService"/> class.
        /// </summary>
        public MasterSystemSettingsService(
            IMasterSystemSettingsRepository repository,
            ICurrentUserClaimService currentUserClaimService,
            IMapper mapper)
        {
            _repository = repository;
            _currentUserClaimService = currentUserClaimService;
            _mapper = mapper;
        }


        #region Delete (Soft Delete)
        /// <summary>
        /// Soft deletes a system setting by marking IsDeleted.
        /// </summary>
        public async Task<JsonModel> DeleteAsync(int settingId)
        {
            if (settingId <= 0)
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = StatusMessage.InvalidId,
                    Data = false
                };

            var setting = await _repository.GetByIdAsync(settingId);
            if (setting == null)
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = StatusMessage.SettingNotFound,
                    Data = false
                };

            setting.IsDeleted = true;
            setting.DeletedBy = _currentUserClaimService.UserId;
            setting.DeletedAt = DateTime.UtcNow;

            _repository.Update(setting);
            await _repository.SaveChangesAsync();

            return new JsonModel
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = StatusMessage.SettingDeleted,
                Data = true
            };
        }
        #endregion

        #region Get All
        /// <summary>
        /// Retrieves all system settings.
        /// </summary>
        public async Task<JsonModel> GetAllAsync(FilterDto filter)
        {
            var settings = await _repository.GetAllAsync(filter);


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
        #endregion

        #region Get By Id
        /// <summary>
        /// Retrieves a system setting by its unique identifier.
        /// </summary>
        public async Task<JsonModel> GetByIdAsync(int id)
        {
            if (id <= 0)
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = StatusMessage.InvalidId,
                    Data = false
                };

            var setting = await _repository.GetByIdAsync(id);

            if (setting == null)
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = StatusMessage.SettingNotFound,
                    Data = false
                };

            return new JsonModel
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = StatusMessage.SettingsRetrieved,
                Data = setting
            };
        }
        #endregion

        #region Save / Update
        /// <summary>
        /// Creates a new system setting or updates an existing one.
        /// </summary>
        /// <param name="masterSettingDto">The DTO containing system setting data.</param>
        /// <returns>A <see cref="JsonModel"/> indicating success or failure.</returns>
        public async Task<JsonModel> SaveAsync(MasterSystemSettingsDto masterSettingDto)
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
            // Case-insensitive duplicate check
            var existingSettingByName = await _repository
                .GetByNameAsync(masterSettingDto.SystemSettingName);

            if (existingSettingByName != null && masterSettingDto.Id == 0)
            {
                jsonModel.Message = StatusMessage.SettingExist;
                jsonModel.StatusCode = (int)HttpStatusCode.Conflict; // 409
                return jsonModel;
            }

            MasterSystemSetting entity;

            if (masterSettingDto.Id == 0)
            {
                // New entity
                entity = _mapper.Map<MasterSystemSetting>(masterSettingDto);
                entity.IsActive = true;
                entity.CreatedBy = userId;
                entity.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                // Update existing tracked entity
                entity = await _repository.GetByIdAsync(masterSettingDto.Id);
                if (entity == null)
                {
                    jsonModel.Message = StatusMessage.NotFound;
                    jsonModel.StatusCode = (int)HttpStatusCode.NotFound;
                    return jsonModel;
                }

                // Map DTO fields onto tracked entity
                _mapper.Map(masterSettingDto, entity);
                entity.UpdatedBy = userId;
                entity.UpdatedAt = DateTime.UtcNow;
            }

            
            await _repository.SaveAsync(entity);

            // Map back to DTO
            var savedDto = _mapper.Map<MasterSystemSettingsDto>(entity);

            jsonModel.Data = savedDto;
            jsonModel.StatusCode = (int)HttpStatusCode.OK;
            jsonModel.Message = StatusMessage.SaveSuccess;

            return jsonModel;
        }
        #endregion

        #region Toggle IsActive
        /// <summary>
        /// Activates or deactivates a system setting.
        /// </summary>
        public async Task<JsonModel> ToggleStatusAsync(int settingId, bool isActive)
        {
            if (settingId <= 0)
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = StatusMessage.InvalidId,
                    Data = false
                };

            var setting = await _repository.GetByIdAsync(settingId);
            if (setting == null)
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = StatusMessage.SettingNotFound,
                    Data = false
                };

            setting.IsActive = isActive;
            setting.UpdatedBy = _currentUserClaimService.UserId;
            setting.UpdatedAt = DateTime.UtcNow;

            _repository.Update(setting);
            await _repository.SaveChangesAsync();

            return new JsonModel
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = isActive ? StatusMessage.SettingActivated : StatusMessage.SettingDeactivated,
                Data = true
            };
        }
        #endregion

        #region Toggle SystemSettingValue
        /// <summary>
        /// Updates the boolean SystemSettingValue (frontend toggle).
        /// </summary>
        public async Task<JsonModel> ToggleValueAsync(int settingId, bool value)
        {
            if (settingId <= 0)
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = StatusMessage.InvalidId,
                    Data = false
                };

            var setting = await _repository.GetByIdAsync(settingId);
            if (setting == null)
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = StatusMessage.SettingNotFound,
                    Data = false
                };

            setting.SystemSettingValue = value;
            setting.UpdatedBy = _currentUserClaimService.UserId;
            setting.UpdatedAt = DateTime.UtcNow;

            _repository.Update(setting);
            await _repository.SaveChangesAsync();

            return new JsonModel
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = value ? StatusMessage.SettingEnabled : StatusMessage.SettingDisabled,
                Data = true
            };
        }
        #endregion
    }
}
