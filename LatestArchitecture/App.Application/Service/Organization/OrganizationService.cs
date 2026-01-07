using App.Application.Dto.Common;
using App.Application.Dto.Organization;
using App.Application.Dto.StorageConfiguration;
using App.Application.Interfaces.Repositories.AuditLogs;
using App.Application.Interfaces.Repositories.Organization;
using App.Application.Interfaces.Services.AwsServices;
using App.Application.Interfaces.Services.AzureBlobStorageService;
using App.Application.Interfaces.Services.Images;
using App.Application.Interfaces.Services.Organization;
using App.Common.Constant;
using App.Common.Models;
using App.Common.Utility;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using static App.Common.Constant.Constants;

namespace App.Application.Service.Organization
{
    public class OrganizationService(IOrganizationRepository organizationRepository,
                   IImageService imageService, IAuditLogRepository auditLogRepository, IHttpContextAccessor httpContextAccessor) : IOrganizationService //, IAwsServices awsService, IAzureBlobStorageService azureBlobService
    {
        private readonly IOrganizationRepository _organizationRepository = organizationRepository;
        private readonly IImageService _imageService = imageService;
        private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        //private readonly IAwsServices _awsService = awsService;
        //private readonly IAzureBlobStorageService _azureBlobService = azureBlobService;


        public async Task<JsonModel> CreateOrganizationAsync(OrganizationDto orgDto, int userId)
        {
            //logo file name creation
            string logoFileName = ShouldGenerateLogoFileName(orgDto)
            ? _imageService.CreateFileNameWrtTime(
            GetLogoPath(orgDto),
            Common.Enums.CommonEnums.ImagesFolder.Logo.ToString())
            : string.Empty;

            string favIconFileName = ShouldGenerateFavIconFileName(orgDto)
                ? _imageService.CreateFileNameWrtTime(
                    GetFaviconPath(orgDto),
                    Common.Enums.CommonEnums.ImagesFolder.Favicon.ToString())
                : string.Empty;

            // Get Base64 values
            string logoBase64 = GetStorageValue(orgDto, Common.Enums.CommonEnums.ImagesFolder.Logo.ToString());
            string faviconBase64 = GetStorageValue(orgDto, Common.Enums.CommonEnums.ImagesFolder.Favicon.ToString());

            // Set DTO paths
            SetStoragePaths(orgDto, logoFileName, Common.Enums.CommonEnums.ImagesFolder.Logo.ToString());
            SetStoragePaths(orgDto, favIconFileName, Common.Enums.CommonEnums.ImagesFolder.Favicon.ToString());

            // Call repository
            var response = await _organizationRepository.CreateOrganizationAsync(orgDto, userId);
            if (response?.Data == null || response.Data.ToString() == "0")
            {
                return new JsonModel
                {
                    Message = response?.Message ?? StatusMessage.InternalServerError,
                    StatusCode = (int)(response?.Message != null ? HttpStatusCode.BadRequest : HttpStatusCode.InternalServerError)
                };
            }
            int organizationId = Convert.ToInt32(response.Data);

            // Save images
            await SaveImagesAsync(orgDto, organizationId, logoFileName, logoBase64, favIconFileName, faviconBase64);

            return new JsonModel()
            {
                Message = StatusMessage.RecordSavedSuccessfully,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        public async Task<JsonModel> UpdateOrganizationAsync(OrganizationDto orgDto, int userId)
        {
            /// Generate logo file name
            string logoFileName = ShouldGenerateLogoFileName(orgDto)
                ? _imageService.CreateFileNameWrtTime(GetLogoPath(orgDto), Common.Enums.CommonEnums.ImagesFolder.Logo.ToString()) //"Logo"
                : string.Empty;

            // Generate favicon file name
            string favIconFileName = ShouldGenerateFavIconFileName(orgDto)
                ? _imageService.CreateFileNameWrtTime(GetFaviconPath(orgDto), Common.Enums.CommonEnums.ImagesFolder.Favicon.ToString()) //"Favicon"
                : string.Empty;

            // Get Base64 values
            string logoBase64 = GetStorageValue(orgDto, Common.Enums.CommonEnums.ImagesFolder.Logo.ToString());
            string faviconBase64 = GetStorageValue(orgDto, Common.Enums.CommonEnums.ImagesFolder.Favicon.ToString());

            // Set DTO paths
            SetStoragePaths(orgDto, logoFileName, Common.Enums.CommonEnums.ImagesFolder.Logo.ToString());
            SetStoragePaths(orgDto, favIconFileName, Common.Enums.CommonEnums.ImagesFolder.Favicon.ToString());


            // Call repository
            var response = await _organizationRepository.UpdateOrganizationAsync(orgDto, userId);
            if (response?.Data == null || response.Data.ToString() == "0")
            {
                return new JsonModel
                {
                    Message = response?.Message ?? StatusMessage.InternalServerError,
                    StatusCode = (int)(response?.Message != null ? HttpStatusCode.BadRequest : HttpStatusCode.InternalServerError)
                };
            }
            int organizationId = Convert.ToInt32(response.Data);


            // Save images
            await SaveImagesAsync(orgDto, organizationId, logoFileName, logoBase64, favIconFileName, faviconBase64);

            return new JsonModel()
            {
                Message = StatusMessage.RecordSavedSuccessfully,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        public async Task<JsonModel> OrganizationStatusUpdateAsync(OrganizationStatusUpdateDto orgDto, int userId)
        {
            var organization = await _organizationRepository.GetByIdAsync(orgDto.OrganizationID);
            if (organization == null)
                return new JsonModel()
                {
                    Message = StatusMessage.NoDataFound,
                    StatusCode = (int)HttpStatusCode.OK
                };

            organization.IsActive = orgDto.IsActive;
            organization.UpdatedBy = userId;
            organization.UpdatedAt = DateTime.UtcNow;

            _organizationRepository.Update(organization);
            //await _unitOfWork.CommitAsync();
            string ipAddress = CommonMethods.GetClientIp(_httpContextAccessor.HttpContext);
            _auditLogRepository.SaveChangesWithAuditLogs(AuditLogsScreen.ManageOrganization, (int)MasterActions.Update, userId, null, ipAddress, (int)MasterPortal.SuperAdminPortal, null, null, null);
            return new JsonModel()
            {
                Message = StatusMessage.RecordSavedSuccessfully,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        public async Task<JsonModel> GetAllOrganizationsAsync(FilterDto filter)
        {
            List<OrganizationReponseDto> data = await _organizationRepository.GetAllOrganizationsAsync(filter);

            return data.Any()
            ? new JsonModel { Data = data, StatusCode = (int)HttpStatusCode.OK }
            : new JsonModel { Message = StatusMessage.InternalServerError, StatusCode = (int)HttpStatusCode.InternalServerError };
        }

        public async Task<JsonModel> GetOrganizationByIdAsync(int organizationId, IHttpContextAccessor contextAccessor)
        {
            OrganizationReponseDto data = await _organizationRepository.GetOrganizationByIdAsync(organizationId);

            if (data == null)
            {
                return new JsonModel()
                {
                    Message = StatusMessage.InternalServerError,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            var scheme = contextAccessor.HttpContext?.Request?.Scheme;
            var host = contextAccessor.HttpContext?.Request?.Host.Value;

            string logoS3Path = organizationId + ImagesPath.OrganizationImagesLogoS3;
            string faviconS3Path = organizationId + ImagesPath.OrganizationImagesFaviconS3;

            if (!string.IsNullOrEmpty(data.LogoLocalPath) || !string.IsNullOrEmpty(data.FavIconLocalPath))
            {
                ProcessLocalPaths(data, contextAccessor);
            }
            else if (!string.IsNullOrEmpty(data.LogoBlobPath) || !string.IsNullOrEmpty(data.FavIconBlobPath))
            {
                // TODO: Handle Azure Blob Storage logic
            }
            else if (!string.IsNullOrEmpty(data.LogoAWSPath) || !string.IsNullOrEmpty(data.FavIconAWSPath))
            {
                //(uncomment this for aws s3 bucket access)
                //await ProcessAwsS3PathsAsync(data, scheme, host, logoS3Path, faviconS3Path);
            }
            return new JsonModel()
            {
                Data = data,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        public async Task<JsonModel> GetCardStatisticsAsync()
        {
            OrganizationCardStatisticsDto data = await _organizationRepository.GetCardStatisticsAsync();

            if (data == null)
            {
                return new JsonModel()
                {
                    Message = StatusMessage.InternalServerError,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            return new JsonModel()
            {
                Data = data,
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        #region Storage Configuration Methods

        public async Task<JsonModel> GetStorageConfigurationAsync(int organizationId)
        {
            try
            {
                var result = await _organizationRepository.GetStorageConfigurationAsync(organizationId);
                
                if (result == null)
                {
                    return new JsonModel
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "Storage configuration not found for organization"
                    };
                }

                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Data = result,
                    Message = "Storage configuration retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error retrieving storage configuration: {ex.Message}"
                };
            }
        }

        public async Task<JsonModel> SaveStorageConfigurationAsync(StorageConfigurationDto storageConfigurationDto, int userId)
        {
            try
            {
                // Validate storage type
                if (!IsValidStorageType(storageConfigurationDto.StorageType))
                {
                    return new JsonModel
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "Invalid storage type. Must be 'Local', 'AzureBlobStorage', or 'AWS'"
                    };
                }

                // Validate required fields based on storage type
                var validationResult = ValidateStorageConfiguration(storageConfigurationDto);
                if (!validationResult.IsValid)
                {
                    return new JsonModel
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = validationResult.ErrorMessage
                    };
                }

                var result = await _organizationRepository.SaveStorageConfigurationAsync(
                    storageConfigurationDto.OrganizationId,
                    storageConfigurationDto.StorageType,
                    storageConfigurationDto.AwsConfig,
                    storageConfigurationDto.AzureConfig,
                    userId
                );

                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Data = result,
                    Message = result.Id > 0 ? "Storage configuration updated successfully" : "Storage configuration created successfully"
                };
            }
            catch (Exception ex)
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error saving storage configuration: {ex.Message}"
                };
            }
        }

        public async Task<JsonModel> DeleteStorageConfigurationAsync(int organizationId, int userId)
        {
            try
            {
                var result = await _organizationRepository.DeleteStorageConfigurationAsync(organizationId, userId);
                
                if (!result)
                {
                    return new JsonModel
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "Storage configuration not found for organization"
                    };
                }

                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Storage configuration deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error deleting storage configuration: {ex.Message}"
                };
            }
        }

        public async Task<JsonModel> GetAllStorageConfigurationsAsync()
        {
            try
            {
                var result = await _organizationRepository.GetAllStorageConfigurationsAsync();
                
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Data = result,
                    Message = "Storage configurations retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new JsonModel
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = $"Error retrieving storage configurations: {ex.Message}"
                };
            }
        }





        #endregion

        #region Helper methods

        // Helper: Process local storage image paths
        private void ProcessLocalPaths(OrganizationReponseDto data, IHttpContextAccessor contextAccessor)
        {
            data.LogoLocalPath = ProcessImageUrl(data.LogoLocalPath, ImagesPath.OrganizationImages + "Logo//", contextAccessor);
            data.FavIconLocalPath = ProcessImageUrl(data.FavIconLocalPath, ImagesPath.OrganizationImages + "Favicon//", contextAccessor);
        }

        // Helper: Process AWS S3 image paths
        private async Task ProcessAwsS3PathsAsync(OrganizationReponseDto data, string? scheme, string? host, string logoS3Path, string faviconS3Path)
        {
            data.LogoAWSPath = await CreateS3ImageUrlAsync(data.LogoAWSPath, scheme, host, logoS3Path);
            data.FavIconAWSPath = await CreateS3ImageUrlAsync(data.FavIconAWSPath, scheme, host, faviconS3Path);
        }

        // Helper: Create S3 image URL if path exists
        private async Task<string> CreateS3ImageUrlAsync(string filePath, string? scheme, string? host, string s3FolderPath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            string fileName = Path.GetFileName(filePath);
            //(uncomment this for aws s3 bucket access)
            //return await _awsService.CreateImageUrlFromS3Async(scheme, host, s3FolderPath, fileName);
            return null;
        }

        // Helper: Save images based on storage type
        private async Task SaveImagesAsync(OrganizationDto dto, int orgId, string logoFileName, string logoBase64, string favIconFileName, string favIconBase64)
        {
            if (dto.IsLocalStorage)
            {
                if (!string.IsNullOrEmpty(logoFileName) && !string.IsNullOrEmpty(favIconFileName))
                {
                    var filesToSave = new List<(string, string, string, string)>
                    {
                        (logoBase64, ImagesPath.OrganizationImages, Common.Enums.CommonEnums.ImagesFolder.Logo.ToString(), logoFileName),
                        (favIconBase64, ImagesPath.OrganizationImages, Common.Enums.CommonEnums.ImagesFolder.Favicon.ToString(), favIconFileName)
                    };
                    _imageService.SaveMultipleImages(filesToSave);
                }
                else if (!string.IsNullOrEmpty(logoFileName))
                    _imageService.SaveImages(logoBase64, ImagesPath.OrganizationImages, Common.Enums.CommonEnums.ImagesFolder.Logo.ToString(), logoFileName);
                else if (!string.IsNullOrEmpty(favIconFileName))
                    _imageService.SaveImages(favIconBase64, ImagesPath.OrganizationImages, Common.Enums.CommonEnums.ImagesFolder.Favicon.ToString(), favIconFileName);
            }
            else if (dto.IsAzureBlobStorage)
            {
                //(uncomment this for azure blob access)
                // TODO: Implement Azure Blob logic
                //string fullPathLogo = orgId + ImagesPath.OrganizationImagesLogoBlob;
                //await _imageService.SaveImagesToBlobAsync(logoBase64, fullPathLogo, logoFileName, Common.Enums.CommonEnums.ImagesFolder.Logo.ToString());
            }
            else if (dto.IsAWSS3Storage)
            {
                //(uncomment this for aws s3 bucket access)
                //if (!string.IsNullOrEmpty(logoFileName) && !string.IsNullOrEmpty(favIconFileName))
                //{
                //    string fullPathLogo = orgId + ImagesPath.OrganizationImagesLogoS3;
                //    string fullPathFavicon = orgId + ImagesPath.OrganizationImagesFaviconS3;

                //    var imagesToUpload = new List<(string Base64String, string FolderPath, string FileName, string ImageType)>
                //    {
                //        (logoBase64, fullPathLogo, logoFileName, Common.Enums.CommonEnums.ImagesFolder.Logo.ToString()),
                //        (favIconBase64, fullPathFavicon, favIconFileName, Common.Enums.CommonEnums.ImagesFolder.Favicon.ToString())
                //    };
                //    await _imageService.SaveMultipleImagesToS3Async(imagesToUpload);
                //}
                //else if (!string.IsNullOrEmpty(logoFileName))
                //{
                //    string fullPathLogo = orgId + ImagesPath.OrganizationImagesLogoS3;
                //    await _imageService.SaveImagesToS3Async(logoBase64, fullPathLogo, logoFileName, Common.Enums.CommonEnums.ImagesFolder.Logo.ToString());
                //}
                //else if (!string.IsNullOrEmpty(favIconFileName))
                //{
                //    string fullPathFavicon = orgId + ImagesPath.OrganizationImagesFaviconS3;
                //    await _imageService.SaveImagesToS3Async(favIconBase64, fullPathFavicon, favIconFileName, Common.Enums.CommonEnums.ImagesFolder.Favicon.ToString());
                //}
            }
        }

        // Helper: Get base64 path
        private string GetStorageValue(OrganizationDto dto, string type) =>
            dto.IsAWSS3Storage ? (type == Common.Enums.CommonEnums.ImagesFolder.Logo.ToString() ? dto.LogoAWSPath : dto.FavIconAWSPath) :
            dto.IsLocalStorage ? (type == Common.Enums.CommonEnums.ImagesFolder.Logo.ToString() ? dto.LogoLocalPath : dto.FavIconLocalPath) :
            dto.IsAzureBlobStorage ? (type == Common.Enums.CommonEnums.ImagesFolder.Logo.ToString() ? dto.LogoBlobPath : dto.FavIconBlobPath) :
            string.Empty;

        // Helper: Set storage file paths in DTO
        private void SetStoragePaths(OrganizationDto dto, string fileName, string type)
        {
            if (type == Common.Enums.CommonEnums.ImagesFolder.Logo.ToString())
            {
                dto.LogoAWSPath = dto.IsAWSS3Storage ? fileName : string.Empty;
                dto.LogoLocalPath = dto.IsLocalStorage ? fileName : string.Empty;
                dto.LogoBlobPath = dto.IsAzureBlobStorage ? fileName : string.Empty;
            }
            else
            {
                dto.FavIconAWSPath = dto.IsAWSS3Storage ? fileName : string.Empty;
                dto.FavIconLocalPath = dto.IsLocalStorage ? fileName : string.Empty;
                dto.FavIconBlobPath = dto.IsAzureBlobStorage ? fileName : string.Empty;
            }
        }

        private string ProcessImageUrl(string imageName, string path, IHttpContextAccessor contextAccessor)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), path.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), imageName);
                if (File.Exists(fullPath))
                {
                    return CommonMethods.CreateImageUrl(contextAccessor.HttpContext, path, imageName);
                }
            }
            return string.Empty;
        }

        private static bool ShouldGenerateLogoFileName(OrganizationDto orgDto)
        {
            return orgDto.IsLocalStorage || !string.IsNullOrEmpty(orgDto.LogoLocalPath) ||
                   orgDto.IsAWSS3Storage || !string.IsNullOrEmpty(orgDto.LogoAWSPath) ||
                   orgDto.IsAzureBlobStorage || !string.IsNullOrEmpty(orgDto.LogoBlobPath);
        }

        private static bool ShouldGenerateFavIconFileName(OrganizationDto orgDto)
        {
            return orgDto.IsLocalStorage || !string.IsNullOrEmpty(orgDto.FavIconLocalPath) ||
                   orgDto.IsAWSS3Storage || !string.IsNullOrEmpty(orgDto.FavIconAWSPath) ||
                   orgDto.IsAzureBlobStorage || !string.IsNullOrEmpty(orgDto.FavIconBlobPath);
        }

        private string GetLogoPath(OrganizationDto dto)
        {
            return dto.IsLocalStorage ? dto.LogoLocalPath
                 : dto.IsAWSS3Storage ? dto.LogoAWSPath
                 : dto.LogoBlobPath;
        }

        private string GetFaviconPath(OrganizationDto dto)
        {
            return dto.IsLocalStorage ? dto.FavIconLocalPath
                 : dto.IsAWSS3Storage ? dto.FavIconAWSPath
                 : dto.FavIconBlobPath;
        }

        #endregion

        #region Storage Configuration Helper Methods

        private bool IsValidStorageType(string storageType)
        {
            return storageType switch
            {
                "Local" or "AzureBlobStorage" or "AWS" => true,
                _ => false
            };
        }

        private (bool IsValid, string ErrorMessage) ValidateStorageConfiguration(StorageConfigurationDto dto)
        {
            return dto.StorageType switch
            {
                "AWS" => ValidateAwsConfiguration(dto.AwsConfig),
                "AzureBlobStorage" => ValidateAzureConfiguration(dto.AzureConfig),
                "Local" => (true, string.Empty),
                _ => (false, "Invalid storage type")
            };
        }

        private (bool IsValid, string ErrorMessage) ValidateAwsConfiguration(AwsStorageConfigDto? awsConfig)
        {
            if (awsConfig == null)
                return (false, "AWS configuration is required for AWS storage type");

            if (string.IsNullOrWhiteSpace(awsConfig.Profile))
                return (false, "AWS Profile is required");

            if (string.IsNullOrWhiteSpace(awsConfig.Region))
                return (false, "AWS Region is required");

            if (string.IsNullOrWhiteSpace(awsConfig.AccessKey))
                return (false, "AWS Access Key is required");

            if (string.IsNullOrWhiteSpace(awsConfig.SecretKey))
                return (false, "AWS Secret Key is required");

            if (string.IsNullOrWhiteSpace(awsConfig.BucketName))
                return (false, "AWS Bucket Name is required");

            return (true, string.Empty);
        }

        private (bool IsValid, string ErrorMessage) ValidateAzureConfiguration(AzureBlobStorageConfigDto? azureConfig)
        {
            if (azureConfig == null)
                return (false, "Azure configuration is required for Azure Blob Storage type");

            if (string.IsNullOrWhiteSpace(azureConfig.ConnectionString))
                return (false, "Azure Connection String is required");

            if (string.IsNullOrWhiteSpace(azureConfig.ContainerName))
                return (false, "Azure Container Name is required");

            if (string.IsNullOrWhiteSpace(azureConfig.BlobStorageUrl))
                return (false, "Azure Blob Storage URL is required");

            return (true, string.Empty);
        }



        #endregion
    }
}
