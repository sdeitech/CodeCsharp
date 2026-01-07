namespace App.Application.Interfaces.Services.Images
{
    public interface IImageService
    {
        string SaveImages(string base64String, string directory, string folderName, string fileName);
        List<string> SaveMultipleImages(List<(string Base64String, string Directory, string FolderName, string FileName)> files);
        Task<string> SaveImagesToS3Async(string base64String, string folderPath, string fileName, string imageType);
        Task<string> SaveImagesToBlobAsync(string base64String, string folderPath, string fileName, string imageType);
        Task<List<string>> SaveMultipleImagesToS3Async(List<(string Base64String, string FolderPath, string FileName, string ImageType)> images);
        string CreateFileNameWrtTime(string base64String, string folderName);
    }
}
