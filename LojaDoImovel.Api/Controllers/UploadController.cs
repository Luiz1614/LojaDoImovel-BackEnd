using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaDoImovel.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "userapproved")]
public class UploadController : ControllerBase
{
    private readonly ILogger<UploadController> _logger;

    public UploadController(ILogger<UploadController> logger)
    {
        _logger = logger;
    }


    /// <summary>
    /// Uploads one or more image files to the server.
    /// </summary>
    /// <remarks>This action requires the caller to be authorized with the 'userapproved' role. Only files
    /// with the extensions .jpg, .jpeg, .png, and .webp are accepted. Files with unsupported extensions are
    /// silently ignored.</remarks>
    /// <param name="files">The list of image files to upload. Cannot be null or empty.</param>
    /// <returns>An HTTP 200 response containing the list of URLs for the successfully uploaded files; otherwise,
    /// a 400 Bad Request response if no files are provided.</returns>
    [HttpPost]
    public async Task<IActionResult> UploadImages(List<IFormFile> files)
    {
        _logger.LogInformation("Requisição de upload recebida com {Count} arquivo(s).", files?.Count ?? 0);

        if (files == null || files.Count == 0)
        {
            _logger.LogWarning("Upload rejeitado: nenhum arquivo enviado.");
            return BadRequest("Nenhum arquivo encontrado.");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadFolder);

        var urls = new List<string>();

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Arquivo '{FileName}' ignorado: extensão '{Extension}' não permitida.", file.FileName, extension);
                continue;
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadFolder, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            urls.Add($"/uploads/{fileName}");
            _logger.LogInformation("Arquivo '{FileName}' salvo como '{SavedName}'.", file.FileName, fileName);
        }

        _logger.LogInformation("Upload concluído. {Count} arquivo(s) processado(s).", urls.Count);
        return Ok(new { urls });
    }
}
