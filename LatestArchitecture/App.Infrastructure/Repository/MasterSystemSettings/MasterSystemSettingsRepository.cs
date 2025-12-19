
using App.Application.Dto.Common;
using App.Application.Dto.MasterAdmin;
using App.Application.Dto.MasterSystemSettings;
using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.MasterSystemSettings;
using App.Common.Constant;
using App.Domain.Entities.MasterSystemSettings;
using App.Infrastructure.DBContext;
using App.SharedConfigs.DBContext;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Repository.MasterSystemSettings
{
    /// <summary>
    /// Repository for performing CRUD operations on MasterSystemSetting entities.
    /// Inherits from BaseRepository and implements IMasterSystemSettingsRepository.
    /// </summary>
    public class MasterSystemSettingsRepository(MasterDbContext context, IDbConnectionFactory dbConnectionFactory)
    :BaseRepository<MasterSystemSetting>(context, dbConnectionFactory), IMasterSystemSettingsRepository
    {
        private readonly MasterDbContext _dbContext = context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterSystemSettingsRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        /// <param name="dbConnectionFactory">Factory for creating database connections (optional, for Dapper/raw SQL).</param>
      
        // Your repository methods...
        #region Get All
        public async Task<IEnumerable<MasterSystemSettingsDto>> GetAllAsync(FilterDto filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SearchTerm", filter.SearchTerm);
            parameters.Add("@SortColumn", filter.SortColumn);
            parameters.Add("@SortOrder", filter.SortOrder ?? "DESC");
            parameters.Add("@PageNumber", filter.PageNumber <= 0 ? 1 : filter.PageNumber);
            parameters.Add("@PageSize", filter.PageSize <= 0 ? 10 : filter.PageSize);

            return await _dbConnection.QueryAsync<MasterSystemSettingsDto>(
                SqlMethod.ADM_MasterSystemSetting_GetAll,
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<MasterSystemSetting> GetByNameAsync(string name)
        {
            return await _dbContext.MasterSystemSetting
                .FirstOrDefaultAsync(s => !s.IsDeleted && s.SystemSettingName.ToLower() == name.ToLower());
        }

        #endregion

        #region Save / Update
        public async Task<MasterSystemSetting> SaveAsync(MasterSystemSetting masterSystemSetting)
        {
            if (masterSystemSetting.Id == 0)
            {
                await _dbContext.AddAsync(masterSystemSetting);
            }
            else
            {
                _dbContext.Update(masterSystemSetting);
            }

            await _dbContext.SaveChangesAsync();
            return masterSystemSetting;
        }
        #endregion

        #region Save Changes
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        
        #endregion
    }





}
