using System.Net;
using System.Text.Json;

namespace FileShareApp.Backend.Helpers;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred while processing request: {Message}", ex.Message);

            context.Response.ContentType = "application/json";

            var (statusCode, message) = ex switch
            {
                FileNotFoundException => (HttpStatusCode.NotFound, "File not found"),
                Minio.Exceptions.ObjectNotFoundException => (HttpStatusCode.NotFound, "File not found"),
                Minio.Exceptions.ErrorResponseException => (HttpStatusCode.BadRequest, "Storage error"),
                UnauthorizedAccessException => (HttpStatusCode.Forbidden, "Access denied"),
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid input"),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                error = message,
                status = context.Response.StatusCode
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
