
using App.Application.Interfaces;
using App.Infrastructure.DBContext;
using App.SharedConfigs.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace App.Infrastructure.Repository
{
    public class MasterRepositoryBase<T> : IMasterRepositoryBase<T> where T : class
    {
        private readonly ApplicationDbContext context;
        private DbSet<T> entities;
        string errorMessage = string.Empty;
        public MasterRepositoryBase(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }


        /// <summary>
        /// Gets the first entity found or default value.
        /// </summary>
        /// <param name="filter">Filter expression for filtering the entities.</param>
        /// <param name="include">Include for eager-loading.</param>
        /// <returns></returns>
        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> filter,
                                          params Expression<Func<T, object>>[] include)
        {
            IQueryable<T> dbQuery = SelectQuery(filter, include);
            return dbQuery.AsNoTracking().FirstOrDefault();
        }

        /// <summary>
        /// Creates the specified entity/entities.
        /// </summary>
        /// <param name="entity">Single entity.</param>
        /// <param name="entities">Multiple entities.</param>
        public virtual void Create(T entity, params T[] entities)
        {
            EntityState state = EntityState.Added;
            SetEntityState(state, entity, entities);
        }


        /// <summary>
        /// Creates the specified entity/entities.
        /// </summary>
        /// <param name="entities">Multiple entities.</param>
        public void Create(T[] entities)
        {
            EntityState state = EntityState.Added;
            SetEntityStateForArray(state, entities);
        }

        /// <summary>
        /// Updates the specified entity/entities.
        /// </summary>
        /// <param name="entity">Single entity.</param>
        /// <param name="entities">Multiple entities.</param>
        public virtual void Update(T entity, params T[] entities)
        {
            EntityState state = EntityState.Modified;
            SetEntityState(state, entity, entities);
        }

        /// <summary>
        /// Updates the specified entity/entities.
        /// </summary>
        /// <param name="entities">Multiple entities.</param>
        public virtual void Update(T[] entities)
        {
            EntityState state = EntityState.Modified;
            SetEntityStateForArray(state, entities);
        }

        /// <summary>
        /// Deletes the specified entity/entities.
        /// </summary>
        /// <param name="entity">Single entity.</param>
        /// <param name="entities">Multiple entities.</param>
        public virtual void Delete(T entity, params T[] entities)
        {
            EntityState state = EntityState.Deleted;
            SetEntityState(state, entity, entities);
        }

        /// <summary>
        /// Deletes the entity by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public virtual void Delete(object id)
        {
            T entity = CreateDbSet<T>().Find(id);
            EntityState state = EntityState.Deleted;
            SetEntityState(state, entity);
        }

        /// <summary>
        /// Deletes multiple entities which are found using filter.
        /// </summary>
        /// <param name="filter">Filter expression for filtering the entities.</param>
        public virtual void Delete(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> dbQuery = SelectQuery(filter);
            dbQuery.AsNoTracking().ToList().ForEach(item => context.Entry(item).State = EntityState.Deleted);
        }

        /// <summary>
        /// Saves the changes to the database.
        /// </summary>
        /// <returns>Number of rows affected.</returns>
        public int SaveChanges()
        {
            try
            {
                int recordsAffected = context.SaveChanges();
                //this.Dispose();  // uncommented by kundan for memeory release 
                return recordsAffected;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Fetch all records .
        /// </summary>
        /// <returns></returns>
        /// 
        public IQueryable<T> FetchAll()
        {
            return GetAll();
        }

        public IQueryable<T> GetAll()
        {
            return context.Set<T>();
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> exp)
        {
            return context.Set<T>().Where(exp);
        }

        public T Get(Expression<Func<T, bool>> exp)
        {
            return context.Set<T>().Where(exp).FirstOrDefault();
        }

        public virtual T GetByID(object id)
        {
            return CreateDbSet<T>().Find(id);
        }

        #region Stored Procedures Factory
        ////When you expect a model back (async)
        //public async Task<IList<T>> ExecWithStoreProcedureAsync(string query, params object[] parameters)
        //{
        //    // EF 6
        //    //context.Database.SqlQuery<T>(query, parameters).ToListAsync();
        //    // EF Core
        //    return await entities.FromSql(query, parameters).ToListAsync();
        //}

        ////When you expect a model back
        //public IEnumerable<T> ExecWithStoreProcedure(string query)
        //{
        //    // EF 6
        //    //_context.Database.SqlQuery<T>(query);
        //    // EF Core
        //    return entities.FromSql(query);
        //}

        ////When you expect a model back
        //public IEnumerable<T> ExecWithStoreProcedureWithParameters(string query, params object[] parameters)
        //{
        //    // EF 6
        //    //_context.Database.SqlQuery<T>(query, parameters);
        //    // EF Core
        //    return entities.FromSql(query, parameters);
        //}

        ////When you expect a model back
        //public T ExecWithStoreProcedureWithParametersForModel(string query, params object[] parameters)
        //{
        //    // EF 6
        //    //IEnumerable<TResult> dbQuery = _context.Database.SqlQuery<TResult>(query, parameters);
        //    //return dbQuery.FirstOrDefault();
        //    // EF Core
        //    IEnumerable<T> dbQuery = entities.FromSql(query, parameters);
        //    return dbQuery.FirstOrDefault();
        //}

        //// Fire and forget (async)
        //public async Task ExecuteWithStoreProcedureAsync(string query, params object[] parameters)
        //{
        //    // EF 6
        //    //await _context.Database.ExecuteSqlCommandAsync(query, parameters);
        //    // EF Core
        //    await context.Database.ExecuteSqlCommandAsync(query, default(CancellationToken), parameters);
        //}

        //// Fire and get no. of row inserted
        //public int ExecuteWithStoreProcedure(string query, params object[] parameters)
        //{
        //    return context.Database.ExecuteSqlCommand(query, parameters);
        //}
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MasterRepositoryBase{TEntity}"/> class.
        /// </summary>
        ~MasterRepositoryBase()
        {
            Dispose(false);
        }
        protected DbSet<TEntity> CreateDbSet<TEntity>() where TEntity : class
        {
            return context.Set<TEntity>();
        }

        #region Private Methods
        private IQueryable<T> SelectQuery(Expression<Func<T, bool>> filter, Expression<Func<T, object>>[] include = null)
        {
            IQueryable<T> dbQuery = CreateDbSet<T>();

            if (filter != null)
            {
                dbQuery = dbQuery.Where(filter);
            }

            if (include != null)
            {
                dbQuery = include.Aggregate(dbQuery, (a, b) => a.Include(b));
            }
            return dbQuery;
        }

        private void SetEntityState(EntityState state, T entity, params T[] entities)
        {
            try
            {
                context.Entry(entity).State = state;
                foreach (T item in entities)
                {
                    context.Entry(item).State = state;
                }
            }
            catch (Exception)
            {
            }
        }

        private void SetEntityStateForArray(EntityState state, T[] entities)
        {
            try
            {
                foreach (T item in entities)
                {
                    context.Entry(item).State = state;
                }
            }
            catch (Exception)
            {
            }
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }
        }
        #endregion Private Methods
    }
}
