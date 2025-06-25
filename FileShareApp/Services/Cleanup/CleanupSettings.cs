namespace FileShareApp.Backend.Services.Cleanup;

public class CleanupSettings
{
    public int CleanupHour { get; set; } = 3;
    public int CleanupMinute { get; set; } = 30;
    public int DaysToKeepFiles { get; set; } = 30;
    public bool Enabled { get; set; } = true;
}