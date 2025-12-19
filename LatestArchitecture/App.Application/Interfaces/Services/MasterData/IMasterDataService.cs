using App.Common.Models;

namespace App.Application.Interfaces.Services.MasterData
{
    public interface IMasterDataService
    {
        public Task<JsonModel> GetMasterDataAsync(string keys);
        public Task<JsonModel> GetMasterStateAsync(int countryId);
    }
}
