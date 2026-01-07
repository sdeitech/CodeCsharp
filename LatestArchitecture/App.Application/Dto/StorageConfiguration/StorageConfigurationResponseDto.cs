namespace App.Application.Dto.StorageConfiguration
{
    public class StorageConfigurationResponseDto
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string StorageType { get; set; } = string.Empty;
        
        // AWS S3 Storage Properties
        public string? AwsProfile { get; set; }
        public string? AwsRegion { get; set; }
        public string? AwsAccessKey { get; set; }
        public string? AwsSecretKey { get; set; }
        public string? AwsBucketName { get; set; }
        
        // Azure Blob Storage Properties
        public string? AzureConnectionString { get; set; }
        public string? AzureContainerName { get; set; }
        public string? AzureBlobStorageUrl { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
