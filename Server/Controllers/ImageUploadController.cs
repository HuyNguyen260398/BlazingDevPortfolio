using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ImageUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ImageUploadController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UploadedImage uploadedImage)
    {
        try
        {
            if (ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (uploadedImage.OldImagePath != string.Empty)
            {
                if (uploadedImage.OldImagePath != "uploads/placeholder/jpg")
                {
                    string oldImageName = uploadedImage.OldImagePath.Split('/').Last();
                    System.IO.File.Delete($"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{oldImageName}");
                }
            }

            string guid = Guid.NewGuid().ToString();
            string imageName = guid + uploadedImage.NewImageExtension;
            string fullImageFileSystemPath = $"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{imageName}";

            FileStream fileStream = System.IO.File.Create(fullImageFileSystemPath);
            byte[] imageAsByte = Convert.FromBase64String(uploadedImage.NewImageBase64Content);
            await fileStream.WriteAsync(imageAsByte, 0, imageAsByte.Length);
            fileStream.Close();

            string relativePathWithoutTrailingSlashes = $"uploads/{imageName}";
            return Created("Create", relativePathWithoutTrailingSlashes);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}
