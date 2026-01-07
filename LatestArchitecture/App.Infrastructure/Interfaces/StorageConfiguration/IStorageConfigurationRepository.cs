using App.Application.Dto.StorageConfiguration;

namespace App.Infrastructure.Interfaces.Repositories.StorageConfiguration
{
    public interface IStorageConfigurationRepository
    {
        Task<StorageConfigurationResponseDto?> GetStorageConfigurationAsync(int organizationId);
        Task<StorageConfigurationResponseDto> SaveStorageConfigurationAsync(int organizationId, string storageType, string configurationJson, int userId);
        Task<bool> DeleteStorageConfigurationAsync(int organizationId, int userId);
        Task<List<StorageConfigurationResponseDto>> GetAllStorageConfigurationsAsync();
    }
}
