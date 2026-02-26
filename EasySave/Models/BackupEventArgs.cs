namespace EasySave.Models
{
    public class BackupEventArgs
    {
        public string BackupName { get; set; } = string.Empty;
        public string SourceFile { get; set; } = string.Empty;
        public string DestFile { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public double TransferTimeMs { get; set; }
        public long EncryptionTimeMs { get; set; }
        public int TotalFiles { get; set; }
        public int ProcessedFiles { get; set; }
        public int Progress { get; set; }
    }
}