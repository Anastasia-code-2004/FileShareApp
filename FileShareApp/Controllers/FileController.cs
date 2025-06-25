using FileShareApp.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileShareApp.Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/files")]
public class FileController(IFileService fileService) : ControllerBase
{
    private readonly IFileService _fileService = fileService;

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var fileKey = await _fileService.UploadFileAsync(file);
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        return Ok(new { link = $"{baseUrl}/api/files/download?key={fileKey}" });
    }

    [AllowAnonymous]
    [HttpGet("download")]
    public async Task<IActionResult> DownloadFile([FromQuery] string key)
    {
        var (fileStream, contentType, fileName) = await _fileService.DownloadAndDeleteFileAsync(key);
        return File(fileStream, contentType, fileName);
    }
}

