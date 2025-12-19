using App.Application.Dto.Common;
using App.Application.Dto.MasterDatabase;
using App.Common.Models;

namespace App.Application.Interfaces.Services.MasterDatabase
{
    public interface IMasterDatabaseService
    {
        public Task<JsonModel> CreateMasterDatabaseAsync(MasterDatabaseDto dbDto, int userId);
        public Task<JsonModel> UpdateMasterDatabaseAsync(MasterDatabaseDto dbDto, int userId);
        public Task<JsonModel> GetAllMasterDatabaseAsync(MasterDatabaseFilterDto filter);
        Task<JsonModel> GetAllMasterDatabaseDropdownAsync();
        Task<JsonModel> GetMasterDatabaseByIdAsync(int databaseId);
        Task<JsonModel> MasterDatabaseStatusUpdateAsync(MasterDatabaseStatusUpdateDto dbDto, int userId);
        Task<JsonModel> GetMasterDatabaseCountsAsync();
    }
}
