namespace App.Application.Interfaces.Services.AzureBlobStorageService
{
    public interface IAzureBlobStorageService
    {
        Task<string> UploadFromStreamAsync(Stream fileStream, string blobName);
        public string GetBlobContainerName();
    }
}
