using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.SuperAdmin.Infrastructure.DBContext
{
    public interface IUnitOfWork
    {
        public Task<bool> CommitAsync();
    }
}
