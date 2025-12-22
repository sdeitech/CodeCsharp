using App.SuperAdmin.Application.Interfaces.Repositories;
using App.SuperAdmin.Domain.Entities;
using App.SuperAdmin.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.SuperAdmin.Infrastructure.Repository
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly DbContext _dbContext;
        protected readonly DbSet<TEntity> DbSet;
        private readonly string _connectionString;
        private readonly string _provider;
        protected readonly IDbConnection _dbConnection;

        protected BaseRepository(DbContext context, IDbConnectionFactory dbConnectionFactory)
        {
            _dbContext = context;
            _connectionString = context.Database.GetDbConnection().ConnectionString;
            _provider = context.Database.ProviderName ?? throw new InvalidOperationException("Database provider name cannot be null.");
            _dbConnection = dbConnectionFactory.CreateConnection(_connectionString, _provider);
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
            return await DbSet.AnyAsync(entity => entity.Id == id);
        }

        public IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        public IQueryable<TEntity> GetAllNoTracking()
        {
            return DbSet.AsNoTracking();
        }

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public void Remove(TEntity entity, bool hardDelete = false)
        {
            if (hardDelete)
            {
                DbSet.Remove(entity);
            }
            else
            {
                //entity.Delete();
                DbSet.Update(entity);
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
                //entity.Delete();
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
    }
}
