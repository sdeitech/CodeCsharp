using App.Api.Controllers;
using App.Application.Interfaces.Services.Images;

using App.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Images;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ImagesController : BaseController
{
    private readonly IImageService _imageService;

    public ImagesController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(JsonModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadImage([FromBody] UploadImageRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Base64String))
            {
                return BadRequest(new JsonModel(null!, "Base64 image data is required", StatusCodes.Status400BadRequest));
            }

            if (string.IsNullOrEmpty(request.FileName))
            {
                return BadRequest(new JsonModel(null!, "File name is required", StatusCodes.Status400BadRequest));
            }

            if (string.IsNullOrEmpty(request.FolderPath))
            {
                return BadRequest(new JsonModel(null!, "Folder path is required", StatusCodes.Status400BadRequest));
            }

            // Generate unique filename with timestamp
            var uniqueFileName = _imageService.CreateFileNameWrtTime(request.Base64String, Common.Enums.CommonEnums.ImagesFolder.DynamicQuestionnaire.ToString());

            // Save image (this will use the configured storage - local, S3, or Azure Blob)
            string imageUrl;
            
            // For now, we'll use local storage. In production, you might want to use S3 or Azure Blob
            imageUrl = _imageService.SaveImages(request.Base64String, "/Images/", request.FolderPath, uniqueFileName);

            var response = new { imageUrl = imageUrl };
            return Ok(new JsonModel(response, "Image uploaded successfully", StatusCodes.Status200OK));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new JsonModel(null!, $"Error uploading image: {ex.Message}", StatusCodes.Status500InternalServerError));
        }
    }
}

public class UploadImageRequest
{
    public string Base64String { get; set; } = string.Empty;
    public string FolderPath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ImageType { get; set; } = string.Empty;
}
