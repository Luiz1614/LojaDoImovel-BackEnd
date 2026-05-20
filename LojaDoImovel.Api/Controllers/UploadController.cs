using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaDoImovel.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "userapproved")]
public class UploadController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> UploadImages(List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest("Nenhum arquivo encontrado.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadFolder);

        var urls = new List<string>();

        foreach(var file in files)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();

            if(!allowedExtensions.Contains(extension))
                continue;

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadFolder, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            urls.Add($"/uploads/{fileName}");
        }

        return Ok(new { urls });
    }
}
