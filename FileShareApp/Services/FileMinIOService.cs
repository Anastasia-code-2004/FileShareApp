using Minio;
using FileShareApp.Backend.Services.Interfaces;
using Minio.DataModel.Args;

namespace FileShareApp.Backend.Services;

public class FileMinIOService(IConfiguration config) : IFileService
{
    private readonly IMinioClient _minioClient = new MinioClient()
            .WithEndpoint(config["S3:Endpoint"])
            .WithCredentials(config["S3:AccessKey"], config["S3:SecretKey"])
            .WithSSL()
            .Build();
    private readonly string _bucketName = config["S3:Bucket"]!;

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        var objectName = $"{Guid.NewGuid()}{extension}";

        using var stream = file.OpenReadStream();
        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(file.ContentType ?? "application/octet-stream"));

        return objectName;
    }

    public async Task<(Stream FileStream, string ContentType, string FileName)> DownloadAndDeleteFileAsync(string objectName)
    {
        var memoryStream = new MemoryStream();
        string contentType = "application/octet-stream";

        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream)));

        memoryStream.Position = 0;

        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName));

        var fileName = objectName.Split('/').Last();
        return (memoryStream, contentType, fileName);
    }

    public async Task DeleteFilesOlderThanAsync(TimeSpan maxAge, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var listArgs = new ListObjectsArgs()
            .WithBucket(_bucketName)
            .WithRecursive(true);

        var observable = _minioClient.ListObjectsAsync(listArgs, cancellationToken);
        await foreach (var item in observable.ToAsyncEnumerable().WithCancellation(cancellationToken))
        {
            if (DateTime.TryParse(item.LastModified, out var lastModified) &&
                (now - lastModified.ToUniversalTime()) > maxAge)
            {
                await _minioClient.RemoveObjectAsync(
                    new RemoveObjectArgs()
                        .WithBucket(_bucketName)
                        .WithObject(item.Key),
                    cancellationToken);
            }
        }
    }
}
