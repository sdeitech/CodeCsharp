using App.Application.Dto.Common;
using App.Application.Dto.Organization;
using App.Application.Dto.StorageConfiguration;
using App.Common.Models;

namespace App.Application.Interfaces.Repositories.Organization
{
    public interface IOrganizationRepository : IRepository<Domain.Entities.Organization.Organization>
    {
        public Task<JsonModel> CreateOrganizationAsync(OrganizationDto orgDto, int userId);
        public Task<JsonModel> UpdateOrganizationAsync(OrganizationDto orgDto, int userId);
        public Task<List<OrganizationReponseDto>> GetAllOrganizationsAsync(FilterDto filter);
        public Task<OrganizationReponseDto> GetOrganizationByIdAsync(int organizationId);
        public Task<OrganizationCardStatisticsDto> GetCardStatisticsAsync();
        
        // Storage Configuration Methods
        public Task<StorageConfigurationResponseDto?> GetStorageConfigurationAsync(int organizationId);
        public Task<StorageConfigurationResponseDto> SaveStorageConfigurationAsync(int organizationId, string storageType, AwsStorageConfigDto? awsConfig, AzureBlobStorageConfigDto? azureConfig, int userId);
        public Task<bool> DeleteStorageConfigurationAsync(int organizationId, int userId);
        public Task<List<StorageConfigurationResponseDto>> GetAllStorageConfigurationsAsync();
    }
}
