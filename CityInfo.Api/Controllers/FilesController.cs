using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _contentTypeProvider;

    public FilesController(FileExtensionContentTypeProvider contentTypeProvider) =>
        _contentTypeProvider = contentTypeProvider;

    [HttpGet("{fileId}")]
    public ActionResult GetFile(string fileId)
    {
        var pathToFile = "580b57fcd9996e24bc43c325.png";

        if (!System.IO.File.Exists(pathToFile))
        {
            return NotFound();
        }

        if (!_contentTypeProvider.TryGetContentType(pathToFile, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var bytes = System.IO.File.ReadAllBytes(pathToFile);
        return File(bytes, contentType, "pikapika");
    }

    [HttpPost]
    public async Task<ActionResult> CreateFile(IFormFile file)
    {
        if (file.Length == 0 || file.Length > 20971520 || file.ContentType != "application/pdf")
        {
            return BadRequest("No file or an invalid one has been inputted");
        }

        var path = Path.Combine(Directory.GetCurrentDirectory(), $"uploaded_file{Guid.NewGuid()}.pdf");
        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok("Your file has been created");
    }
}