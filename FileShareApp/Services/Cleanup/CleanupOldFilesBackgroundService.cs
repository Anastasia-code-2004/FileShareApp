using FileShareApp.Backend.Services.Interfaces;

using Microsoft.Extensions.Options;

namespace FileShareApp.Backend.Services.Cleanup;

public class CleanupOldFilesBackgroundService(
    IFileService fileService,
    ILogger<CleanupOldFilesBackgroundService> logger,
    IOptions<CleanupSettings> settings) : BackgroundService
{
    private readonly IFileService _fileService = fileService;
    private readonly ILogger<CleanupOldFilesBackgroundService> _logger = logger;
    private readonly CleanupSettings _settings = settings.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("Cleanup service is disabled");
            return;
        }

        _logger.LogInformation("CleanupOldFilesBackgroundService started. Settings: {@Settings}", _settings);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;
                var nextRunTime = GetNextRunTime(now);

                var delay = nextRunTime - now;
                if (delay > TimeSpan.Zero)
                {
                    _logger.LogInformation($"Next cleanup at {nextRunTime}. Waiting {delay}...");
                    await Task.Delay(delay, stoppingToken);
                }

                _logger.LogInformation("Starting cleanup of old files...");
                await _fileService.DeleteFilesOlderThanAsync(
                    TimeSpan.FromDays(_settings.DaysToKeepFiles),
                    stoppingToken);
                _logger.LogInformation("Cleanup completed.");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error during cleanup of old files.");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); 
            }
        }
    }

    private DateTime GetNextRunTime(DateTime now)
    {
        return now.Date.AddDays(1)
            .AddHours(_settings.CleanupHour)
            .AddMinutes(_settings.CleanupMinute);
    }
}