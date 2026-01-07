using App.Application.Interfaces;
using Dapper;
using System.Data;
using App.Common.Constant;
using App.Application.Dto.Organization;
using App.Application.Dto.StorageConfiguration;
using App.Common.Models;
using App.Application.Dto.Common;
using App.Application.Interfaces.Repositories.Organization;
using App.SharedConfigs.DBContext;

namespace App.Infrastructure.Repository.Organization
{
    public class OrganizationRepository(MasterDbContext context, IDbConnectionFactory dbConnectionFactory) : BaseRepository<App.Domain.Entities.Organization.Organization>(context, dbConnectionFactory), IOrganizationRepository
    {
        public async Task<JsonModel> CreateOrganizationAsync(OrganizationDto orgDto, int userId)
        {
            var parameters = BuildOrganizationParameters(orgDto, userId);
            await _dbConnection.ExecuteAsync(SqlMethod.ORG_SaveOrUpdateOrganization, parameters, commandType: CommandType.StoredProcedure);

            //bool success = parameters.Get<bool>("@Success");
            string message = parameters.Get<string>("@Message");
            int organizationId = parameters.Get<int>("@OrganizationID");
            return new JsonModel
            {
                Data = organizationId,
                Message = message
            };
        }

        public async Task<JsonModel> UpdateOrganizationAsync(OrganizationDto orgDto, int userId)
        {
            var parameters = BuildOrganizationParameters(orgDto, userId, includeId: true);
            await _dbConnection.ExecuteAsync(SqlMethod.ORG_SaveOrUpdateOrganization, parameters, commandType: CommandType.StoredProcedure);

            string message = parameters.Get<string>("@Message");
            int organizationId = parameters.Get<int>("@OrganizationID");
            return new JsonModel
            {
                Data = organizationId,
                Message = message
            };
        }

        private static DynamicParameters BuildOrganizationParameters(OrganizationDto orgDto, int userId, bool includeId = false)
        {
            var parameters = new DynamicParameters();

            // OrganizationID: Input for update, Output for create
            if (includeId)
            {
                parameters.Add("@OrganizationID", orgDto.OrganizationID, DbType.Int32, ParameterDirection.InputOutput);
            }
            else
            {
                parameters.Add("@OrganizationID", dbType: DbType.Int32, direction: ParameterDirection.Output);
            }

            parameters.Add("@OrganizationName", orgDto.OrganizationName, DbType.String);
            parameters.Add("@LogoLocalPath", orgDto.LogoLocalPath, DbType.String);
            parameters.Add("@LogoBlobPath", orgDto.LogoBlobPath, DbType.String);
            parameters.Add("@LogoAWSPath", orgDto.LogoAWSPath, DbType.String);
            parameters.Add("@FavIconLocalPath", orgDto.FavIconLocalPath, DbType.String);
            parameters.Add("@FavIconBlobPath", orgDto.FavIconBlobPath, DbType.String);
            parameters.Add("@FavIconAWSPath", orgDto.FavIconAWSPath, DbType.String);
            parameters.Add("@ContactPersonFirstName", orgDto.ContactPersonFirstName, DbType.String);
            parameters.Add("@ContactPersonLastName", orgDto.ContactPersonLastName, DbType.String);
            parameters.Add("@ContactPersonPhone", orgDto.ContactPersonPhone, DbType.String);
            parameters.Add("@ContactPersonEmail", orgDto.ContactPersonEmail, DbType.String);
            parameters.Add("@Address", orgDto.Address, DbType.String);
            parameters.Add("@City", orgDto.City, DbType.String);
            parameters.Add("@StateID", orgDto.StateID, DbType.Int32);
            parameters.Add("@CountryID", orgDto.CountryID, DbType.Int32);
            parameters.Add("@ZipCode", orgDto.ZipCode, DbType.String);
            parameters.Add("@DatabaseID", orgDto.DatabaseID, DbType.Int32); // Needed for create
            parameters.Add("@UserID", userId, DbType.Int32); // TO BE USED FROM TOKEN
            parameters.Add("@ContactTypeID", orgDto.ContactTypeID, DbType.Int32);
            parameters.Add("@DomainURL", orgDto.DomainURL, DbType.String);
            parameters.Add("@SubDomainName", orgDto.SubDomainName, DbType.String);

            // Output parameters
            parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@Message", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            return parameters;
        }

        public async Task<List<OrganizationReponseDto>> GetAllOrganizationsAsync(FilterDto filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OrganizationName", filter.SearchTerm, DbType.String);
            parameters.Add("@PageNumber", filter.PageNumber, DbType.Int32);
            parameters.Add("@PageSize", filter.PageSize, DbType.Int32);
            parameters.Add("@SortColumn", filter.SortColumn, DbType.String);
            parameters.Add("@SortOrder", filter.SortOrder, DbType.String);

            return (List<OrganizationReponseDto>)await _dbConnection.QueryAsync<OrganizationReponseDto>(SqlMethod.ORG_GetAllOrganizations, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<OrganizationReponseDto> GetOrganizationByIdAsync(int organizationId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OrganizationID", organizationId, DbType.Int32);

            return await _dbConnection.QueryFirstOrDefaultAsync<OrganizationReponseDto>(SqlMethod.ORG_GetOrganizationByID, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<OrganizationCardStatisticsDto> GetCardStatisticsAsync()
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<OrganizationCardStatisticsDto>(SqlMethod.ORG_GetCardStatistics, commandType: CommandType.StoredProcedure);
        }

        #region Storage Configuration Methods

        public async Task<StorageConfigurationResponseDto?> GetStorageConfigurationAsync(int organizationId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OrganizationId", organizationId);

            var result = await _dbConnection.QueryFirstOrDefaultAsync<StorageConfigurationResponseDto>(
                SqlMethod.ORG_GetStorageConfiguration,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<StorageConfigurationResponseDto> SaveStorageConfigurationAsync(int organizationId, string storageType, AwsStorageConfigDto? awsConfig, AzureBlobStorageConfigDto? azureConfig, int userId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@OrganizationId", organizationId);
                parameters.Add("@StorageType", storageType);
                
                // AWS S3 Parameters
                parameters.Add("@AwsProfile", awsConfig?.Profile);
                parameters.Add("@AwsRegion", awsConfig?.Region);
                parameters.Add("@AwsAccessKey", awsConfig?.AccessKey);
                parameters.Add("@AwsSecretKey", awsConfig?.SecretKey);
                parameters.Add("@AwsBucketName", awsConfig?.BucketName);
                
                // Azure Blob Storage Parameters
                parameters.Add("@AzureConnectionString", azureConfig?.ConnectionString);
                parameters.Add("@AzureContainerName", azureConfig?.ContainerName);
                parameters.Add("@AzureBlobStorageUrl", azureConfig?.BlobStorageUrl);
                
                parameters.Add("@UserId", userId);
                parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);
                parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync(SqlMethod.ORG_SaveStorageConfiguration, parameters, commandType: CommandType.StoredProcedure);

                bool success = parameters.Get<bool>("@Success");
                string message = parameters.Get<string>("@Message");
                int id = parameters.Get<int>("@Id");

                if (!success)
                {
                    throw new Exception(message);
                }

                // Get the complete response
                return await GetStorageConfigurationAsync(organizationId) ?? new StorageConfigurationResponseDto
                {
                    Id = id,
                    OrganizationId = organizationId,
                    StorageType = storageType,
                    AwsProfile = awsConfig?.Profile,
                    AwsRegion = awsConfig?.Region,
                    AwsAccessKey = awsConfig?.AccessKey,
                    AwsSecretKey = awsConfig?.SecretKey,
                    AwsBucketName = awsConfig?.BucketName,
                    AzureConnectionString = azureConfig?.ConnectionString,
                    AzureContainerName = azureConfig?.ContainerName,
                    AzureBlobStorageUrl = azureConfig?.BlobStorageUrl,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteStorageConfigurationAsync(int organizationId, int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OrganizationId", organizationId);
            parameters.Add("@UserId", userId);
            parameters.Add("@Success", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@Message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

            await _dbConnection.ExecuteAsync(SqlMethod.ORG_DeleteStorageConfiguration, parameters, commandType: CommandType.StoredProcedure);

            bool success = parameters.Get<bool>("@Success");
            string message = parameters.Get<string>("@Message");

            if (!success)
            {
                throw new Exception(message);
            }

            return success;
        }

        public async Task<List<StorageConfigurationResponseDto>> GetAllStorageConfigurationsAsync()
        {
            var result = await _dbConnection.QueryAsync<StorageConfigurationResponseDto>(
                SqlMethod.ORG_GetAllStorageConfigurations,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }


        #endregion
    }
}
