using App.Application.Interfaces.Services.AzureBlobStorageService;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;

namespace App.Application.Service.AzureBlobStorageService
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly IConfiguration _configuration;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFromStreamAsync(Stream fileStream, string blobName)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(_configuration["AzureBlobStorage:ConnectionString"]);
                var containerClient = blobServiceClient.GetBlobContainerClient(GetBlobContainerName());

                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                var blobClient = containerClient.GetBlobClient(blobName);

                await blobClient.UploadAsync(fileStream, overwrite: true);

                //Console.WriteLine($"File uploaded to: {blobClient.Uri}");
                return blobClient.Uri.ToString();
            }
            catch (Exception)
            {
                //Console.WriteLine($"Error uploading stream: {ex.Message}");
                throw;
            }
        }

        public string GenerateBlobSasUri(BlobClient blobClient, TimeSpan expiryTime, string connectionString)
        {
            // Parse connection string to get credentials
            var blobUriBuilder = new BlobUriBuilder(blobClient.Uri);
            var blobServiceClient = new BlobServiceClient(connectionString);
            var accountName = blobServiceClient.AccountName;

            // Use StorageSharedKeyCredential to sign SAS
            var connectionStringParts = connectionString.Split(';');
            string accountKeyPrefix = "AccountKey=";
            string accountKey = connectionString
                .Split(';')
                .FirstOrDefault(p => p.StartsWith(accountKeyPrefix))?
                .Substring(accountKeyPrefix.Length);

            var credential = new StorageSharedKeyCredential(accountName, accountKey);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobUriBuilder.BlobContainerName,
                BlobName = blobUriBuilder.BlobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiryTime)
            };

            // Permissions: read access
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = sasBuilder.ToSasQueryParameters(credential).ToString();
            var sasUrl = $"{blobClient.Uri}?{sasToken}";

            return sasUrl;
        }

        public string GetBlobContainerName()
        {
            return _configuration["AzureBlobStorage:ContainerName"];
        }

    }
}
