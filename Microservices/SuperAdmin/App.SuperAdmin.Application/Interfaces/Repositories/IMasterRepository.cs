using App.SuperAdmin.Domain.Entities;

namespace App.SuperAdmin.Application.Interfaces.Repositories
{
    public interface IMasterRepository : IRepository<DropDownDetails>
    {
        Task<DropDownDetails> GetByMasterIdAsync(int masterId);
        //dapper
        Task<IEnumerable<DropDownDetails>> GetAllAsync(int masterId);
    }
}
