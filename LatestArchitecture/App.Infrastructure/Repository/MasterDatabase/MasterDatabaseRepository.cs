using App.Application.Interfaces;
using Microsoft.Data.SqlClient;
using App.Application.Dto.MasterDatabase;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;
using App.Application.Dto.Common;
using Microsoft.EntityFrameworkCore;
using App.Application.Interfaces.Repositories.MasterDatabase;
using App.SharedConfigs.DBContext;

namespace App.Infrastructure.Repository.MasterDatabase
{
    public class MasterDatabaseRepository(MasterDbContext context, IDbConnectionFactory dbConnectionFactory, IMapper mapper,
        IConfiguration configuration) : BaseRepository<App.Domain.Entities.MasterDatabase.MasterDatabase>(context, dbConnectionFactory), IMasterDatabaseRepository
    {
        private readonly MasterDbContext _dbContext = context;
        private readonly IMapper _mapper = mapper;
        private string _masterConnectionString = string.Empty;
        private readonly IConfiguration _configuration = configuration;
        private readonly string _wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        public async Task<bool> CreateDatabaseAsync(MasterDatabaseDto dbDto, int userId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var masterDatabase = _mapper.Map<App.Domain.Entities.MasterDatabase.MasterDatabase>(dbDto);
                masterDatabase.CreatedBy = userId;
                masterDatabase.IsActive = true;

                _dbContext.MasterDatabase.Add(masterDatabase);
                await _dbContext.SaveChangesAsync();

                if (masterDatabase.Id <= 0)
                    return false; //Failure(StatusMessage.DatabaseCreationFailed);

                ///uncomment this to create actual db on server
                //if (!CreateDatabase(dbDto.DatabaseName))
                //    return false; //Failure(StatusMessage.DatabaseCreationFailed);

                //if (!ApplyDatabaseSchema(dbDto.DatabaseName))
                //    return false; // Failure(StatusMessage.ScriptExecutionFailed);

                await transaction.CommitAsync();

                return true; //Success(dbDto, StatusMessage.DatabaseSavedSuccessfully);
            }
            catch (Exception)
            {
                return false; //Failure($"Error: {ex.Message}. Transaction rolled back.");
            }
        }

        private bool CreateDatabase(string databaseName)
        {
            try
            {
                //_masterConnectionString = configuration.GetConnectionString("ApplicationConnection"); ///CreateOrganizationConnectionString();
                _masterConnectionString = configuration.GetConnectionString("LocalConnection"); //CreateOrganizationConnectionString();
                //_masterConnectionString = "Server=SMARTDATA-353\\SMARTDATA353;Database=DermatologyEMR;User ID=sa;Password=Secure@007;TrustServerCertificate=True";
                using (SqlConnection conn = new SqlConnection(_masterConnectionString))
                {
                    conn.Open();
                    string checkDbQuery = $"IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = '{databaseName}') BEGIN CREATE DATABASE [{databaseName}] END";
                    using (SqlCommand cmd = new SqlCommand(checkDbQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ApplyDatabaseSchema(string databaseName)
        {
            try
            {
                string _relativePath = _configuration["DatabaseSettings:SchemaScriptPath"];

                string scriptPath = Path.Combine(_wwwRootPath, _relativePath);
                if (!File.Exists(scriptPath))
                {
                    Console.WriteLine("SQL script file not found.");
                    return false;
                }

                //string newDbConnectionString = _masterConnectionString.Replace("Database=DermatologyEMR", $"Database={databaseName}");
                string newDbConnectionString = _masterConnectionString.Replace("Database=master", $"Database={databaseName}");
                string scriptContent = File.ReadAllText(scriptPath);

                using (SqlConnection conn = new SqlConnection(newDbConnectionString))//_masterConnectionString)) //newDbConnectionString))
                {
                    conn.Open();

                    // Split on "GO" statements
                    string[] scriptBatches = scriptContent
                        .Split(new string[] { "\r\nGO\r\n", "\nGO\n", "\r\nGO ", "\nGO " }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string batch in scriptBatches)
                    {
                        string trimmedBatch = batch.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmedBatch))
                        {
                            using (SqlCommand cmd = new SqlCommand(trimmedBatch, conn))
                            {
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {
                                    Console.WriteLine($"SQL Error: {ex.Message}\nBatch: {trimmedBatch}");
                                    return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<MasterDatabaseResponseDto>> GetAllMasterDatabaseAsync(MasterDatabaseFilterDto filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SearchTerm", filter.SearchTerm ?? string.Empty, DbType.String);
            parameters.Add("@ParentOrganizationID", filter.ParentOrganizationID, DbType.Int32);
            parameters.Add("@PageNumber", filter.PageNumber, DbType.Int32);
            parameters.Add("@PageSize", filter.PageSize, DbType.Int32);
            parameters.Add("@SortColumn", filter.SortColumn, DbType.String);
            parameters.Add("@SortOrder", filter.SortOrder, DbType.String);

            return (List<MasterDatabaseResponseDto>)await _dbConnection.QueryAsync<MasterDatabaseResponseDto>(App.Common.Constant.SqlMethod.MST_GetAllMasterDatabase, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<MasterDatabaseResponseForDropdownDto>> GetAllMasterDatabaseDropdownAsync()
        {
            return await _dbContext.MasterDatabase
                .AsNoTracking()
                .Where(a => a.IsActive && !a.IsDeleted)
                .Select(a => new MasterDatabaseResponseForDropdownDto
                {
                    DatabaseID = a.Id,
                    DatabaseName = a.DatabaseName
                }).ToListAsync();
        }

        public async Task<MasterDatabaseResponseDto> GetMasterDatabaseByIdAsync(int databaseId)
        {
            return await _dbContext.MasterDatabase
                .AsNoTracking()
                .Where(a => a.Id == databaseId && a.IsActive && !a.IsDeleted)
                .Select(a => new MasterDatabaseResponseDto
                {
                    DatabaseID = a.Id,
                    DatabaseName = a.DatabaseName,
                    Password = a.Password,
                    ServerName = a.ServerName,
                    UserName = a.UserName,
                    IsActive = a.IsActive,
                    IsCentralized = a.IsCentralized,
                    ParentOrganizationID = a.ParentOrganizationID
                })
                .FirstOrDefaultAsync();
        }

        public async Task<MasterDatabaseCountsResponseDto> GetMasterDatabaseCountsAsync()
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<MasterDatabaseCountsResponseDto>(App.Common.Constant.SqlMethod.MDB_GetMasterDatabaseCounts, null, commandType: CommandType.StoredProcedure);
        }

    }
}
