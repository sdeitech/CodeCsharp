using System.ComponentModel.DataAnnotations;

namespace App.Application.Dto.StorageConfiguration
{
    public class StorageConfigurationDto
    {
        public int? Id { get; set; }
        
        [Required]
        public int OrganizationId { get; set; }
        
        [Required]
        public string StorageType { get; set; } = "Local"; // Default to Local
        
        public AwsStorageConfigDto? AwsConfig { get; set; }
        public AzureBlobStorageConfigDto? AzureConfig { get; set; }
    }

    public class AwsStorageConfigDto
    {
        [Required]
        public string Profile { get; set; } = string.Empty;
        
        [Required]
        public string Region { get; set; } = string.Empty;
        
        [Required]
        public string AccessKey { get; set; } = string.Empty;
        
        [Required]
        public string SecretKey { get; set; } = string.Empty;
        
        [Required]
        public string BucketName { get; set; } = string.Empty;
    }

    public class AzureBlobStorageConfigDto
    {
        [Required]
        public string ConnectionString { get; set; } = string.Empty;
        
        [Required]
        public string ContainerName { get; set; } = string.Empty;
        
        [Required]
        public string BlobStorageUrl { get; set; } = string.Empty;
    }
}
