namespace EasySave.Models
{
    /// <summary>
    /// Event data used to notify observers about backup progress.
    /// </summary>
    public class BackupEventArgs
    {
        public string BackupName { get; set; } = string.Empty;
        public string SourceFile { get; set; } = string.Empty;
        public string DestFile { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public double TransferTimeMs { get; set; }
        public int TotalFiles { get; set; }
        public int ProcessedFiles { get; set; }

        /// <summary>
        /// Percentage of completion (0-100).
        /// </summary>
        public int Progress { get; set; }
    }
}
