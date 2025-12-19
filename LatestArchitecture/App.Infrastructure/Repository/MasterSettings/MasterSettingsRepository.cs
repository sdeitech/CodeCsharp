using App.Application.Dto.Common;
using App.Application.Dto.MasterSetting;
using App.Application.Dto.MasterSystemSettings;
using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.DynamicQuestionnaire;
using App.Application.Interfaces.Repositories.MasterSettings;
using App.Common.Constant;
using App.Common.Models;
using App.Domain.Entities;
using App.Domain.Entities.MasterSettings;
using App.Infrastructure.DBContext;
using App.SharedConfigs.DBContext;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace App.Infrastructure.Repository.MasterSettings
{
    /// <summary>
    /// Repository implementation for managing MasterSetting entities.
    /// Inherits from <see cref="BaseRepository{MasterSetting}"/> to provide 
    /// common repository functionality and implements <see cref="IMasterSettingsRepository"/>.
    /// </summary>
    /// 
    public class MasterSettingsRepository(MasterDbContext context, IDbConnectionFactory dbConnectionFactory)
    : BaseRepository<MasterSetting>(context, dbConnectionFactory), IMasterSettingsRepository
    {
        private readonly MasterDbContext _dbContext = context;


        /// <summary>
        /// Your repository methods here...
        /// </summary>


        /// <summary>
        /// Initializes a new instance of the <see cref="MasterSettingsRepository"/> class.
        /// </summary>
        /// <param name="context">The database context for EF Core operations.</param>
        /// <param name="dbConnectionFactory">Factory for creating database connections (for Dapper/raw SQL if needed).</param>


        /// <summary>
        /// Saves a <see cref="MasterSetting"/> entity asynchronously.  
        /// - If the entity <see cref="MasterSetting.Id"/> is 0, it will be added as a new record.  
        /// - Otherwise, the entity will be updated.  
        /// </summary>
        /// <param name="masterSetting">The <see cref="MasterSetting"/> entity to be saved.</param>
        /// <returns>
        /// The saved <see cref="MasterSetting"/> entity, including updated values (e.g., generated Id).
        /// </returns>
        public async Task<MasterSetting> SaveAsync(MasterSetting masterSetting)
        {
            // Check if entity is new (Id = 0 means not yet persisted in DB).
            if (masterSetting.Id == 0)
            {
                // Add new entity to the database
                await _dbContext.AddAsync(masterSetting);
            }
            else
            {
                // Update the existing entity in the database.
                _dbContext.Update(masterSetting);
            }
            // Commit changes to the database.
            await _dbContext.SaveChangesAsync();
            // Return the saved entity with updated values (like Id).
            return masterSetting;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<MasterSettingsDto>> GetAllAsync(FilterDto filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SearchTerm", filter.SearchTerm);
            parameters.Add("@SortColumn", filter.SortColumn);
            parameters.Add("@SortOrder", filter.SortOrder ?? "DESC");
            parameters.Add("@PageNumber", filter.PageNumber <= 0 ? 1 : filter.PageNumber);
            parameters.Add("@PageSize", filter.PageSize <= 0 ? 10 : filter.PageSize);

            return await _dbConnection.QueryAsync<MasterSettingsDto>(
                SqlMethod.ADM_MasterSetting_GetAll,
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<MasterSetting> GetByNameAsync(string name)
        {
            return await _dbContext.MasterSetting
                .FirstOrDefaultAsync(s => !s.IsDeleted && s.SettingName.ToLower() == name.ToLower());
        }

        public async Task<List<MasterSettingNames>> GetAllSettingNamesAsync()
        {
            return await _dbContext.Set<MasterSettingNames>()
                     .ToListAsync();
        }


    }
}
