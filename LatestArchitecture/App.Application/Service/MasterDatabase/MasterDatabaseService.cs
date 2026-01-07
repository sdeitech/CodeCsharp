using App.Application.Interfaces.Services.MasterDatabase;
using App.Common.Models;
using App.Application.Dto.MasterDatabase;
using App.Application.Dto.Common;
using App.Common.Constant;
using System.Net;
using static App.Common.Constant.Constants;
using App.Application.Interfaces.Repositories.AuditLogs;
using App.Application.Interfaces.Repositories.MasterDatabase;
using App.Application.Dto.Organization;
using App.Application.Interfaces.Repositories.Organization;
using App.Common.Utility;
using Microsoft.AspNetCore.Http;

namespace App.Application.Service.MasterDatabase
{
    public class MasterDatabaseService(IMasterDatabaseRepository masterDatabaseRepository,
                   IAuditLogRepository auditLogRepository, IHttpContextAccessor httpContextAccessor) : IMasterDatabaseService
    {
        private readonly IMasterDatabaseRepository _masterDatabaseRepository = masterDatabaseRepository;
        private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<JsonModel> CreateMasterDatabaseAsync(MasterDatabaseDto dbDto, int userId)
        {
            bool resultData = await _masterDatabaseRepository.CreateDatabaseAsync(dbDto, userId);
            if (resultData)
            {
                return new JsonModel()
                {
                    Message = StatusMessage.DatabaseSavedSuccessfully,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            else
            {
                return new JsonModel()
                {
                    Message = StatusMessage.InternalServerError,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<JsonModel> GetAllMasterDatabaseAsync(MasterDatabaseFilterDto filter)
        {
            List<MasterDatabaseResponseDto> data = await _masterDatabaseRepository.GetAllMasterDatabaseAsync(filter);

            return data.Any()
            ? new JsonModel()
            {
                Meta = new Meta
                {
                    TotalRecords = data?.Count > 0 ? data[0].TotalRecords : 0,
                    CurrentPage = filter.PageNumber,
                    PageSize = filter.PageSize,
                    DefaultPageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling(Convert.ToDecimal((data?.Count > 0 ? data[0].TotalRecords : 0) / filter.PageSize))
                },
                Data = data,
                StatusCode = (int)HttpStatusCode.OK
            } : new JsonModel()
            {
                Message = StatusMessage.InternalServerError,
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }

        public async Task<JsonModel> GetAllMasterDatabaseDropdownAsync()
        {
            List<MasterDatabaseResponseForDropdownDto> data = await _masterDatabaseRepository.GetAllMasterDatabaseDropdownAsync();

            return data.Any()
            ? new JsonModel { Data = data, StatusCode = (int)HttpStatusCode.OK }
            : new JsonModel { Message = StatusMessage.InternalServerError, StatusCode = (int)HttpStatusCode.InternalServerError };
        }

        public async Task<JsonModel> GetMasterDatabaseByIdAsync(int databaseId)
        {
            MasterDatabaseResponseDto data = await _masterDatabaseRepository.GetMasterDatabaseByIdAsync(databaseId);

            return data != null
            ? new JsonModel { Data = data, StatusCode = (int)HttpStatusCode.OK }
            : new JsonModel { Message = StatusMessage.InternalServerError, StatusCode = (int)HttpStatusCode.InternalServerError };
        }

        public async Task<JsonModel> UpdateMasterDatabaseAsync(MasterDatabaseDto dbDto, int userId)
        {
            var database = await _masterDatabaseRepository.GetByIdAsync(dbDto.DatabaseID);
            if (database == null)
            {
                return new JsonModel
                {
                    Message = StatusMessage.NoDataFound,
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }

            database.DatabaseName = dbDto.DatabaseName;
            database.UserName = dbDto.UserName;
            database.ServerName = dbDto.ServerName;
            database.Password = dbDto.Password;
            database.IsCentralized = dbDto.IsCentralized;
            database.ParentOrganizationID = dbDto.ParentOrganizationID;
            database.UpdatedAt = DateTime.UtcNow;
            database.UpdatedBy = userId;

            _masterDatabaseRepository.Update(database);
            //await _unitOfWork.CommitAsync();
            //int userid = 2; // This should be taken from the token, hardcoded for now
            string ipAddress = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);
            _auditLogRepository.SaveChangesWithAuditLogs(AuditLogsScreen.UpdateDatabase, (int)MasterActions.Update, userId, null, ipAddress, (int)MasterPortal.SuperAdminPortal, null, null, null);
            return new JsonModel
            {
                Message = StatusMessage.RecordSavedSuccessfully,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        public async Task<JsonModel> MasterDatabaseStatusUpdateAsync(MasterDatabaseStatusUpdateDto dbDto, int userId)
        {
            var database = await _masterDatabaseRepository.GetByIdAsync(dbDto.DatabaseID);
            if (database == null)
                return new JsonModel()
                {
                    Message = StatusMessage.NoDataFound,
                    StatusCode = (int)HttpStatusCode.OK
                };

            database.IsActive = dbDto.IsActive;
            database.UpdatedBy = userId;
            database.UpdatedAt = DateTime.UtcNow;

            _masterDatabaseRepository.Update(database);
            //await _unitOfWork.CommitAsync();
            string ipAddress = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);
            _auditLogRepository.SaveChangesWithAuditLogs(AuditLogsScreen.ManageDatabase, (int)MasterActions.Update, userId, null, ipAddress, (int)MasterPortal.SuperAdminPortal, null, null, null);
            return new JsonModel()
            {
                Message = StatusMessage.RecordSavedSuccessfully,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        public async Task<JsonModel> GetMasterDatabaseCountsAsync()
        {
            MasterDatabaseCountsResponseDto data = await _masterDatabaseRepository.GetMasterDatabaseCountsAsync();

            return data != null
            ? new JsonModel { Data = data, StatusCode = (int)HttpStatusCode.OK }
            : new JsonModel { Message = StatusMessage.InternalServerError, StatusCode = (int)HttpStatusCode.InternalServerError };
        }

    }
}
