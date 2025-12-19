using App.Application.Dto.MasterCountry;

namespace App.Application.Interfaces.Repositories.MasterData
{
    public interface IMasterDataRepository : IRepository<Domain.Entities.MasterData.MasterCountry>
    {
        public Task<Dictionary<string, object>> GetMasterDataAsync(string keys);
        public Task<List<MasterStateResponseDto>> GetMasterStateAsync(int countryId);
    }
}
