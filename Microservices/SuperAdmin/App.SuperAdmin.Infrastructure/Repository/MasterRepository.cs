using App.Master.DBContext;
using App.SuperAdmin.Application.Interfaces.Repositories;
using App.SuperAdmin.Domain.Entities;
using App.SuperAdmin.Infrastructure.DBContext;

namespace App.SuperAdmin.Infrastructure.Repository
{
    public class MasterRepository(MasterDbContext context, IDbConnectionFactory dbConnectionFactory) : BaseRepository<DropDownDetails>(context, dbConnectionFactory), IMasterRepository
    {
        public Task<IEnumerable<DropDownDetails>> GetAllAsync(int masterId)
        {
            throw new NotImplementedException();
        }

        public Task<DropDownDetails> GetByMasterIdAsync(int masterId)
        {
            throw new NotImplementedException();
        }
    }
}
