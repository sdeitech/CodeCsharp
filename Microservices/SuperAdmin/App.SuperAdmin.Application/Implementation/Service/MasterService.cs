using App.SuperAdmin.Application.Dto;
using App.SuperAdmin.Application.Interfaces.Repositories;
using App.SuperAdmin.Application.Interfaces.Service;
using App.SuperAdmin.Domain.Entities;
using App.SuperAdmin.Infrastructure.DBContext;

namespace App.SuperAdmin.Infrastructure.Service
{
    public class MasterService(IMasterRepository masterRepository, IUnitOfWork unitOfWork) : IMasterService
    {
        private readonly IMasterRepository _masterRepository = masterRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<int> CreateMasterDataAsync(MasterDto masterDto)
        {
            var DropDownDetails = new DropDownDetails(
                                       masterDto.Name,
                                       masterDto.MasterDropDownId,
                                       1,
                                       masterDto.IsActive);
            _masterRepository.Add(DropDownDetails);
            //await _unitOfWork.CommitAsync();
            return DropDownDetails.Id;
        }

        public async Task<List<DropDownDetails>> GetAllAsync(int masterDropDownId)
        {
            var masterDetails = await _masterRepository.GetAllAsync(masterDropDownId);
            return [.. masterDetails];
        }

        public Task<MasterDto?> GetMasterDetailByMasterIdAsync(int masterId)
        {
            throw new NotImplementedException();
        }
    }
}
