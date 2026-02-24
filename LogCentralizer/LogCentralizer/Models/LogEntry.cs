namespace LogCentralizer.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string BackupName { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public string DestPath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public long TransferTimeMs { get; set; }
        public long? EncryptionTimeMs { get; set; }
        public string? EventType { get; set; }
        public string? Reason { get; set; }
        public string? BusinessSoftware { get; set; }
    }
}
