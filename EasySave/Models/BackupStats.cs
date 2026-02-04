namespace EasySave.Models
{
    /// <summary>
    /// Statistics and progress tracking for a backup job
    /// </summary>
    public class BackupStats
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string State { get; set; } = "Inactive"; // "Active", "Inactive", "Completed"

        // Progress tracking
        public int TotalFiles { get; set; }
        public int FilesRemaining { get; set; }
        public long TotalSize { get; set; }
        public long SizeRemaining { get; set; }

        // Current file being processed
        public string CurrentSourceFile { get; set; } = string.Empty;
        public string CurrentDestFile { get; set; } = string.Empty;

        public int Progress => TotalFiles > 0
            ? (int)((TotalFiles - FilesRemaining) * 100.0 / TotalFiles)
            : 0;

        public BackupStats()
        {
            Timestamp = DateTime.Now;
        }
    }
}
