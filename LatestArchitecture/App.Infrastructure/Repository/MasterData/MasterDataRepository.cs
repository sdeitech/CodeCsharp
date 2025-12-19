using App.Application.Interfaces;
using App.Application.Dto.MasterCountry;
using System.Data;
using Microsoft.EntityFrameworkCore;
using App.SharedConfigs.DBContext;
using Dapper;
using App.Common.Constant;
using App.Application.Interfaces.Repositories.MasterData;

namespace App.Infrastructure.Repository.MasterData
{
    public class MasterDataRepository(MasterDbContext context, IDbConnectionFactory dbConnectionFactory) : BaseRepository<App.Domain.Entities.MasterData.MasterCountry>(context, dbConnectionFactory), IMasterDataRepository
    {
        private readonly MasterDbContext _context = context;

        public async Task<Dictionary<string, object>> GetMasterDataAsync(string keys)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Keys", keys, DbType.String);

            using var multi = await _dbConnection.QueryMultipleAsync(
                SqlMethod.MST_GetMasterData,
                parameters,
                commandType: CommandType.StoredProcedure);

            var result = new Dictionary<string, object>();

            if (keys.Contains("country", StringComparison.OrdinalIgnoreCase))
                result["country"] = multi.Read().ToList();

            //if (keys.Contains("state", StringComparison.OrdinalIgnoreCase))
            //    result["state"] = multi.Read().ToList();

            if (keys.Contains("contacttype", StringComparison.OrdinalIgnoreCase))
                result["contacttype"] = multi.Read().ToList();
            
            if (keys.Contains("database", StringComparison.OrdinalIgnoreCase))
                result["database"] = multi.Read().ToList();
            
            if (keys.Contains("organization", StringComparison.OrdinalIgnoreCase))
                result["organization"] = multi.Read().ToList();

            return result;
        }

        public async Task<List<MasterStateResponseDto>> GetMasterStateAsync(int countryId)
        {
            return await _context.MasterState
                .AsNoTracking()
                .Where(a => a.CountryID == countryId && a.IsActive && !a.IsDeleted)
                .Select(a => new MasterStateResponseDto
                {
                    StateID = a.Id,
                    StateName = a.StateName
                })
                .ToListAsync();
        }
    }
}
