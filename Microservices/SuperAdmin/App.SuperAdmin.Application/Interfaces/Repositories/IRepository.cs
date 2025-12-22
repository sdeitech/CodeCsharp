using App.SuperAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.SuperAdmin.Application.Interfaces.Repositories
{
    public interface IRepository<TEntity> : IDisposable where TEntity : BaseEntity
    {
        void Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        IQueryable<TEntity> GetAll();

        IQueryable<TEntity> GetAllNoTracking();

        Task<TEntity?> GetByIdAsync(Guid id);

        void Update(TEntity entity);

        Task<bool> ExistsAsync(int id);
        public void Remove(TEntity entity, bool hardDelete = false);
        void RemoveRange(IEnumerable<TEntity> entities, bool hardDelete = false);
    }
}
