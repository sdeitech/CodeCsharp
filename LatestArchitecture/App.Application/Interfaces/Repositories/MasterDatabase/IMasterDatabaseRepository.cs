using App.Application.Dto.Common;
using App.Application.Dto.MasterDatabase;
using App.Common.Models;

namespace App.Application.Interfaces.Repositories.MasterDatabase
{
    public interface IMasterDatabaseRepository : IRepository<Domain.Entities.MasterDatabase.MasterDatabase>
    {
        public Task<bool> CreateDatabaseAsync(MasterDatabaseDto dbDto, int userId);
        public Task<List<MasterDatabaseResponseDto>> GetAllMasterDatabaseAsync(MasterDatabaseFilterDto filter);
        Task<List<MasterDatabaseResponseForDropdownDto>> GetAllMasterDatabaseDropdownAsync();
        public Task<MasterDatabaseResponseDto> GetMasterDatabaseByIdAsync(int databaseId);
        public Task<MasterDatabaseCountsResponseDto> GetMasterDatabaseCountsAsync();
    }
}
