
using App.Application.Dto.Organization;
using App.Application.Interfaces.Services.Images;
using App.Common.Utility;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace App.Application.Service.Images
{
    public class ImageService : IImageService
    {
        //(uncomment this for aws s3 bucket and azure blob access)
        //private readonly IAwsServices awsService;
        private readonly string bucketName = string.Empty;
        private readonly string blobName = string.Empty;
        //private readonly IAzureBlobStorageService _azureBlobService;

        //public ImageService(IAwsServices awsServices, IAzureBlobStorageService azureBlobService)
        public ImageService()
        {
            //(uncomment this for aws s3 bucket and azure blob access)
            //awsService = awsServices;
            //bucketName = awsService.GetAwsBucketName();
            //_azureBlobService = azureBlobService;
            //blobName = _azureBlobService.GetBlobContainerName();
        }

        public string SaveImages(string base64String, string directory, string folderName, string fileName)
        {
            //get current directory root
            string webRootPath = Directory.GetCurrentDirectory();

            //add your custom path
            webRootPath = webRootPath + directory + folderName;

            //check 
            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
            }

            if (!string.IsNullOrEmpty(base64String))
            {
                //getting data from base64 url
                var base64Data = Regex.Match(base64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                //getting extension of the image
                string extension = CommonMethods.GetExtenstion(Regex.Match(base64String, @"data:(?<type>.+?),(?<data>.+)").Groups["type"].Value.Split(';')[0]);

                string path = webRootPath + "/" + fileName;

                //convert files into base
                byte[] bytes = Convert.FromBase64String(base64Data);

                TypeImageModel typeImageModel = new TypeImageModel();
                typeImageModel.Type = folderName;
                typeImageModel.Url = path;
                typeImageModel.Bytes = bytes;

                ////save into the directory
                CommonMethods.SaveImages(typeImageModel);
                //File.WriteAllBytes(path, bytes);

                return fileName;
            }
            else
            {
                return "";
            }
        }

        public List<string> SaveMultipleImages(List<(string Base64String, string Directory, string FolderName, string FileName)> files)
        {
            var savedFileNames = new List<string>();

            foreach (var file in files)
            {
                var fileName = SaveImages(file.Base64String, file.Directory, file.FolderName, file.FileName);
                if (!string.IsNullOrEmpty(fileName))
                {
                    savedFileNames.Add(fileName);
                }
            }

            return savedFileNames;
        }

        public string CreateFileNameWrtTime(string base64String, string folderName)
        {
            if (string.IsNullOrEmpty(base64String))
                return string.Empty;

            // Extract base64 data and file extension
            var base64Data = Regex.Match(base64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var mimeType = Regex.Match(base64String, @"data:(?<type>.+?);").Groups["type"].Value;
            string extension = CommonMethods.GetExtenstion(mimeType);

            string time = DateTime.UtcNow.TimeOfDay.ToString();
            time = time.Replace(":", "_").Replace(".", "_");
            string fileName = folderName + "_" + time;
            return $"{fileName}{extension}";
        }

        public async Task<string> SaveImagesToS3Async(string base64String, string folderPath, string fileName, string imageType)
        {
            // Extract base64 data and file extension
            var base64Data = Regex.Match(base64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var mimeType = Regex.Match(base64String, @"data:(?<type>.+?);").Groups["type"].Value;
            string extension = CommonMethods.GetExtenstion(mimeType);


            // Convert base64 to byte array
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            using var originalStream = new MemoryStream(imageBytes);
            using var image = Image.FromStream(originalStream, true, true);

            // Set default dimensions
            int width = 180, height = 32;

            if (imageType == Common.Enums.CommonEnums.ImagesFolder.Favicon.ToString())
            {
                width = 16;
                height = 16;
            }

            // Create thumbnail/resize
            using var thumbnailImg = new Bitmap(width, height);
            using (var thumbGraph = Graphics.FromImage(thumbnailImg))
            {
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var imageRectangle = new Rectangle(0, 0, width, height);
                thumbGraph.DrawImage(image, imageRectangle);

                using var thumbnailStream = new MemoryStream();
                thumbnailImg.Save(thumbnailStream, image.RawFormat);
                thumbnailStream.Position = 0;

                string fullImageKey = $"{folderPath}/{fileName}";
                //await awsService.UploadStreamToS3Async(thumbnailStream, bucketName, fullImageKey, $"image/{extension}");
            }

            return fileName;
        }
        
        public async Task<string> SaveImagesToBlobAsync(string base64String, string folderPath, string fileName, string imageType)
        {
            // Extract base64 data and file extension
            var base64Data = Regex.Match(base64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var mimeType = Regex.Match(base64String, @"data:(?<type>.+?);").Groups["type"].Value;
            string extension = CommonMethods.GetExtenstion(mimeType);


            // Convert base64 to byte array
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            using var originalStream = new MemoryStream(imageBytes);
            using var image = Image.FromStream(originalStream, true, true);

            // Set default dimensions
            int width = 180, height = 32;

            if (imageType == Common.Enums.CommonEnums.ImagesFolder.Favicon.ToString())
            {
                width = 16;
                height = 16;
            }

            // Create thumbnail/resize
            using var thumbnailImg = new Bitmap(width, height);
            using (var thumbGraph = Graphics.FromImage(thumbnailImg))
            {
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var imageRectangle = new Rectangle(0, 0, width, height);
                thumbGraph.DrawImage(image, imageRectangle);

                using var thumbnailStream = new MemoryStream();
                thumbnailImg.Save(thumbnailStream, image.RawFormat);
                thumbnailStream.Position = 0;

                string fullImageKey = $"{folderPath}/{fileName}";
                //(uncomment this for azure blob access)
                //await _azureBlobService.UploadFromStreamAsync(thumbnailStream, blobName);
            }

            return fileName;
        }

        //multiple files
        public async Task<List<string>> SaveMultipleImagesToS3Async(List<(string Base64String, string FolderPath, string FileName, string ImageType)> images)
        {
            var uploadTasks = images.Select(async img =>
            {
                // Extract base64 data and file extension
                var base64Data = Regex.Match(img.Base64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var mimeType = Regex.Match(img.Base64String, @"data:(?<type>.+?);").Groups["type"].Value;
                string extension = CommonMethods.GetExtenstion(mimeType);

                // Convert base64 to byte array
                byte[] imageBytes = Convert.FromBase64String(base64Data);

                using var originalStream = new MemoryStream(imageBytes);
                using var image = Image.FromStream(originalStream, true, true);

                // Default dimensions
                int width = 180, height = 32;
                if (img.ImageType == Common.Enums.CommonEnums.ImagesFolder.Favicon.ToString())
                {
                    width = 16;
                    height = 16;
                }

                // Create thumbnail/resize
                using var thumbnailImg = new Bitmap(width, height);
                using (var thumbGraph = Graphics.FromImage(thumbnailImg))
                {
                    thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                    thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                    thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    var imageRectangle = new Rectangle(0, 0, width, height);
                    thumbGraph.DrawImage(image, imageRectangle);

                    using var thumbnailStream = new MemoryStream();
                    thumbnailImg.Save(thumbnailStream, image.RawFormat);
                    thumbnailStream.Position = 0;

                    string fullImageKey = $"{img.FolderPath}/{img.FileName}";
                    //(uncomment this for aws s3 bucket access)
                    //await awsService.UploadStreamToS3Async(thumbnailStream, bucketName, fullImageKey, $"image/{extension}");
                }

                return img.FileName;
            });

            // Run all uploads in parallel
            var uploadedFiles = await Task.WhenAll(uploadTasks);
            return uploadedFiles.ToList();
        }


    }
}
