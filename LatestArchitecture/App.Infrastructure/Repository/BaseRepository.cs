using App.Application.Interfaces;
using App.Application.Interfaces.Repositories;
using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace App.Infrastructure.Repository
{
    public class BaseRepository<TEntity> : DbConnectionRepositoryBase, IRepository<TEntity> where TEntity : BaseEntity
    {
        private const string IsDeletedPropertyName = "IsDeleted";
        private const string DeletedAtPropertyName = "DeletedAt";
        
        protected readonly DbSet<TEntity> DbSet;

        protected BaseRepository(DbContext context, IDbConnectionFactory dbConnectionFactory)
            : base(context, dbConnectionFactory)
        {
            DbSet = _dbContext.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var query = DbSet.AsQueryable();
            
            // Check if entity has IsDeleted property for soft delete filtering
            if (HasIsDeletedProperty())
            {
                query = query.Where(e => !EF.Property<bool>(e, IsDeletedPropertyName));
            }
            
            return await query.AnyAsync(entity => entity.Id == id);
        }

        public IQueryable<TEntity> GetAll()
        {
            var query = DbSet.AsQueryable();
            
            // Filter out soft deleted entities if the entity supports soft delete
            if (HasIsDeletedProperty())
            {
                query = query.Where(e => !EF.Property<bool>(e, IsDeletedPropertyName));
            }
            
            return query;
        }

        public IQueryable<TEntity> GetAllNoTracking()
        {
            var query = DbSet.AsNoTracking();
            
            // Filter out soft deleted entities if the entity supports soft delete
            if (HasIsDeletedProperty())
            {
                query = query.Where(e => !EF.Property<bool>(e, IsDeletedPropertyName));
            }
            
            return query;
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            var query = DbSet.AsQueryable();
            
            // Filter out soft deleted entities if the entity supports soft delete
            if (HasIsDeletedProperty())
            {
                query = query.Where(e => !EF.Property<bool>(e, IsDeletedPropertyName));
            }
            
            return await query.FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public void Remove(TEntity entity, bool hardDelete = false)
        {
            if (hardDelete)
            {
                DbSet.Remove(entity);
            }
            else
            {
                // Perform soft delete if entity supports it
                if (HasIsDeletedProperty())
                {
                    typeof(TEntity).GetProperty(IsDeletedPropertyName)?.SetValue(entity, true);
                    typeof(TEntity).GetProperty(DeletedAtPropertyName)?.SetValue(entity, DateTime.UtcNow);
                    DbSet.Update(entity);
                }
                else
                {
                    // Fallback to hard delete if soft delete is not supported
                    DbSet.Remove(entity);
                }
            }
        }

        public void RemoveRange(IEnumerable<TEntity> entities, bool hardDelete = false)
        {
            if (hardDelete)
            {
                DbSet.RemoveRange(entities);
                return;
            }

            foreach (var entity in entities)
            {
                Remove(entity, hardDelete);
            }
        }

        public void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

        private static bool HasIsDeletedProperty()
        {
            return typeof(TEntity).GetProperty(IsDeletedPropertyName) != null;
        }
    }
}
