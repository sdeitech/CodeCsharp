namespace App.Application.Interfaces.Services.AwsServices
{
    public interface IAwsServices
    {
        public string GetAwsBucketName();
        Task<string> CreateImageUrlFromS3Async(string scheme, string host, string folderPath, string fileName);
        Task UploadStreamToS3Async(Stream stream, string bucket, string key, string contentType);
    }
}
