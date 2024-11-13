using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
   private readonly FileExtensionContentTypeProvider _contentTypeProvider;

   public FilesController(FileExtensionContentTypeProvider contentTypeProvider) => _contentTypeProvider = contentTypeProvider;

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
}