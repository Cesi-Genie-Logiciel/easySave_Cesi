namespace EasySave.Models
{
    /// <summary>
    /// Event arguments for backup progress
    /// </summary>
    public class BackupEventArgs : EventArgs
    {
        public string BackupName { get; set; } = string.Empty;
        public string SourceFile { get; set; } = string.Empty;
        public string DestFile { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public long TransferTime { get; set; }
        public int TotalFiles { get; set; }
        public int ProcessedFiles { get; set; }
        public BackupStats Stats { get; set; } = new();  // ← AJOUTÉ
    }
}
