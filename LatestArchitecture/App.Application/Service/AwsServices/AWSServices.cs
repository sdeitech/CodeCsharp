using Amazon.S3;
using Amazon.S3.Model;
using App.Application.Interfaces.Services.AwsServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace App.Application.Service.AwsServices
{
    public class AwsServices : IAwsServices
    {
        private readonly IAmazonS3 awsS3Client;
        private readonly ILogger<AwsServices> _logger;
        private readonly IConfiguration _configuration;

        public AwsServices(IAmazonS3 s3Client, IHttpContextAccessor httpContextAccessor, ILogger<AwsServices> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            awsS3Client = s3Client;
            _logger = logger;
        }

        public async Task<string> CreateImageUrlFromS3Async(string scheme, string host, string folderPath, string fileName)
        {
            string bucketName = GetAwsBucketName();
            string s3Key = $"{folderPath.Trim('/').Replace("\\", "/")}/{fileName}";

            try
            {
                var metadataRequest = new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = s3Key
                };

                // Check if the file exists in S3
                await awsS3Client.GetObjectMetadataAsync(metadataRequest);

                // If it exists, generate the accessible URL
                var encodedFolderPath = string.Join("/", folderPath.Trim('/').Split('/').Select(Uri.EscapeDataString));
                var encodedFile = Uri.EscapeDataString(fileName);

                var imageUrl = $"{scheme}://{host}/api/awsservice/get-file/{encodedFolderPath}/{encodedFile}";

                _logger.LogInformation("Image exists in S3. Returning image URL: {ImageUrl}", imageUrl);

                return imageUrl;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Image file not found in S3 (but exists in DB). Bucket: {Bucket}, Key: {Key}", bucketName, s3Key);
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, "Amazon S3 error while checking metadata for image file. Bucket: {Bucket}, Key: {Key}", bucketName, s3Key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while generating image URL from S3.");
            }

            var defaultUrl = String.Empty;
            _logger.LogInformation("Returning default image URL: {DefaultUrl}", defaultUrl);
            return defaultUrl;
        }

        public string GetAwsBucketName()
        {
            return _configuration["AWS:BucketName"];
        }

        public async Task UploadStreamToS3Async(Stream stream, string bucket, string key, string contentType)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = key,
                InputStream = stream,
                ContentType = contentType
            };

            await awsS3Client.PutObjectAsync(request);
        }

    }
}
