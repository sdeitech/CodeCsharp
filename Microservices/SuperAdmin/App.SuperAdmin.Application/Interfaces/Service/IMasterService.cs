using App.SuperAdmin.Application.Dto;
using App.SuperAdmin.Domain.Entities;

namespace App.SuperAdmin.Application.Interfaces.Service
{
    public interface IMasterService
    {
        public Task<MasterDto?> GetMasterDetailByMasterIdAsync(int masterId);
        public Task<int> CreateMasterDataAsync(MasterDto masterDto);
        public Task<List<DropDownDetails>> GetAllAsync(int masterDropDownId);
    }
}
