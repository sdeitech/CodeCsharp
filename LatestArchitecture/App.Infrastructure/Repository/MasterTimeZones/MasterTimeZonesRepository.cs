using App.Application.Interfaces;
using App.Application.Interfaces.Repositories.MasterSystemSettings;
using App.Application.Interfaces.Repositories.MasterTimeZone;
using App.Domain.Entities.MasterSystemSettings;
using App.Domain.Entities.MasterTimeZone;
using App.SharedConfigs.DBContext;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Repository.MasterTimeZone
{
    public class MasterTimeZonesRepository : BaseRepository<MasterTimeZones>, IMasterTimeZonesRepository
    {
        private readonly MasterDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterSystemSettingsRepository"/> class.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        /// <param name="dbConnectionFactory">Factory for creating database connections (optional, for Dapper/raw SQL).</param>
        public MasterTimeZonesRepository(MasterDbContext context, IDbConnectionFactory dbConnectionFactory)
            : base(context, dbConnectionFactory)
        {
            _dbContext = context;  // ✅ assign here
        }

        public async Task<string> GetActiveTimeZoneAsync(CancellationToken ct = default)
        {
            if (_dbContext == null) return "UTC";

            // Query the MasterSetting table for the active SystemTimeZone
            var timeZone = await _dbContext.MasterSetting
                .Where(s => s.IsActive && s.SettingName == "SystemTimeZone")
                .OrderByDescending(s => s.Id) // optional: pick the latest if multiple
                .Select(s => s.SettingValue)
                .FirstOrDefaultAsync(ct);

            // Fallback to UTC if no value is found
            return string.IsNullOrEmpty(timeZone) ? "UTC" : timeZone;
        }


        #region Get All
        /// <summary>
        /// Retrieves all MasterSystemSetting records from the database.
        /// </summary>
        /// <returns>A list of <see cref="MasterTimeZones"/> entities.</returns>
        public async Task<List<MasterTimeZones>> GetAllAsync()
        {
            return await _dbContext.Set<MasterTimeZones>().ToListAsync();
        }
        #endregion
    }



}

