using Microsoft.AspNetCore.Mvc;

namespace FileShareApp.Backend.Services.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file);
    Task<(Stream FileStream, string ContentType, string FileName)> DownloadAndDeleteFileAsync(string fileKey);
    Task DeleteFilesOlderThanAsync(TimeSpan maxAge, CancellationToken cancellationToken = default);
}