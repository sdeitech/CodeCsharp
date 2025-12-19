using App.Application.Dto.Common;
using App.Application.Dto.Organization;
using App.Application.Dto.StorageConfiguration;
using App.Common.Models;
using Microsoft.AspNetCore.Http;

namespace App.Application.Interfaces.Services.Organization
{
    public interface IOrganizationService
    {
        public Task<JsonModel> CreateOrganizationAsync(OrganizationDto orgDto, int userId);
        public Task<JsonModel> UpdateOrganizationAsync(OrganizationDto orgDto, int userId);
        public Task<JsonModel> OrganizationStatusUpdateAsync(OrganizationStatusUpdateDto orgDto, int userId);
        public Task<JsonModel> GetAllOrganizationsAsync(FilterDto filter);//List<OrganizationDto>
        Task<JsonModel> GetOrganizationByIdAsync(int organizationId, IHttpContextAccessor contextAccessor);//OrganizationDto
        Task<JsonModel> GetCardStatisticsAsync();//OrganizationCardStatisticsDto
        
        // Storage Configuration Methods
        Task<JsonModel> GetStorageConfigurationAsync(int organizationId);
        Task<JsonModel> SaveStorageConfigurationAsync(StorageConfigurationDto storageConfigurationDto, int userId);
        Task<JsonModel> DeleteStorageConfigurationAsync(int organizationId, int userId);
        Task<JsonModel> GetAllStorageConfigurationsAsync();
    }
}
