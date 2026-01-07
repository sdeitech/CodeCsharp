using App.Common.Models;
using System.Net;
using App.Application.Dto.MasterCountry;
using App.Application.Interfaces.Repositories.MasterData;
using App.Application.Interfaces.Services.MasterData;
using App.Common.Constant;

namespace App.Application.Service.MasterData
{
    public class MasterDataService(IMasterDataRepository masterRepository) : IMasterDataService
    {
        private readonly IMasterDataRepository _masterRepository = masterRepository;

        public async Task<JsonModel> GetMasterDataAsync(string keys)
        {
            Dictionary<string, object> data = await _masterRepository.GetMasterDataAsync(keys);

            return data.Any()
            ? new JsonModel { Data = data, StatusCode = (int)HttpStatusCode.OK }
            : new JsonModel { Message = StatusMessage.InternalServerError, StatusCode = (int)HttpStatusCode.InternalServerError };
        }

        public async Task<JsonModel> GetMasterStateAsync(int countryId)
        {
            List<MasterStateResponseDto> data = await _masterRepository.GetMasterStateAsync(countryId);

            return data.Any()
            ? new JsonModel { Data = data, StatusCode = (int)HttpStatusCode.OK }
            : new JsonModel { Message = StatusMessage.InternalServerError, StatusCode = (int)HttpStatusCode.InternalServerError };
        }
    }
}
